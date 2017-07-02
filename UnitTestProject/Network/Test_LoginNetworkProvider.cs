using Abstractions.Interfaces.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using FluentAssertions;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Models.Network.ServiceEntities;
using Abstractions.Interfaces.Settings;
using Abstractions;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Database.Tables;
using System;

namespace UnitTestProject.Network
{
    [TestClass]
    public class Test_LoginNetworkProvider
    {
        public static ILoginNetworkProvider CreateProvider(bool success = true)
        {
            var mock = new Mock<ILoginRestClient>();
            Func<Task<ApiProfile>> action = () =>
                {
                    if (success)
                    {
                        return Task.FromResult(Test.GetServerProfile());
                    }
                    else
                    {
                        throw new LoginLoadException();
                    }
                };
            mock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(action);
            mock.Setup(x => x.SiginAsync()).Returns(action);
            var container = Test.CreateChildContainer();
            container.RegisterInstance(mock.Object);
            return container.Resolve<ILoginNetworkProvider>();
        }
        private ISettingsProvider settings;
        private IProfileStorage storage;

        [TestInitialize]
        public void PrepareDatabase()
        {
            settings = Test.Container.Resolve<ISettingsProvider>();
            Test.Container.Resolve<IDataStorage>().ClearDataBase();
            storage = Test.Container.Resolve<IProfileStorage>();
            settings.ClearPrefs();
        }

        [TestMethod]
        public async Task LoginAsync_SuccessLogin()
        {
            var provider = CreateProvider();
            await provider.LoginAsync("RVbot", "test");

            var apiProfile = Test.GetServerProfile();
            settings.CsrfToken.Should().Be(apiProfile.token);
            settings.SessionName.Should().Be(apiProfile.session_name);
            settings.SessionId.Should().Be(apiProfile.sessid);

            var profile = storage.GetProfile();
            profile.Uid.Should().Be(apiProfile.user.uid);
            profile.Name.Should().Be(apiProfile.user.name);
            profile.Email.Should().Be(apiProfile.user.mail);
            profile.PictureUrl.Should().Be(apiProfile.user.picture.url);
        }

        [TestMethod]
        public async Task LoginAsync_FailLogin()
        {
            try
            {
                var provider = CreateProvider(false);
                await provider.LoginAsync("login", "password");
                Assert.Fail();
            }
            catch (AuthorizationSyncException)
            {
                settings.CsrfToken.Should().BeEmpty();
                settings.SessionName.Should().BeEmpty();
                settings.SessionId.Should().BeEmpty();
            }
        }

        [TestMethod]
        public async Task SiginAsync_SuccessSigin()
        {
            var provider = CreateProvider();
            await provider.SiginAsync();

            var apiProfile = Test.GetServerProfile();
            settings.CsrfToken.Should().Be(apiProfile.token);
            settings.SessionName.Should().Be(apiProfile.session_name);
            settings.SessionId.Should().Be(apiProfile.sessid);

            var profile = storage.GetProfile();
            profile.Uid.Should().Be(apiProfile.user.uid);
            profile.Name.Should().Be(apiProfile.user.name);
            profile.Email.Should().Be(apiProfile.user.mail);
            profile.PictureUrl.Should().Be(apiProfile.user.picture.url);
        }

        [TestMethod]
        public async Task FailedSigin()
        {
            try
            {
                var provider = CreateProvider(false);
                await provider.SiginAsync();
                Assert.Fail();
            }
            catch (AuthorizationSyncException)
            {
            }
        }
    }
}
