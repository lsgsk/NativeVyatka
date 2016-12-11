using Abstractions;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Interfaces.Plugins;
using Abstractions.Interfaces.Settings;
using Abstractions.Interfaces.Utilities.Validators;
using Microsoft.Practices.Unity;
using NativeVyatkaAndroid;
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
            container.RegisterType<IDatabase, BurialDatabase>();
            container.RegisterType<IProfileStorage, BurialDatabase>();
            container.RegisterType<IBurialStorage, BurialDatabase>();
            //----------------------------------------------------------------------------------------
            container.RegisterType<ILoginDataProvider, LoginDataProvider > ();
            container.RegisterType<IBurialsDataProvider, BurialsDataProvider>();

            container.RegisterType<ILoginRestClient, LoginRestClient>();
            //----------------------------------------------------------------------------------------
            container.RegisterType<ISettingsProvider, SettingsProvider>();
            container.RegisterType<ISignInValidator, SignInValidator>();

            container.RegisterType<IBurialImageGuide, BurialImageGuide>();

#if ANDROID
            container.RegisterType<ICrossPageNavigator, NativeVyatkaAndroid.Utilities.PageNavigator>()
                     .RegisterType<IPageTypeImplementation, NativeVyatkaAndroid.Utilities.ActivityTypeImplementation>();
#elif UWP
#elif __IOS__
            
#elif TEST
          
#endif

            return container;
        }
    }
}
