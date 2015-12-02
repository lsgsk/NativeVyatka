using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "NativeVyatkaAndroid", MainLauncher = true, Icon = "@mipmap/icon")]
    public class LoginActivity : BaseAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_LoginActivity);
            Button button = FindViewById<Button>(Resource.Id.button1);            
            button.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };
        }


        private AutoCompleteTextView mEmailView;
        private EditText mPasswordView;
        private View mProgressView;
        private View mLoginFormView;
    }
}


