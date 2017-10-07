using NativeVyatkaCore;
using NativeVyatkaCore.Database;
using Microsoft.Practices.Unity;
using Abstractions.Interfaces.Plugins;
using Moq;
using Abstractions.Models;
using System.Collections.Generic;
using FluentAssertions;
using System.Threading.Tasks;
using Plugin.Geolocator.Abstractions;
using System.Threading;
using System;
using Acr.UserDialogs;
using Plugin.Media.Abstractions;
using System.IO;
using Abstractions.Models.Network.ServiceEntities;

namespace UnitTestProject
{
    public static class Test
    {
        static Test()
        {
            RegisterTypesIntoDI.InitContainer(container);
            SQLitePCL.Batteries.Init();
            BurialDatabase.InitILobbyPhoneDatabase("..\\..\\Temp");
        }

        private readonly static IUnityContainer container = new UnityContainer();

        public static IUnityContainer Container
        {
            get
            {
                return container;
            }
        }

        public static IUnityContainer CreateChildContainer()
        {
            return container.CreateChildContainer();
        }

        public static ICrossPageNavigator CreateMockNavigation(TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>> navigationCallback)
        {
            var mock = new Mock<ICrossPageNavigator>();
            mock.Setup(x => x.GoToPage(It.IsAny<PageStates>(), It.IsAny<Dictionary<string, string>>())).Callback((PageStates state, Dictionary<string, string> extras) =>
            {
                navigationCallback.SetResult(new Tuple<PageStates, Dictionary<string, string>>(state, extras));
            });
            return mock.Object;
        }

        public static IUserDialogs CreateMockUserDialog(string awaitingMessage = null, string awaitingTitle = null, bool select = true)
        {
            var mock = new Mock<IUserDialogs>();
            mock.Setup(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(async (string message, string title) =>
            {
                await Task.Delay(0);
                awaitingMessage.Should().Be(message);
                awaitingTitle.Should().Be(title);
            });
            mock.Setup(x => x.DatePromptAsync(It.IsAny<DatePromptConfig>(), It.IsAny<CancellationToken?>())).Returns(Task.FromResult(new DatePromptResult(true, DateTime.UtcNow)));
            mock.Setup(x => x.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken?>())).Returns(Task.FromResult(select));
            return mock.Object;
        }

        public static IGeolocator CreateMockGeolocator(bool gpsAvailable, bool gpsTaken)
        {
            var mock = new Mock<IGeolocator>();
            mock.Setup(x => x.StartListeningAsync(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            mock.Setup(x => x.StopListeningAsync()).Returns(Task.FromResult(true));
            mock.Setup(x => x.GetPositionAsync(It.IsAny<int>(), It.IsAny<CancellationToken?>(), It.IsAny<bool>()))
                .Returns(async (int timeoutMilliseconds, CancellationToken? token, bool includeHeading) =>
                {
                    await Task.Delay(0);
                    if (gpsTaken)
                    {
                        var rd = new Random();
                        return CreateGpsPosition();
                    }
                    else
                    {
                        throw new Exception("No gps");
                    }
                });             
            mock.Setup(x => x.IsGeolocationAvailable).Returns(gpsAvailable);
            return mock.Object;
        }

        public static IMedia CreateMockMedia(bool available, bool taken)
        {
            var mock = new Mock<IMedia>();
            mock.Setup(x => x.TakePhotoAsync(It.IsAny<StoreCameraMediaOptions>())).Returns(async () =>
            {
                await Task.Delay(0);
                return (taken) ? CreatePhoto() : null;
            });            
            mock.Setup(x => x.IsCameraAvailable).Returns(available);
            mock.Setup(x => x.IsTakePhotoSupported).Returns(available);
            return mock.Object;
        }

        public static Position CreateGpsPosition()
        {
            var rd = new Random();
            return new Position()
            {
                Latitude = rd.NextDouble(),
                Longitude = rd.NextDouble(),
                Altitude = rd.NextDouble(),
                Heading = rd.NextDouble(),
            };
        }
    
        public static MediaFile CreatePhoto()
        {
            return new MediaFile("folder/newimage.png", () => new MemoryStream());
        }

        public static LoginApiProfile GetServerProfile()
        {
            return new LoginApiProfile()
            {
                sessid = "rVVclCwzWSNSLqL9BR-Gstvki_voqQ0M2tnZ570W3n8",
                session_name = "SESS71c9e28fb6908726055dbf62a338405f",
                token = "mqZNaTyKNKzQDAo5yKR5atPG8bYmrhPc50vNAzRRUyA",
                user = new LoginApiUser()
                {
                    uid = "99",
                    name = "RVbot",
                    mail = "zykov-ivan@rambler.ru",
                    login = 1483439541,
                    status = "1",
                    picture = new LoginApiPicture()
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
    }
}
