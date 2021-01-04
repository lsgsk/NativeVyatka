using Acr.UserDialogs;
using Core.Properties;
using System;
using System.Threading.Tasks;

#nullable enable
namespace NativeVyatka
{
    public interface IBurialEditObserver
    {
        void DisplayBurial(BurialViewModel burial);
        void UpdatedBurialSaveState(bool value);
    }

    public interface IBurialEditObservable
    {
        void AddObserver(IBurialEditObserver observer);
        void RemoveObserver(IBurialEditObserver observer);
    }

    public interface IBurialEditPresenter : IBurialEditObservable, IDisposable
    {
        void DisplayBurial(BurialModel model, Action closeCallback);
        void UpdateBurial(int id, string value);
        void UpdateBurial(int id, string day, string month, string year);
        void UpdateBurial(bool favorite);

        Task SaveAndUploadBurialAsync();
        Task SaveAndUploadBurialAndGoBackAsync();
        Task DeleteRecordAsync();
    }

    public class BurialEditPresenter : IBurialEditPresenter
    {
        private readonly IRouter router;
        private readonly IBurialImageGuide burialImageGuide;
        private readonly IBurialsNetworkProvider burialsDataProvider;
        private readonly IUserDialogs dialogs;
        private BurialModel? initBurial;
        private BurialModel? burial;
        private Action? closeCallback;
        private IBurialEditObserver? observer;

        public BurialEditPresenter(
            IRouter router,
            IBurialImageGuide burialImageGuide,
            IBurialsNetworkProvider burialsDataProvider,
            IUserDialogs dialogs) {
            this.router = router;
            this.burialImageGuide = burialImageGuide;
            this.burialsDataProvider = burialsDataProvider;
            this.dialogs = dialogs;
        }

        public void AddObserver(IBurialEditObserver observer) {
            this.observer = observer;
        }

        public void RemoveObserver(IBurialEditObserver observer) {
            this.observer = null;
        }

        public void DisplayBurial(BurialModel burial, Action closeCallback) {
            this.initBurial = burial.Copy();
            if (burial.RecordTime == DateTime.MinValue) {
                //это создание, а не редактирование
                burial.RecordTime = DateTime.UtcNow;
            }
            this.burial = burial.Copy();
            this.observer?.DisplayBurial(PrepareBurialModel(burial));
            this.observer?.UpdatedBurialSaveState(burial != initBurial);
            this.closeCallback = closeCallback;
        }

        public void UpdateBurial(int id, string value) {
            if (burial is not null) {
                switch (id) {
                    case Resource.Id.etName:
                        burial.Name = value;
                        break;
                    case Resource.Id.etSurname:
                        burial.Surname = value;
                        break;
                    case Resource.Id.etPatronymic:
                        burial.Patronymic = value;
                        break;
                    case Resource.Id.etDescription:
                        burial.Description = value;
                        break;
                    default:
                        break;
                }
            }
            this.observer?.UpdatedBurialSaveState(burial != initBurial);
        }

        public void UpdateBurial(int id, string day, string month, string year) {
            if (burial is not null) {
                switch (id) {
                    case Resource.Id.etBirthTimeDay:
                    case Resource.Id.etBirthTimeMonth:
                    case Resource.Id.etBirthTimeYear:
                        burial.BirthDay = $"{(string.IsNullOrEmpty(day) ? "00" : day.ToString())}-" +
                      $"{(string.IsNullOrEmpty(month) ? "00" : month.ToString())}-" +
                      $"{(string.IsNullOrEmpty(year) ? "0000" : year.ToString())}";

                        break;
                    case Resource.Id.etDeathTimeDay:
                    case Resource.Id.etDeathTimeMonth:
                    case Resource.Id.etDeathTimeYear:
                        burial.DeathDay = $"{(string.IsNullOrEmpty(day) ? "00" : day.ToString())}-" +
                      $"{(string.IsNullOrEmpty(month) ? "00" : month.ToString())}-" +
                      $"{(string.IsNullOrEmpty(year) ? "0000" : year.ToString())}";
                        break;
                    default:
                        break;
                }
            }
            this.observer?.UpdatedBurialSaveState(burial != initBurial);
        }

        public void UpdateBurial(bool favorite) {
            if (burial is not null) {
                burial.Favorite = favorite;
            }
            this.observer?.UpdatedBurialSaveState(burial != initBurial);
        }

        private BurialViewModel PrepareBurialModel(BurialModel burial) {
            var birth = burial.BirthDay.Split('-');
            var birthDay = new BurialViewModel.DateViewModel(day: birth[0] == "00" ? string.Empty : birth[0],
                                                             month: birth[1] == "00" ? string.Empty : birth[1],
                                                             year: birth[2] == "0000" ? string.Empty : birth[2]);
            var death = burial.DeathDay.Split('-');
            var deathDay = new BurialViewModel.DateViewModel(day: death[0] == "00" ? string.Empty : death[0],
                                                             month: death[1] == "00" ? string.Empty : death[1],
                                                             year: death[2] == "0000" ? string.Empty : death[2]);

            var image = new BurialViewModel.ImageViewModel(burial.PicturePath);

            return new BurialViewModel(title: $"{burial.Name} {burial.Surname} {burial.Patronymic}",
                                       image: image,
                                       surname: burial.Surname,
                                       name: burial.Name,
                                       patronymic: burial.Patronymic,
                                       description: burial.Description,
                                       recordTime: burial.RecordTime.ToShortDateString(),
                                       birthDay: birthDay,
                                       deathDay: deathDay,
                                       isFavorite: burial.Favorite);
        }

        public async Task SaveAndUploadBurialAsync() {
            if (burial is not null) {
                try {
                    dialogs.ShowLoading();
                    await burialsDataProvider.UploadBurialAsync(burial);
                    try {
                        await burialsDataProvider.SynchronizeBurialsAsync();
                    }
                    catch {
                        //синхранизация всех должна быть скрытой
                    }
                    initBurial = burial.Copy();
                    this.observer?.UpdatedBurialSaveState(false);
                    dialogs.HideLoading();
                }
                catch (BurialSyncException) {
                    dialogs.HideLoading();
                    await dialogs.AlertAsync(Resources.EditScreeen_SyncFailed, Resources.Dialog_Attention);
                }
                catch (Exception ex) {
                    dialogs.HideLoading();
                    iConsole.Error(ex);
                }
                finally {
                    closeCallback?.Invoke();
                    router.GoBack();
                }
            }
        }

        public async Task SaveAndUploadBurialAndGoBackAsync() {
            if (burial is not null && burial != initBurial) {
                var confirm = await ConfirmAsync(Resources.EditScreeen_SaveAndSync, okText: Resources.Dialog_Yes, cancelText: Resources.Dialog_No);
                if (confirm) {
                    await SaveAndUploadBurialAsync();
                }
                else {
                    await TryDeletePicture(burial.PicturePath);
                    router.GoBack();
                }
            }
            else {
                router.GoBack();
            }
        }

        public async Task DeleteRecordAsync() {
            if (burial is not null) {
                var confirm = await ConfirmAsync(burial.Uploaded ? Resources.EditScreeen_SyncDeleteQuestion : Resources.EditScreeen_NoSyncDeleteQuestion, Resources.Dialog_Attention);
                if (confirm) {
                    try {
                        dialogs.ShowLoading();
                        await burialsDataProvider.RemoveBurialAsync(burial);
                        await dialogs.AlertAsync(Resources.EditScreeen_DeleteFinised);
                        dialogs.HideLoading();
                        closeCallback?.Invoke();
                        router.GoBack();
                    }
                    catch (Exception ex) {
                        dialogs.HideLoading();
                        await dialogs.AlertAsync(Resources.EditScreeen_DeleteFailed);
                        iConsole.Error(ex);
                    }
                }
            }
        }

        private async Task TryDeletePicture(string picturePath) {
            try {
                await burialImageGuide.DeleteFromFileSystemAsync(picturePath);
            }
            catch (FileGuideException) {
            }
        }

        private Task<bool> ConfirmAsync(string message, string? title = null, string? okText = null, string? cancelText = null) {
            return dialogs.ConfirmAsync(message, title, okText, cancelText);
        }

        public void Dispose() {
        }
    }
}
#nullable restore