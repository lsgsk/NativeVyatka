using NativeVyatkaCore;
using NativeVyatkaCore.Database;
using Microsoft.Practices.Unity;
using Abstractions.Interfaces.Plugins;
using Moq;
using Abstractions.Models;
using System.Collections.Generic;
using FluentAssertions;
using System.Threading.Tasks;

namespace UnitTestProject
{
    public static class TestInitialization
    {
        static TestInitialization()
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

        public static ICrossPageNavigator CreateMockNavigation(PageStates? awaitingPage = null)
        {
            var mockNavigation = new Mock<ICrossPageNavigator>();
            mockNavigation.Setup(x => x.GoToPage(It.IsAny<PageStates>(), It.IsAny<Dictionary<string, string>>())).Callback((PageStates state, Dictionary<string, string> extras) =>
            {
                state.Should().Be(awaitingPage);
            });
            return mockNavigation.Object;
        }

        public static IUserDialog CreateMockUserDialog(string awaitingMessage = null, string awaitingTitle = null)
        {
            var mockDialog = new Mock<IUserDialog>();
            mockDialog.Setup(x => x.AlertAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(async (string message, string title) =>
            {
                await Task.Delay(0);
                awaitingMessage.Should().Be(message);
                awaitingTitle.Should().Be(title);
            });
            return mockDialog.Object;
        }
    }
}
