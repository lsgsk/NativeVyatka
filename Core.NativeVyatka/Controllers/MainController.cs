﻿using Abstractions.Interfaces.Controllers;
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
using Abstractions.Interfaces.Utilities;
using Abstractions;
using Abstractions.Interfaces.Settings;
using DeviceMotion.Plugin.Abstractions;

namespace NativeVyatkaCore.Controllers
{
    public class MainController : BaseController, IMainController
    {
        public MainController(IBurialsNetworkProvider burialsNetworkProvider, ICrossPageNavigator navigator, IProfileStorage pstorage, IBurialStorage bstorage, IGeolocator geolocator, IDeviceMotion compass, IUserDialogs dialogs, IMedia media, IGpsSatelliteManager satelliteManager, IDataStorage storage, ISettingsProvider settings) : base(dialogs, media)
        {
            this.mNavigator = navigator;
            this.mPstorage = pstorage;
            this.mBstorage = bstorage;
            this.storage = storage;
            this.settings = settings;
            this.geolocator = geolocator;
            this.compass = compass;
            this.satelliteManager = satelliteManager;
            this.mBurialsNetworkProvider = burialsNetworkProvider;
            this.geolocator.StartListeningAsync(new TimeSpan(10000), 5, true);
            this.compass.SensorValueChanged += this.OnSensorValueChanged;
            this.compass.Start(MotionSensorType.Compass);
            this.satelliteManager.OnGpsEnableChanged += OnGpsEnableChanged;
        }

        public async override void Dispose()
        {
            base.Dispose();
            await geolocator.StopListeningAsync();
            this.compass.SensorValueChanged -= OnSensorValueChanged;
        }

        private void OnSensorValueChanged(object sender, SensorValueChangedEventArgs e)
        {
            heading = e.Value.Value;
        }      

        public void Logout()
        {
            settings.ClearPrefs();
            storage.ClearDataBase();
            mNavigator.GoToPage(PageStates.LoginPage, closePrevious:true);
        }

        public async Task CreateNewBurial()
        {
            try
            {
                var burial = new BurialModel();
                if (geolocator.IsGeolocationAvailable)
                {
                    Progress = true;
                    var position = await geolocator.GetPositionAsync(TimeSpan.FromSeconds(15));
                    var path = await CreatePhoto();
                    if (!string.IsNullOrEmpty(path))
                    {
                        burial.PicturePath = path;
                        try
                        {
                            burial.Location.Latitude = position.Latitude;
                            burial.Location.Longitude = position.Longitude;
                            burial.Location.Altitude = position.Altitude;
                            burial.Location.Heading = heading;
                            burial.BirthDay = "00-00-0000";
                            burial.DeathDay = "00-00-0000";
                            DisplayBurial(burial);
                        }
                        catch (Exception ex)
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
            catch(Exception ex)
            {
                iConsole.Error(ex);
                await AlertAsync(Resources.MainScreeen_Error, Resources.Dialog_Attention);
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

        private void OnGpsEnableChanged(object sender, int e)
        {
            GpsEnableChanged?.Invoke(this, e);
        }

        private double? heading = null;
        private IBurialsNetworkProvider mBurialsNetworkProvider;
        private readonly IGeolocator geolocator;
        private readonly IDeviceMotion compass;
        private readonly IProfileStorage mPstorage;
        private readonly IBurialStorage mBstorage;
        private readonly IDataStorage storage;
        private readonly ISettingsProvider settings;
        private readonly ICrossPageNavigator mNavigator;
        private readonly IGpsSatelliteManager satelliteManager;
        public event EventHandler<int> GpsEnableChanged;
    }
}
