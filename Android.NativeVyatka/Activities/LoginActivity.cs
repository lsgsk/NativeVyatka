using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content.PM;
using Android.Accounts;
using Android.Support.Design.Widget;
using Abstractions.Interfaces.Controllers;
using Microsoft.Practices.Unity;
using Java.Interop;
using Abstractions.Exceptions;

namespace NativeVyatkaAndroid
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme", WindowSoftInputMode = SoftInput.StateAlwaysHidden, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
    public class LoginActivity : AccountAuthenticatorActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);            
            FindAndBindViews();
            mController = App.Container.Resolve<ILoginController>();
            mController.TryAutoLogin();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mController.Dispose();
        }

        public void FindAndBindViews()
        {
            SetContentView(Resource.Layout.Layout_LoginActivity);
            tvEmailView = FindViewById<TextInputEditText>(Resource.Id.tvEmail);
            tvPasswordView = FindViewById<TextInputEditText>(Resource.Id.tvPassword);
            tvPasswordView.EditorAction += delegate(object sender, TextView.EditorActionEventArgs e)
            {
                if ((int)e.ActionId == Resource.Id.login || e.ActionId == Android.Views.InputMethods.ImeAction.ImeNull)
                {
                    OnSignInClick(null);                    
                }                 
            };
#if DEBUG
            tvEmailView.Text = "Лысов Александр";
            tvPasswordView.Text = "ui98oVaN";
#endif
        }

        [Export("OnSignInClick")]
        public async void OnSignInClick(View view)
        {
            try
            {
                tvEmailView.Error = tvPasswordView.Error = null;
                await mController.Login(tvEmailView.Text, tvPasswordView.Text);
            }
            catch(NotValidLoginOrPasswordException ex)
            {
                if(!string.IsNullOrEmpty(ex.EmailMessage))
                {
                    tvEmailView.Error = ex.EmailMessage;
                    tvEmailView.RequestFocus();
                }
                if (!string.IsNullOrEmpty(ex.PasswordMessage))
                {
                    tvPasswordView.Error = ex.EmailMessage;
                    tvPasswordView.RequestFocus();
                }
            }
        }
        public ILoginController mController;
        private TextInputEditText tvEmailView;
        private TextInputEditText tvPasswordView;
    }
}


