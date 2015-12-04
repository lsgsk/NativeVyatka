using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "NativeVyatkaAndroid", MainLauncher = true, Icon = "@mipmap/icon", WindowSoftInputMode = SoftInput.StateAlwaysHidden)]
    public class LoginActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_FrameActivity);
            if (SupportFragmentManager.FindFragmentByTag(LoginFragment.LoginFragmentTag) == null)
            {
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, LoginFragment.NewInstance(), LoginFragment.LoginFragmentTag).Commit();
            }
        }
    }
}


