using Abstractions.Interfaces.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abstractions.Models.AppModels;
using Abstractions.Interfaces.Plugins;
using Abstractions.Models;
using Newtonsoft.Json;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Database.Tables;
using NativeVyatkaCore.Utilities;
using Plugin.Geolocator.Abstractions;
using NativeVyatkaCore.Properties;
using Acr.UserDialogs;
using Plugin.Media.Abstractions;
using Abstractions.Interfaces.Network;
using System.Linq;

namespace NativeVyatkaCore.Controllers
{
    public class MainController : BaseController, IMainController
    {
        public MainController(IBurialsNetworkProvider burialsNetworkProvider, ICrossPageNavigator navigator, IProfileStorage pstorage, IBurialStorage bstorage, IGeolocator geolocator, IUserDialogs dialogs, IMedia media) : base(dialogs, media)
        {
            this.mNavigator = navigator;
            this.mPstorage = pstorage;
            this.mBstorage = bstorage;
            this.mGeolocator = geolocator;
            this.mBurialsNetworkProvider = burialsNetworkProvider;
            this.mGeolocator.StartListeningAsync(new TimeSpan(10000), 5, true);
        }

        public async override void Dispose()
        {
            base.Dispose();
            await mGeolocator.StopListeningAsync();
        }

        public async Task CreateNewBurial()
        {
            var burial = new BurialModel();
            if (mGeolocator.IsGeolocationAvailable)
            {
                Progress = true;
                var path = await CreatePhoto();
                if(!string.IsNullOrEmpty(path))
                {
                    burial.PicturePath = path;
                    try
                    {
                        var position = await mGeolocator.GetPositionAsync(new TimeSpan(5000));
                        burial.Location.Latitude = position.Latitude;
                        burial.Location.Longitude = position.Longitude;
                        burial.Location.Altitude = position.Altitude;
                        burial.Location.Heading = position.Heading;
                        DisplayBurial(burial);
                    }
                    catch(Exception ex)
                    {
                        iConsole.Error(ex);
                        await AlertAsync(Resources.MainScreeen_GpsNotAvailable, Resources.Dialog_Attention);
                    }                 
                }
                Progress = false;
            }
            else
            {
                await AlertAsync(Resources.MainScreeen_GpsNotAvailable, Resources.Dialog_Attention);
            }            
        }

        private ProfileModel profile;
        public ProfileModel Profile
        {
            get
            {
                profile = profile ?? mPstorage.GetProfile();
                return profile;
            }
        }

        public List<BurialModel> GetBurials()
        {
            return  mBstorage.GetBurials();
        }

        public void DisplayBurial(BurialModel burial)
        {
            mNavigator.GoToPage(PageStates.BurialEditPage, new Dictionary<string, string>()
            {
                [FormBundleConstants.BurialModel] = JsonConvert.SerializeObject(burial)
            });
        }

        public async Task ForceSyncBurials()
        {
            try
            {
                Progress = true;                
                await mBurialsNetworkProvider.SynchronizeBurialsAsync();                
                Progress = false;
            }
            catch (BurialSyncException)
            {
                Progress = false;
                await AlertAsync(Resources.MainScreeen_SyncFailed, Resources.Dialog_Attention);
            }
        }

        private IBurialsNetworkProvider mBurialsNetworkProvider;
        private readonly IGeolocator mGeolocator;
        private readonly IProfileStorage mPstorage;
        private readonly IBurialStorage mBstorage;
        private readonly ICrossPageNavigator mNavigator;
    }
}
