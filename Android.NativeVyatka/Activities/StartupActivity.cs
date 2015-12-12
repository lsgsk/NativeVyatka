
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
    [Activity(Label = "StartupActivity", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]            
    public class StartupActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var am = AccountManager.Get(this);
            var account = am.GetAccountsByType(MyAccount.TYPE).FirstOrDefault();
            if (account == null)
            {
                CreateAccount();
                return;
            }
        }
    }
}

