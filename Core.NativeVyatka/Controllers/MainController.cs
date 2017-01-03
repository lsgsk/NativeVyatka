using Abstractions.Interfaces.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abstractions.Models.AppModels;
using Abstractions.Interfaces.Plugins;
using Abstractions.Models;
using Plugin.Geolocator;
using Newtonsoft.Json;
using Abstractions;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Database.Tables;
using NativeVyatkaCore.Utilities;

namespace NativeVyatkaCore.Controllers
{
    public class MainController : BaseController, IMainController
    {
        public MainController(ICrossPageNavigator navigator, IProfileStorage pstorage, IBurialStorage bstorage)
        {
            this.mNavigator = navigator;
            this.mPstorage = pstorage;
            this.mBstorage = bstorage;
        }

        public async Task CreateNewBurial()
        {
            var burial = new BurialModel();
            if (CrossGeolocator.Current.IsGeolocationAvailable)
            {
                Progress = true;
                var path = await CreatePhoto();
                if(!string.IsNullOrEmpty(path))
                {
                    burial.PicturePath = path;
                    try
                    {
                        var position = await CrossGeolocator.Current.GetPositionAsync(10000);
                        burial.Location.Latitude = position.Latitude;
                        burial.Location.Longitude = position.Longitude;
                        burial.Location.Altitude = position.Altitude;
                        burial.Location.Heading = position.Heading;
                        DisplayBurial(burial);
                    }
                    catch(Exception ex)
                    {
                        iConsole.Error(ex);
                        await AlertAsync("В настоящее время gps не доступен. Сделать запись невозможно", "Внимание");
                    }                 
                }
                Progress = false;
            }
            else
            {
                await AlertAsync("В настоящее время gps не доступен. Сделать запись невозможно", "Внимание");
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
                //тут отправить, всех, кто не синхранизирован
                await Task.Delay(1500);
                Progress = false;
            }
            catch (BurialSyncException)
            {
                Progress = false;
                await AlertAsync("Синхранизация не удалась");
            }
        }

        private readonly IProfileStorage mPstorage;
        private readonly IBurialStorage mBstorage;
        private readonly ICrossPageNavigator mNavigator;
    }
}
