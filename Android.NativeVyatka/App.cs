using System;
using Android.App;
using Android.Runtime;
using Plugin.CurrentActivity;
using Microsoft.Practices.Unity;
using NativeVyatkaCore;
using Acr.UserDialogs;
using NativeVyatkaCore.Database;
using Plugin.Geolocator;

namespace NativeVyatkaAndroid
{
    [Application(HardwareAccelerated = false)]
    public class App : Application, Application.IActivityLifecycleCallbacks
    {
        public App(IntPtr handle, JniHandleOwnership transer):base(handle, transer)
        {      
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            RegisterTypesIntoDI.InitContainer(Container);
            BurialDatabase.InitILobbyPhoneDatabase(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            UserDialogs.Init(() => CrossCurrentActivity.Current.Activity);            
        }

        

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Android.OS.Bundle savedInstanceState)
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

        public void OnActivitySaveInstanceState(Activity activity, Android.OS.Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
        }

        public static UnityContainer Container { get; } = new UnityContainer();
    }
}