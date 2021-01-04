using System;
using System.Threading.Tasks;
using Core.Properties;
using Acr.UserDialogs;
using Plugin.Media.Abstractions;
using System.IO;

#nullable enable
namespace NativeVyatka
{
    public interface IObservable
    {
        void AddObserver(IMainObserver observer);
        void AddObserver(IRecordsObserver observer);
        void AddObserver(IMapObserver observer);
        void AddObserver(IProfileObserver observer);
        void RemoveObserver(IMainObserver observer);
        void RemoveObserver(IRecordsObserver observer);
        void RemoveObserver(IProfileObserver obserbver);
        void RemoveObserver(IMapObserver observer);
    }

    public interface IMainObserver
    {
        void UpdateScreenState(int tab);
        void UpadateGpsEnableState(GpsState e);
        void OpenSettiongs();
    }

    public interface IMainPresenter : IMainRecordsPresenter, IMainMapPresenter, IProfilePresenter, IFavoritesPresenter, IDisposable
    {
        void InitFragmentState();
        void CreateNewBurial();
        void GoBack();
    }

    public partial class MainPresenter : IMainPresenter
    {
        private readonly IBurialsNetworkProvider burialsNetworkProvider;
        private readonly IBurialStorage bStorage;
        private readonly ISettingsProvider settings;
        private readonly IRouter router;
        private readonly IGeolocationService geolocationProvider;
        private readonly IPermissionsProvider permissions;
        private readonly IMedia media;
        private readonly IUserDialogs dialogs;
        private IMainObserver? mainObserver;

        public MainPresenter(
            IBurialsNetworkProvider burialsNetworkProvider,
            IRouter router,
            IProfileStorage pstorage,
            IBurialStorage bstorage,
            IUserDialogs dialogs,
            IMedia media,
            ISettingsProvider settings,
            IGeolocationService geolocationProvider,
            IPermissionsProvider permissions) {
            this.router = router;
            this.pStorage = pstorage;
            this.bStorage = bstorage;
            this.settings = settings;
            this.burialsNetworkProvider = burialsNetworkProvider;
            this.geolocationProvider = geolocationProvider;
            this.permissions = permissions;
            this.media = media;
            this.dialogs = dialogs;
            this.geolocationProvider.OnGpsEnableChanged += OnGpsEnableChanged;
        }

        public void Dispose() {
            geolocationProvider.OnGpsEnableChanged -= OnGpsEnableChanged;
            geolocationProvider.Disconnect();
            //loginDataProvider.Cancel();
        }

        public void AddObserver(IMainObserver observer) {
            this.mainObserver = observer;
        }

        public void RemoveObserver(IMainObserver observer) {
            this.mainObserver = null;
        }

        public void InitFragmentState() {
            mainObserver?.UpdateScreenState(Resource.Id.navigation_my_records);
        }

        public async void CreateNewBurial() {
            try {
                if ((await permissions.IsCameraPermissionGrantedAsync() == false) ||
                    (await permissions.IsLocationPermissionGrantedAsync() == false) ||
                    (await permissions.IsStoragePermissionGrantedAsync() == false)) {
                    await dialogs.AlertAsync("Вы не предоставили все необходимые разрешения для работы приложения. Зайдите в настройки и включите их", Resources.Dialog_Attention);
                    return;
                }
                if (geolocationProvider.IsGeolocationAvailable == false) {
                    await dialogs.AlertAsync("Не удается получить доступ к модулю геолокацци. Проверьте, включен ли gps.", Resources.Dialog_Attention);
                    return;

                }
                dialogs.ShowLoading();
                var position = await geolocationProvider.GetPositionAsync();
                var path = await CreatePhoto();
                if (!string.IsNullOrEmpty(path)) {
                    try {
                        var burial = new BurialModel(settings.UserHash) {
                            PicturePath = path,
                            BirthDay = "00-00-0000",
                            DeathDay = "00-00-0000"
                        };
                        burial.Location.Latitude = position.Location.Latitude;
                        burial.Location.Longitude = position.Location.Longitude;
                        burial.Location.Accuracy = position.Location.Accuracy;
                        burial.Location.Altitude = position.Location.Altitude;
                        burial.Location.Heading = position.Heading;
                        DisplayBurial(burial);
                    }
                    catch (Exception ex) {
                        iConsole.Error(ex);
                        await dialogs.AlertAsync(Resources.MainScreeen_GpsNotAvailable, Resources.Dialog_Attention);
                    }
                }
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                await dialogs.AlertAsync(Resources.MainScreeen_Error, Resources.Dialog_Attention);
            }
            finally {
                dialogs.HideLoading();
            }
        }

        protected async Task<string> CreatePhoto() {
            try {
                if (media.IsCameraAvailable && media.IsTakePhotoSupported) {
                    var file = await media.TakePhotoAsync(new StoreCameraMediaOptions {
                        PhotoSize = PhotoSize.Full,
                        CompressionQuality = 75,
                        Directory = "BurialFolder",
                        Name = Path.GetRandomFileName() + ".jpg"
                    });
                    if (file is not null && string.IsNullOrEmpty(file.Path) == false) {
                        return file.Path;
                    }
                }
                else {
                    await dialogs.AlertAsync(Resources.MainScreeen_CameraNotAvailable, Resources.Dialog_Attention);
                }
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                dialogs.HideLoading();
                await dialogs.AlertAsync(Resources.MainScreeen_CameraException, Resources.Dialog_Attention);
            }
            return string.Empty;
        }

        private void OnGpsEnableChanged(object sender, GpsState e) {
            mainObserver?.UpadateGpsEnableState(e);
        }

        public void GoBack() {
            router.GoBack();
        }
    }
}
#nullable restore