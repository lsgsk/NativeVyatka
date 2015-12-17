using System;
using Android.App;

namespace NativeVyatkaAndroid
{
    [Service]
    [IntentFilter(new[] { "android.accounts.AccountAuthenticator" })]
    [MetaData("android.accounts.AccountAuthenticator", Resource = "@xml/authenticator")]
    public class AuthenticatorService : Service
    {
        private Authenticator mAuthenticator;

        public override void OnCreate()
        {
            base.OnCreate();
            mAuthenticator = new Authenticator(ApplicationContext);
        }

        public override Android.OS.IBinder OnBind(Android.Content.Intent intent)
        {
            return mAuthenticator.IBinder;
        }
    }
}

