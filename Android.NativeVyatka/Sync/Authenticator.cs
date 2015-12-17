using System;
using Android.OS;
using Android.Accounts;
using Android.Content;
using Java.Lang;

namespace NativeVyatkaAndroid
{
    public class Authenticator: AbstractAccountAuthenticator
    {
        // Simple constructor
        public Authenticator(Context context) : base(context)
        {
        }
        // Editing properties is not supported
        public override Bundle EditProperties(AccountAuthenticatorResponse response, string accountType)
        {
            throw new Java.Lang.UnsupportedOperationException();
        }
        // Don't add additional accounts //throws NetworkErrorException
        public override Bundle AddAccount(AccountAuthenticatorResponse response, string accountType, string authTokenType, string[] requiredFeatures, Bundle options)
        {            
            throw new UnsupportedOperationException();
        }        
        // Ignore attempts to confirm credentials
        public override Bundle ConfirmCredentials(AccountAuthenticatorResponse response, Account account, Bundle options)
        {
            //throws NetworkErrorException
            return null;
        }
        // Getting an authentication token is not supported //throws NetworkErrorException
        public override Bundle GetAuthToken(AccountAuthenticatorResponse response, Account account, string authTokenType, Bundle options)
        {           
            throw new UnsupportedOperationException();
        }
        // Getting a label for the auth token is not supported
        public override string GetAuthTokenLabel(string authTokenType)
        {
            throw new Java.Lang.UnsupportedOperationException();
        }
        // Updating user credentials is not supported
        public override Bundle UpdateCredentials(AccountAuthenticatorResponse response, Account account, string authTokenType, Bundle options)
        {
            //throws NetworkErrorException
            throw new Java.Lang.UnsupportedOperationException();
        }
        // Checking features for the account is not supported
        public override Bundle HasFeatures(AccountAuthenticatorResponse response, Account account, string[] features)
        {
            //throws NetworkErrorException
            throw new Java.Lang.UnsupportedOperationException();
        }
    }
}

