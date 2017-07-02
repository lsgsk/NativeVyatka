using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Plugin.Media.Abstractions;
using Abstractions.Interfaces.Plugins;
using Acr.UserDialogs;
using Abstractions.Interfaces.Network;
using UnitTestProject.Network;
using Abstractions.Interfaces.Controllers;
using Abstractions.Models;
using Abstractions.Interfaces.Settings;
using Abstractions;
using Abstractions.Models.AppModels;
using FluentAssertions;
using UnitTestProject.Database.Table;
using NativeVyatkaCore.Properties;
using Abstractions.Interfaces.Database.Tables;

namespace UnitTestProject.Controllers
{
    [TestClass]
    public class Test_IBurialEditController
    {
        public static IBurialEditController CreateController(bool network = true, string message = null, string title = null, bool select = true,  TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>> navigationCallback = null, bool gps = true, bool gpsTaken = true, bool camera = true, bool photoTaken = true)
        {
            var container = Test.CreateChildContainer();
            container.RegisterInstance<IBurialsNetworkProvider>(Test_IBurialsNetworkProvider.CreateProvider(network));
            container.RegisterInstance<IUserDialogs>(Test.CreateMockUserDialog(message, title, select));
            container.RegisterInstance<ICrossPageNavigator>(Test.CreateMockNavigation(navigationCallback));
            container.RegisterInstance<IMedia>(Test.CreateMockMedia(camera, photoTaken));
            return container.Resolve<IBurialEditController>();
        }

        [TestInitialize]
        public void PrepareDatabase()
        {
            Test.Container.Resolve<ISettingsProvider>().ClearPrefs();
            Test.Container.Resolve<IDataStorage>().ClearDataBase();
        }

        [TestMethod]
        public void CheckNullBurial()
        {
            var controller = CreateController();
            controller.Burial.Should().Be(BurialModel.Null);
        }

        [TestMethod]
        public void CheckBurialProparty()
        {
            var burial = Test_IBurialStorage.CreateBurial();
            var controller = CreateController();
            controller.Burial = burial;
            controller.Burial.Should().Be(burial);
        }

        [TestMethod]
        public async Task TryForceGoBack()
        {
            var callback = new TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>>();
            var controller = CreateController(navigationCallback: callback, message: Resources.EditScreen_OpeningFailed, title: Resources.Dialog_Attention);
            controller.ForceGoBack();
            var result = await callback.Task;
            result.Item1.Should().Be(PageStates.BulialListPage);
        }

        [TestMethod]
        public async Task SuccessRetakePhoto()
        {
            var controller = CreateController();
            controller.Burial = Test_IBurialStorage.CreateBurial();
            var last = controller.Burial.PicturePath;
            await controller.RetakePhotoAsync();
            controller.Burial.PicturePath.Should().NotBe(last);
        }

        [TestMethod]
        public async Task CancelRetakePhoto()
        {
            var controller = CreateController(photoTaken: false);
            controller.Burial = Test_IBurialStorage.CreateBurial();
            var last = controller.Burial.PicturePath;
            await controller.RetakePhotoAsync();
            controller.Burial.PicturePath.Should().Be(last);
        }

        [TestMethod]
        public async Task TrySetTime()
        {
            var controller = CreateController();
            controller.Burial = Test_IBurialStorage.CreateBurial();
            controller.Burial.BirthDay = controller.Burial.DeathDay = null;
            await controller.SetBirthTimeAsync();
            await controller.SetDeathTimeAsync();
            controller.Burial.BirthDay.Value.ToShortDateString().Should().Be(DateTime.UtcNow.ToShortDateString());
            controller.Burial.DeathDay.Value.ToShortDateString().Should().Be(DateTime.UtcNow.ToShortDateString());
        }

        [TestMethod]
        public async Task SuccessSaveAndUploadBurialAsync()
        {
            var controller = CreateController(message: Resources.EditScreen_SyncSuccess);
            controller.Burial = Test_IBurialStorage.CreateBurial();
            controller.Burial.Updated = false;
            await controller.SaveAndUploadBurialAsync();
            var dbBurial = Test.Container.Resolve<IBurialStorage>().GetBurial(controller.Burial.CloudId);
            dbBurial.Should().NotBeNull();
            dbBurial.Updated.Should().BeTrue();
        }

        [TestMethod]
        public async Task FailedSaveAndUploadBurialAsync()
        {
            var controller = CreateController(network: false, message: Resources.EditScreeen_SyncFailed, title: Resources.Dialog_Attention);
            controller.Burial = Test_IBurialStorage.CreateBurial();
            controller.Burial.Updated = false;
            await controller.SaveAndUploadBurialAsync();
            var dbBurial = Test.Container.Resolve<IBurialStorage>().GetBurial(controller.Burial.CloudId);
            dbBurial.Should().NotBeNull();
            dbBurial.Updated.Should().BeFalse();
        }

        [TestMethod]
        public async Task SaveAndUploadBurialAndGoBackAsync()
        {
            var callback = new TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>>();
            var controller = CreateController(navigationCallback:callback, message: Resources.EditScreen_SyncSuccess);
            controller.Burial = Test_IBurialStorage.CreateBurial();
            controller.Updated = true;
            await controller.SaveAndUploadBurialAndGoBackAsync();
            var dbBurial = Test.Container.Resolve<IBurialStorage>().GetBurial(controller.Burial.CloudId);            
            dbBurial.Updated.Should().BeTrue();
            controller.Burial.Updated = true;
            dbBurial.ShouldBeEquivalentTo(controller.Burial);
            var result = await callback.Task;
            result.Item1.Should().Be(PageStates.BulialListPage);
        }

        [TestMethod]
        public async Task CancelSaveAndUploadBurialAndGoBackAsync()
        {
            var callback = new TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>>();
            var controller = CreateController(select:false, navigationCallback: callback);
            controller.Burial = Test_IBurialStorage.CreateBurial();
            controller.Updated = true;
            await controller.SaveAndUploadBurialAndGoBackAsync();
            var dbBurial = Test.Container.Resolve<IBurialStorage>().GetBurial(controller.Burial.CloudId);
            dbBurial.ShouldBeEquivalentTo(BurialModel.Null);
            var result = await callback.Task;
            result.Item1.Should().Be(PageStates.BulialListPage);
        }

        [TestMethod]
        public async Task DeleteRecordAsync()
        {
            var storage = Test.Container.Resolve<IBurialStorage>();
            var callback = new TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>>();
            var controller = CreateController(navigationCallback: callback, message: Resources.EditScreeen_DeleteFinised);
            controller.Burial = Test_IBurialStorage.CreateBurial();
            storage.InsertOrUpdateBurial(controller.Burial);
            await controller.DeleteRecordAsync();
            var dbBurial = storage.GetBurial(controller.Burial.CloudId);
            dbBurial.ShouldBeEquivalentTo(BurialModel.Null);
            var result = await callback.Task;
            result.Item1.Should().Be(PageStates.BulialListPage);
        }

        [TestMethod]
        public async Task CancelDeleteRecordAsync()
        {
            var storage = Test.Container.Resolve<IBurialStorage>();
            var callback = new TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>>();
            var controller = CreateController(select:false, navigationCallback: callback);
            controller.Burial = Test_IBurialStorage.CreateBurial();
            storage.InsertOrUpdateBurial(controller.Burial);
            await controller.DeleteRecordAsync();
            var dbBurial = storage.GetBurial(controller.Burial.CloudId);
            dbBurial.ShouldBeEquivalentTo(controller.Burial);
            var result = await callback.Task;
            result.Item1.Should().Be(PageStates.BulialListPage);
        }
    }
}
