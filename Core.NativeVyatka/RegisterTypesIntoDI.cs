using Abstractions;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Interfaces.Plugins;
using Abstractions.Interfaces.Settings;
using Abstractions.Interfaces.Utilities;
using Abstractions.Interfaces.Utilities.Validators;
using Acr.UserDialogs;
using Microsoft.Practices.Unity;
using NativeVyatkaCore.Controllers;
using NativeVyatkaCore.Database;
using NativeVyatkaCore.Network;
using NativeVyatkaCore.Network.RestClients;
using NativeVyatkaCore.Settings;
using NativeVyatkaCore.Utilities;
using NativeVyatkaCore.Utilities.SaveProviders.IoGuide;
using NativeVyatkaCore.Utilities.Validators;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
            container.RegisterType<IHttpClientFactory, HttpClientFactory>();
            container.RegisterType<ILoginNetworkProvider, LoginNetworkProvider> ();
            container.RegisterType<IBurialsNetworkProvider, BurialsNetworkProvider>();

            container.RegisterType<ILoginRestClient, LoginRestClient>()
                     .RegisterType<IBurialRestClient, BurialRestClient>();
            //----------------------------------------------------------------------------------------
            container.RegisterType<ISettingsProvider, SettingsProvider>();
            container.RegisterType<ISignInValidator, SignInValidator>();

            container.RegisterType<IApiBurialConverter, ApiBurialConverter>();
            container.RegisterType<IBurialImageGuide, BurialImageGuide>();
            container.RegisterInstance<IGeolocator>(CrossGeolocator.Current);            
            container.RegisterInstance<IMedia>(CrossMedia.Current);            
#if ANDROID
            container.RegisterType<ICrossPageNavigator, NativeVyatkaAndroid.Utilities.PageNavigator>()
                     .RegisterType<IPageTypeImplementation, NativeVyatkaAndroid.Utilities.ActivityTypeImplementation>();
            container.RegisterInstance<IUserDialogs>(UserDialogs.Instance);

#elif UWP
#elif __IOS__
            container.RegisterType<IPageNameImplementation, NativeVyatkaIOS.Utilities.ControllersTypeImplementation>()
                     //.RegisterInstance<UIKit.UIStoryboard>(NativeVyatkaIOS.AppDelegate.MainStoryboard)
                     //.RegisterInstance<UIKit.UINavigationController>(NativeVyatkaIOS.AppDelegate.NavigationController)
                     .RegisterType<ICrossPageNavigator, NativeVyatkaIOS.Utilities.PageNavigator>();
            container.RegisterInstance<IUserDialogs>(UserDialogs.Instance);
#elif WINDOWS_UWP
            container.RegisterType<ICrossPageNavigator, NativeVyatka.UWP.Utilities.PageNavigator>()
                     .RegisterType<IPageTypeImplementation, NativeVyatka.UWP.Utilities.PageTypeImplementation>();
            container.RegisterInstance<IUserDialogs>(UserDialogs.Instance);
#endif
            return container;
        }
    }
}
