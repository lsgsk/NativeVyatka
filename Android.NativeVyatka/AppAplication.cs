using System;
using Android.App;
using Android.Runtime;
using Microsoft.Practices.Unity;
using NativeVyatkaCore;

namespace NativeVyatkaAndroid
{
    [Application(Theme="@style/AppTheme")]
    public class AppApplication : Application
    {        
        public AppApplication(IntPtr handle, JniHandleOwnership transfer): base(handle, transfer)
        {
        }
        public override void OnCreate()
        {
            base.OnCreate();
            Container.RegisterInstance<IDatabase>(new BurialDatabase(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Burials.db")),new ContainerControlledLifetimeManager());
            Container.RegisterType<IBurialsManager, AppBurialsManager>();
            Container.RegisterType<ILocationManager, AppLocationManager>(new ContainerControlledLifetimeManager());
        }
        public static readonly UnityContainer Container = new UnityContainer();
    }
}

