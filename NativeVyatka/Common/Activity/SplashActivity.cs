using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;

namespace NativeVyatka.Common.Activity
{
    [Activity(MainLauncher = true, Icon = "@mipmap/ic_launcher", Theme = "@style/Launcher", WindowSoftInputMode = SoftInput.StateAlwaysHidden, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            var intent = new Intent(this, typeof(ApplicationActivity));
            StartActivity(intent);
            Finish();
        }
    }
}
