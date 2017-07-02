using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Abstractions.Interfaces.Settings;
using Abstractions;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Network;
using UnitTestProject.Network;
using Abstractions.Interfaces.Plugins;
using Abstractions.Exceptions;
using FluentAssertions;
using NativeVyatkaCore.Properties;
using Abstractions.Models;
using Acr.UserDialogs;
using System.Collections.Generic;
using System;

namespace UnitTestProject.Controllers
{
    [TestClass]
    public class Test_ILoginController
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            Test.Container.Resolve<ISettingsProvider>().ClearPrefs();
            Test.Container.Resolve<IDataStorage>().ClearDataBase();
        }

        public static ILoginController CreateController(bool networkSuccess = true, string awaitingMessage = null, string awaitingTitle = null, TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>> navigationCallback = null)
        {
            var container = Test.CreateChildContainer();
            container.RegisterInstance<ILoginNetworkProvider>(Test_LoginNetworkProvider.CreateProvider(networkSuccess));
            container.RegisterInstance<IUserDialogs>(Test.CreateMockUserDialog(awaitingMessage, awaitingTitle));
            container.RegisterInstance<ICrossPageNavigator>(Test.CreateMockNavigation(navigationCallback));
            return container.Resolve<ILoginController>();
        }

        [TestMethod]
        public async Task SuccessLogin()
        {
            var callback = new TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>>();
            var controller = CreateController(navigationCallback: callback);
            await controller.Login("RVbot", "test");
            (await callback.Task).Item1.Should().Be(PageStates.BulialListPage);
        }

        [TestMethod]
        public async Task FailWithNotValidLoginAndPassword()
        {
            try
            {
                var controller = CreateController();
                await controller.Login(string.Empty, string.Empty);
                Assert.Fail();
            }
            catch (NotValidLoginOrPasswordException ex)
            {
                ex.EmailMessage.Should().Be(Resources.Validator_EmptyEmail);
                ex.PasswordMessage.Should().Be(Resources.Validator_EmptyPassword);
            }
        }
        [TestMethod]
        public async Task FailWithIncorrctLoginAndPassword()
        {
            var controller = CreateController(awaitingMessage: Resources.Authorization_LoginFailed, awaitingTitle: Resources.Dialog_Attention);
            await controller.Login("login", "password");
        }

        [TestMethod]
        public async Task SuccessAutoLogin()
        {
            var callback = new TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>>();
            var settings = Test.Container.Resolve<ISettingsProvider>();
            settings.SessionId = "correct_session";
            var controller = CreateController(navigationCallback: callback);
            controller.TryAutoLogin();
            (await callback.Task).Item1.Should().Be(PageStates.BulialListPage);
        }

        [TestMethod]
        public void AutoLoginNotStart()
        {
            var controller = CreateController();
            controller.TryAutoLogin();
        }
    }
}
