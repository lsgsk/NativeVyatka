using Abstractions;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Interfaces.Plugins;
using Abstractions.Interfaces.Settings;
using Abstractions.Interfaces.Utilities.Validators;
using Microsoft.Practices.Unity;
using NativeVyatkaCore.Controllers;
using NativeVyatkaCore.Database;
using NativeVyatkaCore.Network;
using NativeVyatkaCore.Network.RestClients;
using NativeVyatkaCore.Settings;
using NativeVyatkaCore.Utilities.SaveProviders.IoGuide;
using NativeVyatkaCore.Utilities.Validators;

namespace NativeVyatkaCore
{
    public static class RegisterTypesIntoDI
    {
        public static IUnityContainer InitContainer(IUnityContainer container)
        {
            container.RegisterType<ILoginController, LoginController>()
                     .RegisterType<IMainController, MainController>()
                     .RegisterType<IBurialEditController, BurialEditController>();            
            //----------------------------------------------------------------------------------------
            container.RegisterType<IDataStorage, BurialDatabase>();
            container.RegisterType<IProfileStorage, BurialDatabase>();
            container.RegisterType<IBurialStorage, BurialDatabase>();
            //----------------------------------------------------------------------------------------
            container.RegisterType<ILoginNetworkProvider, LoginNetworkProvider > ();
            container.RegisterType<IBurialsNetworkProvider, BurialsNetworkProvider>();

            container.RegisterType<ILoginRestClient, LoginRestClient>();
            //----------------------------------------------------------------------------------------
            container.RegisterType<ISessionSettings, SessionSettings>();
            container.RegisterType<ISignInValidator, SignInValidator>();

            container.RegisterType<IBurialImageGuide, BurialImageGuide>();

#if ANDROID
            container.RegisterType<ICrossPageNavigator, NativeVyatkaAndroid.Utilities.PageNavigator>()
                     .RegisterType<IPageTypeImplementation, NativeVyatkaAndroid.Utilities.ActivityTypeImplementation>();
#elif UWP
#elif __IOS__
            container.RegisterType<IPageNameImplementation, NativeVyatkaIOS.Utilities.ControllersTypeImplementation>()
                     //.RegisterInstance<UIKit.UIStoryboard>(NativeVyatkaIOS.AppDelegate.MainStoryboard)
                     //.RegisterInstance<UIKit.UINavigationController>(NativeVyatkaIOS.AppDelegate.NavigationController)
                     .RegisterType<ICrossPageNavigator, NativeVyatkaIOS.Utilities.PageNavigator>();
#elif TEST
#endif
            return container;
        }
    }
}
