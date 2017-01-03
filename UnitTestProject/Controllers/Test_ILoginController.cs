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

namespace UnitTestProject.Controllers
{
    [TestClass]
    public class Test_ILoginController
    {
        [TestInitialize]
        public void PrepareDatabase()
        {
            TestInitialization.Container.Resolve<ISessionSettings>().ClearPrefs();
            TestInitialization.Container.Resolve<IDataStorage>().ClearDataBase();
        }

        public static ILoginController CreateController(bool networkSuccess = true, string awaitingMessage = null, string awaitingTitle = null, PageStates? awaitingPage = null)
        {
            var container = TestInitialization.CreateChildContainer();
            container.RegisterInstance<ILoginNetworkProvider>(Test_LoginNetworkProvider.CreateProvider(networkSuccess));
            container.RegisterInstance<IUserDialog>(TestInitialization.CreateMockUserDialog(awaitingMessage, awaitingTitle));
            container.RegisterInstance<ICrossPageNavigator>(TestInitialization.CreateMockNavigation(awaitingPage));
            return container.Resolve<ILoginController>();
        }

        [TestMethod]
        public async Task SuccessLogin()
        {
            var controller = CreateController(awaitingPage: PageStates.BulialListPage);
            await controller.Login("RVbot", "test");
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
            var settings = TestInitialization.Container.Resolve<ISessionSettings>();
            settings.SessionId = "correct_session";
            var controller = CreateController(awaitingPage: PageStates.BulialListPage);
            await controller.TryAutoLogin();
        }

        [TestMethod]
        public async Task AutoLoginNotStart()
        {
            var controller = CreateController();
            await controller.TryAutoLogin();
        }
    }
}
