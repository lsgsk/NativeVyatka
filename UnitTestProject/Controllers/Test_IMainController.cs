using Abstractions.Interfaces.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Abstractions;
using Abstractions.Models;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Plugins;
using FluentAssertions;
using Abstractions.Models.AppModels;
using Abstractions.Interfaces.Database.Tables;
using UnitTestProject.Database.Table;
using Plugin.Geolocator.Abstractions;
using NativeVyatkaCore.Properties;
using Acr.UserDialogs;
using Plugin.Media.Abstractions;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Abstractions.Interfaces.Network;
using UnitTestProject.Network;

namespace UnitTestProject.Controllers
{
    [TestClass]
    public class Test_IMainController
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            Test.Container.Resolve<ISettingsProvider>().ClearPrefs();
            Test.Container.Resolve<IDataStorage>().ClearDataBase();
        }

        public static IMainController CreateController(bool network = true, string message = null, string title = null, TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>> navigationCallback = null,  bool gps = true, bool gpsTaken = true, bool camera = true, bool photoTaken = true)
        {
            var container = Test.CreateChildContainer();
            container.RegisterInstance<IBurialsNetworkProvider>(Test_IBurialsNetworkProvider.CreateProvider(network));
            container.RegisterInstance<IUserDialogs>(Test.CreateMockUserDialog(message, title));
            container.RegisterInstance<ICrossPageNavigator>(Test.CreateMockNavigation(navigationCallback));
            container.RegisterInstance<IGeolocator>(Test.CreateMockGeolocator(gps, gpsTaken));
            container.RegisterInstance<IMedia>(Test.CreateMockMedia(camera, photoTaken));
            return container.Resolve<IMainController>();
        }

        [TestMethod]
        public void SuccessCheckProfile()
        {
            var profile = Test_IProfileStorage.CreateProfile();
            Test.Container.Resolve<IProfileStorage>().SaveProfile(profile);
            var controller = CreateController();
            controller.Profile.ShouldBeEquivalentTo(profile);
        }

        [TestMethod]
        public void SuccessCheckNotExistProfile()
        {
            var controller = CreateController();
            controller.Profile.Should().Be(ProfileModel.Null);
        }

        [TestMethod]
        public async Task CreateNewBurialWitoutEnableGps()
        {
            var controller = CreateController(message: Resources.MainScreeen_GpsNotAvailable, title: Resources.Dialog_Attention, gps:false);
            await controller.CreateNewBurial();
        }

        [TestMethod]
        public async Task CreateNewBurialWitoutGpsOverHead()
        {
            var controller = CreateController(message: Resources.MainScreeen_GpsNotAvailable, title: Resources.Dialog_Attention, gps: true, gpsTaken:false);
            await controller.CreateNewBurial();
        }

        [TestMethod]
        public async Task CreateNewBurialWitoutEnableCamera()
        {
            var controller = CreateController(message: Resources.MainScreeen_CameraNotAvailable, title: Resources.Dialog_Attention, gps: false);
            await controller.CreateNewBurial();
        }

        [TestMethod]
        public async Task CreateNewBurialButCancelCamera()
        {
            var controller = CreateController(photoTaken: false);
            await controller.CreateNewBurial();
        }

        [TestMethod]
        public async Task SuccessCreateNewBurial()
        {
            var callback = new TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>>();
            var controller = CreateController(navigationCallback: callback);
            await controller.CreateNewBurial();
            var result = await callback.Task;
            result.Item1.Should().Be(PageStates.BurialEditPage);
            var burial = JsonConvert.DeserializeObject<BurialModel>(result.Item2[FormBundleConstants.BurialModel]);
            burial.Should().NotBeNull();
            //большинство параметров рандомны, если валидный объект, то он корректный            
        }

        [TestMethod]
        public void CheckGetBurialsForEmptyStorage()
        {
            var controller = CreateController();
            var collection = (controller as IMainRecordsController).GetBurials();
            collection.Should().BeEmpty();
        }


        [TestMethod]
        public void CheckGetBurials()
        {
            var storage = Test.Container.Resolve<IBurialStorage>();
            var controller = CreateController() as IMainRecordsController;
            controller.GetBurials().Should().BeEmpty();
            var collection = Test_IBurialStorage.CreateBurialsCollection();
            foreach (var burial in collection)
            {
                storage.InsertOrUpdateBurial(burial);
            }
            controller.GetBurials().ShouldAllBeEquivalentTo(collection);
        }

        [TestMethod]
        public async Task CheckDisplayBurial()
        {
            var callback = new TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>>();
            var controller = CreateController(navigationCallback: callback) as IMainRecordsController;
            var burial = Test_IBurialStorage.CreateBurial();
            controller.DisplayBurial(burial);
            var result = await callback.Task;
            result.Item1.Should().Be(PageStates.BurialEditPage);
            var serialized = JsonConvert.DeserializeObject<BurialModel>(result.Item2[FormBundleConstants.BurialModel]);
            serialized.ShouldBeEquivalentTo(burial);
        }

        [TestMethod]
        public async Task TryForceSyncBurialsEmpty()
        {
            var controller = CreateController();
            await controller.ForceSyncBurials();
        }

        [TestMethod]
        public async Task TryForceSyncBurials()
        {
            var storage = Test.Container.Resolve<IBurialStorage>();
            var controller = CreateController();
            var collection = Test_IBurialStorage.CreateBurialsCollection();
            foreach (var burial in collection)
            {
                storage.InsertOrUpdateBurial(burial);
            }
            await controller.ForceSyncBurials();            
        }

        [TestMethod]
        public async Task FailForceSyncBurials()
        {
            var controller = CreateController(network:false, message: Resources.MainScreeen_SyncFailed, title: Resources.Dialog_Attention);
            await controller.ForceSyncBurials();
        }
    }
}
