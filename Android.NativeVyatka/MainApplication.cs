using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using Microsoft.Practices.Unity;
using Abstractions;
using NativeVyatkaCore;
using System.IO;
using Android.Content;
using ServiceStack;

namespace NativeVyatkaAndroid
{
    [Application(HardwareAccelerated = false)]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public static UnityContainer Container;

        public MainApplication(IntPtr handle, JniHandleOwnership transer):base(handle, transer)
        {       
            
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Licensing.RegisterLicense(@"2510-e1JlZjoyNTEwLE5hbWU6QXBwR2VhciBMVEQsVHlwZTpJbmRpZSxIYXNoOldvUXpvU3M2UGJJOTJWSXRuNkhjUzlzSDVrSnZLWkZZMjA5UDBLL20vWGFHNmUvejY2Q3pnQWRzcUdEY1VzbG5Md09MelFGd2t4L3JtN1JjRU5jM0w4RWl4cDZSSEpkdzQrWmNIZ1hwYU83Z2sxT1U2aG9SYVRKT0lRVlQ4SExjbUZIeDZ5MWd1QkpuenE1b3dvQ21oWDU2UFJmMXpXaXA1T1RZZGVzalNwUT0sRXhwaXJ5OjIwMTYtMDQtMTN9");
            RegisterActivityLifecycleCallbacks(this);
            Container = new UnityContainer();           
            string dbPath = Path.Combine(System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),"burials.db3");
            Container.RegisterInstance<IDatabase>(new BurialDatabase(dbPath));
            Container.RegisterType<IBurialsManager, AppBurialsManager>();
            Container.RegisterType<IImageFactor, ImageFactor>();  
            Container.RegisterType<IBurialEssence, BurialEssence>();
            Container.RegisterInstance<Context>(ApplicationContext);
            Container.RegisterType<IUploaderListener, UploaderListener>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IRestServiceDataProvider, RestServiceDataProvider>();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
        }
    }
}