
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Content.PM;
using Android.Accounts;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "StartupActivity", MainLauncher = true, Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]            
    public class StartupActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CheckAccount();
        }
        private void CheckAccount()
        {
            var am = AccountManager.Get(this);
            var account = am.GetAccountsByType(AppAccount.TYPE).FirstOrDefault();
            if (account == null)
            {
                var intent = new Intent(this, typeof(LoginActivity));
                StartActivityForResult(intent, CREATE_ACCOUNT);
            }
            else
            {
                BaseAppCompatActivity.OnRefresh(account);
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && requestCode == CREATE_ACCOUNT)
            {
                CheckAccount();
            }
        }
        public const int CREATE_ACCOUNT = 5;
    }
}

