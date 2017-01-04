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

namespace UnitTestProject.Controllers
{
    [TestClass]
    public class Test_IBurialEditController
    {
        public static IBurialEditController CreateController(bool network = true, string message = null, string title = null, TaskCompletionSource<Tuple<PageStates, Dictionary<string, string>>> navigationCallback = null, bool gps = true, bool gpsTaken = true, bool camera = true, bool photoTaken = true)
        {
            var container = TestInitialization.CreateChildContainer();
            container.RegisterInstance<IBurialsNetworkProvider>(Test_IBurialsNetworkProvider.CreateProvider(network));
            container.RegisterInstance<IUserDialogs>(TestInitialization.CreateMockUserDialog(message, title));
            container.RegisterInstance<ICrossPageNavigator>(TestInitialization.CreateMockNavigation(navigationCallback));
            container.RegisterInstance<IMedia>(TestInitialization.CreateMockMedia(camera, photoTaken));
            return container.Resolve<IBurialEditController>();
        }

        [TestInitialize]
        public void PrepareDatabase()
        {
            TestInitialization.Container.Resolve<ISessionSettings>().ClearPrefs();
            TestInitialization.Container.Resolve<IDataStorage>().ClearDataBase();
        }

        [TestMethod]
        public async Task qwe()
        {
        }
    }
}
