using Android.App;
using Android.Widget;
using Android.OS;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "NativeVyatkaAndroid", MainLauncher = true, Icon = "@mipmap/icon")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Activity_Login);
            Button button = FindViewById<Button>(Resource.Id.button1);            
            button.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };
        }
    }
}


