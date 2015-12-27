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

namespace NativeVyatkaAndroid
{
	//You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public static UnityContainer Container;

        public MainApplication(IntPtr handle, JniHandleOwnership transer):base(handle, transer)
        {       
            
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            Container = new UnityContainer();           
            string dbPath = Path.Combine(System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal),"burials.db3");
            Container.RegisterInstance<IDatabase>(new BurialDatabase(dbPath));
            Container.RegisterType<IBurialsManager, AppBurialsManager>();
            Container.RegisterType<IImageFactor, ImageFactor>();  
            Container.RegisterType<IBurialEssence, BurialEssence>();
            Container.RegisterInstance<Context>(ApplicationContext);
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