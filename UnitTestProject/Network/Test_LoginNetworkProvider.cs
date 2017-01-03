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
using System.Threading;

namespace UnitTestProject.Network
{
    [TestClass]
    public class Test_LoginNetworkProvider
    {
        public static ApiProfile ProfileFromServer()
        {
            return new ApiProfile()
            {
                Sessid = "rVVclCwzWSNSLqL9BR-Gstvki_voqQ0M2tnZ570W3n8",
                Session_name = "SESS71c9e28fb6908726055dbf62a338405f",
                Token = "mqZNaTyKNKzQDAo5yKR5atPG8bYmrhPc50vNAzRRUyA",
                User = new ApiUser()
                {
                    uid = "99",
                    name = "RVbot",
                    mail = "zykov-ivan@rambler.ru",
                    login = 1483439541,
                    Status = "1",
                    Timezone = "Europe/Moscow",
                    Language = "",
                    Picture = new ApiPicture()
                    {
                        fid = "11114",
                        uid = "1",
                        filename = "picture-99-1444378820.png",
                        uri = "public://images/avatars/picture-99-1444378820.png",
                        url = "http://rodnaya-vyatka.ru/sites/default/files/images/avatars/picture-99-1444378820.png",
                    }
                }
            };
        }

        public static ILoginNetworkProvider CreateProvider(bool networkSuccess = true, CancellationToken token = new CancellationToken())
        {
            var mock = new Mock<ILoginRestClient>();
            mock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(async () =>
            {
                await Task.Delay(0);
                if (networkSuccess && !token.IsCancellationRequested)
                {
                    return ProfileFromServer();
                }
                else
                {
                    throw new LoginLoadException();
                }
            });
            mock.Setup(x => x.SiginAsync(It.IsAny<string>())).Returns(async () =>
            {
                await Task.Delay(0);
                if (!networkSuccess || token.IsCancellationRequested)
                {
                    throw new SigninLoadException();
                }
            });
            var container = TestInitialization.CreateChildContainer();
            container.RegisterInstance(mock.Object);
            return container.Resolve<ILoginNetworkProvider>();
        }

        [TestInitialize]
        public void PrepareDatabase()
        {
            TestInitialization.Container.Resolve<ISessionSettings>().ClearPrefs();
            TestInitialization.Container.Resolve<IDataStorage>().ClearDataBase();
        }

        [TestMethod]
        public async Task SuccessLogin()
        {
            var provider = CreateProvider();
            await provider.LoginAsync("login", "password");

            var apiProfile = ProfileFromServer();
            var settings = TestInitialization.Container.Resolve<ISessionSettings>();
            settings.CsrfToken.Should().Be(apiProfile.Token);
            settings.SessionName.Should().Be(apiProfile.Session_name);
            settings.SessionId.Should().Be(apiProfile.Sessid);
            var profile = TestInitialization.Container.Resolve<IProfileStorage>().GetProfile();
            profile.Uid.Should().Be(apiProfile.User.uid);
            profile.Name.Should().Be(apiProfile.User.name);
            profile.Email.Should().Be(apiProfile.User.mail);
            profile.PictureUrl.Should().Be(apiProfile.User.Picture.url);
        }

        [TestMethod]
        public async Task FailLogin()
        {
            try
            {
                var provider = CreateProvider(false);
                await provider.LoginAsync("login", "password");
                Assert.Fail();
            }
            catch (AuthorizationSyncException)
            {
                var settings = TestInitialization.Container.Resolve<ISessionSettings>();
                settings.CsrfToken.Should().BeEmpty();
                settings.SessionName.Should().BeEmpty();
                settings.SessionId.Should().BeEmpty();
            }
        }

        [TestMethod]
        public async Task SuccessSigin()
        {
            var provider = CreateProvider();
            await provider.SiginAsync();
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

        [TestMethod]
        public async Task TryToCancel()
        {
            try
            {
                var provider = CreateProvider(token: new CancellationToken(true));
                await provider.LoginAsync("qwe", "asd");
                Assert.Fail();
            }
            catch (AuthorizationSyncException)
            {
            }
        }
    }
}
