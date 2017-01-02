using Abstractions.Interfaces.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using FluentAssertions;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Models.Network.ServiceEntities;

namespace UnitTestProject.Network
{
    [TestClass]
    public class Test_LoginNetworkProvider
    {
        public const string CorrectToken = "CorrectToken";

        public static ILoginNetworkProvider CreateProvider()
        {
            var mock = new Mock<ILoginRestClient>();
            mock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(async () =>
            {
                await Task.Delay(0);
                return new ApiProfile();
            });
            mock.Setup(x => x.SiginAsync(It.IsAny<string>())).Returns(async () =>
            {
                await Task.Delay(0);
            });
            var container = TestInitialization.CreateChildContainer();
            container.RegisterType<ILoginRestClient>(new InjectionFactory(x => mock.Object));
            return container.Resolve<ILoginNetworkProvider>();
        }

        [TestInitialize]
        public void PrepareDatabase()
        {
            TestInitialization.Container.Resolve<ISettingsProvider>().ClearPrefs();
            var storage = TestInitialization.Container.Resolve<IDataStorage>();
            storage.ClearDataBase();
        }


        [TestMethod]
        public async Task ValidLogin()
        {
            var validator = TestInitialization.Container.Resolve<ISignInValidator>();
            validator.VerifyEmailAndPassword("qwe@qwe.ru", "qweq");
        }
    }
}
