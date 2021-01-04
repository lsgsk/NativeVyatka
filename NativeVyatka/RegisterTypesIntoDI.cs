using Acr.UserDialogs;
using Unity;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Unity.Lifetime;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Android.Content;
using Android.App;

namespace NativeVyatka
{
    public static class RegisterTypesIntoDI
    {
        public static IUnityContainer InitContainer(IUnityContainer container) {
            container.RegisterType<ILoginPresenter, LoginPresenter>()
                     .RegisterType<IMainPresenter, MainPresenter>()
                     .RegisterType<IBurialEditPresenter, BurialEditPresenter>();
            //----------------------------------------------------------------------------------------
            container.RegisterType<IDataStorage, BurialDatabase>();
            container.RegisterType<IProfileStorage, BurialDatabase>();
            container.RegisterType<IBurialStorage, BurialDatabase>();
            //----------------------------------------------------------------------------------------
            container.RegisterType<IHttpClientFactory, HttpClientFactory>();
            container.RegisterType<ILoginNetworkProvider, LoginNetworkProvider>();
            container.RegisterType<IBurialsNetworkProvider, BurialsNetworkProvider>();

            container.RegisterType<ILoginRestClient, LoginRestClient>()
                     .RegisterType<IBurialRestClient, BurialRestClient>();
            //----------------------------------------------------------------------------------------
            container.RegisterType<ISettingsProvider, SettingsProvider>()
                     .RegisterType<ISignInValidator, SignInValidator>();

            container.RegisterType<IApiBurialConverter, ApiBurialConverter>()
                     .RegisterType<IImageSizeConverter, ImageSizeConverter>();

            container.RegisterType<IBurialImageGuide, BurialImageGuide>();
            container.RegisterInstance<IMedia>(CrossMedia.Current);

            container.RegisterType<IPermissionsProvider, PermissionsProvider>()
                     .RegisterInstance<IPermissions>(CrossPermissions.Current);

            container.RegisterType<IGpsProvider, GpsProvider>(new ContainerControlledLifetimeManager())
                     .RegisterType<ICompassProvider, CompassProvider>(new ContainerControlledLifetimeManager())
                     .RegisterType<IGeolocationService, GeolocationService>();

            container.RegisterType<IRouter, Router>()
                     .RegisterInstance<IUserDialogs>(UserDialogs.Instance)
                     .RegisterType<IMd5HashGenerator, Md5HashGenerator>()
                     .RegisterInstance<Context>(Application.Context);

            return container;
        }
    }
}
