using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Content.PM;
using Android.Accounts;
using Android.Support.Design.Widget;
using System;
using LoaderManager = Android.Support.V4.App.LoaderManager;
using Loader = Android.Support.V4.Content.Loader;
using CursorLoader = Android.Support.V4.Content.CursorLoader;
using Android.Provider;
using Android.Content;
using Android.Database;
using System.Collections.Generic;
using System.Threading;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "NativeVyatkaAndroid", Icon = "@mipmap/icon", Theme = "@style/AppTheme", WindowSoftInputMode = SoftInput.StateAlwaysHidden, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
    public class LoginActivity : AccountAuthenticatorActivity, LoaderManager.ILoaderCallbacks
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_LoginActivity);
            FindAndBindViews();
        }

        public void FindAndBindViews()
        {
            mEmailView = FindViewById<AutoCompleteTextView>(Resource.Id.email);
            PopulateAutoComplete();
            mPasswordView = FindViewById<EditText>(Resource.Id.password);
            mPasswordView.EditorAction += delegate(object sender, TextView.EditorActionEventArgs e)
            {
                if ((int)e.ActionId == Resource.Id.login || e.ActionId == Android.Views.InputMethods.ImeAction.ImeNull)
                {
                    AttemptLogin();                       
                }                 
            };
            var mEmailSignInButton = FindViewById<Button>(Resource.Id.email_sign_in_button);
            mEmailSignInButton.Click += (sender, e) => AttemptLogin();
            mLoginFormView = FindViewById(Resource.Id.login_form);
            mProgressView = FindViewById(Resource.Id.login_progress);
            ShowProgress(mProgress);
            #if DEBUG
            mEmailView.Text = "qwe@qwe.ru";
            #endif
        }

        private void PopulateAutoComplete()
        {
            if (!MayRequestContacts())
            {
                return;
            }
            SupportLoaderManager.InitLoader(0, null, this);
        }

        private bool MayRequestContacts()
        {           
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                return true;
            }
            if (CheckSelfPermission(Android.Manifest.Permission.ReadContacts) == Permission.Granted)
            {
                return true;
            }
            if (ShouldShowRequestPermissionRationale(Android.Manifest.Permission.ReadContacts))
            {
                Snackbar.Make(mEmailView, Resource.String.permission_rationale, Snackbar.LengthIndefinite).SetAction(Android.Resource.String.Ok, obj => RequestPermissions(new[] { Android.Manifest.Permission.ReadContacts }, REQUEST_READ_CONTACTS));
            }
            else
            {
                RequestPermissions(new []{ Android.Manifest.Permission.ReadContacts }, REQUEST_READ_CONTACTS);
            }
            return false;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == REQUEST_READ_CONTACTS)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    PopulateAutoComplete();
                }
            }
        }

        private void AttemptLogin()
        {
            if (mAuthTask != null)
            {
                return;
            }
            // Reset errors.
            mEmailView.Error = null;
            mPasswordView.Error = null;

            // Store values at the time of the login attempt.
            var email = mEmailView.Text;
            var password = mPasswordView.Text;

            bool cancel = false;
            View focusView = null;

            // Check for a valid email address.
            if (string.IsNullOrEmpty(email))
            {
                mEmailView.Error = GetString(Resource.String.error_field_required);
                focusView = mEmailView;
                cancel = true;
            }
            else if (!IsEmailValid(email))
            {
                mEmailView.Error = GetString(Resource.String.error_invalid_email);
                focusView = mEmailView;
                cancel = true;
            }
            if (cancel)
            {
                // There was an error; don't attempt login and focus the first
                // form field with an error.
                focusView.RequestFocus();
            }
            else
            {
                // Show a progress spinner, and kick off a background task to
                // perform the user login attempt.
                ShowProgress(true);
                mAuthTask = new UserLoginTask(this, email, password);
                mAuthTask.Execute();
            }
        }

        private static bool IsEmailValid(String email)
        {
            return email.Contains("@");
        }

        private bool mProgress = false;

        private void ShowProgress(bool show)
        {
            mProgress = show;
            int shortAnimTime = Resources.GetInteger(Android.Resource.Integer.ConfigShortAnimTime);
            mLoginFormView.Visibility = (show ? ViewStates.Gone : ViewStates.Visible);            
            var formAnimator = new ViewAnimator();
            formAnimator.AnimationEndEvent += (sender, e) => mLoginFormView.Visibility = (show ? ViewStates.Gone : ViewStates.Visible);
            mLoginFormView.Animate().SetDuration(shortAnimTime).Alpha(show ? 0 : 1).SetListener(formAnimator);
            var progressAnimator = new ViewAnimator();
            progressAnimator.AnimationEndEvent += (sender, e) => mProgressView.Visibility = (show ? ViewStates.Visible : ViewStates.Gone);
            mProgressView.Visibility = (show ? ViewStates.Visible : ViewStates.Gone);
            mProgressView.Animate().SetDuration(shortAnimTime).Alpha(show ? 1 : 0).SetListener(progressAnimator);            
        }

        public Loader OnCreateLoader(int id, Bundle args)
        {
            return new Android.Support.V4.Content.CursorLoader(BaseContext,
                // Retrieve data rows for the device user's 'profile' contact.
                Android.Net.Uri.WithAppendedPath(ContactsContract.Profile.ContentUri, ContactsContract.Contacts.Data.ContentDirectory), ProfileQuery.PROJECTION,
                // Select only email addresses.
                ContactsContract.Contacts.Data.InterfaceConsts.Mimetype + " = ?", new []{ ContactsContract.CommonDataKinds.Email.ContentItemType },
                // Show primary email addresses first. Note that there won't be a primary email address if the user hasn't specified one.
                ContactsContract.Contacts.Data.InterfaceConsts.IsPrimary + " DESC");
        }

        public void OnLoadFinished(Loader loader, Java.Lang.Object data)
        {
            var cursor = data as ICursor;
            var emails = new List<string>();
            cursor.MoveToFirst();
            while (!cursor.IsAfterLast)
            {
                emails.Add(cursor.GetString(ProfileQuery.ADDRESS));
                cursor.MoveToNext();
            }
            AddEmailsToAutoComplete(emails);
        }

        public void OnLoaderReset(Loader loader)
        {
        }

        private static class ProfileQuery
        {
            public static string[] PROJECTION =
                {
                    ContactsContract.CommonDataKinds.Email.Address,
                    ContactsContract.CommonDataKinds.Email.InterfaceConsts.IsPrimary,
                };

            public const int ADDRESS = 0;
            public const int IS_PRIMARY = 1;
        }


        private void AddEmailsToAutoComplete(List<String> emailAddressCollection)
        {
            var adapter = new ArrayAdapter<string>(BaseContext, Android.Resource.Layout.SimpleDropDownItem1Line, emailAddressCollection);
            mEmailView.Adapter = adapter;
        }

        public class UserLoginTask : AsyncTask
        {
            private readonly string mEmail;
            private readonly string mPassword;
            private readonly LoginActivity mActivity;

            public UserLoginTask(LoginActivity activity, string email, string password)
            {
                mActivity = activity;
                mEmail = email;
                mPassword = password;
            }

            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
            {    
                Thread.Sleep(500);
                const string login = "login";
                const string password = "password";
                const string token = "qweqweqweq";
                OnTokenReceived(new AppAccount(login), password, token);
                return true;
            }

            public void OnTokenReceived(Account account, string password, string token)
            {
                var am = AccountManager.Get(Application.Context);
                var result = new Bundle();
                if (am.AddAccountExplicitly(account, password, new Bundle()))
                {
                    result.PutString(AccountManager.KeyAccountName, account.Name);
                    result.PutString(AccountManager.KeyAccountType, account.Type);
                    result.PutString(AccountManager.KeyAuthtoken, token);
                    am.SetAuthToken(account, account.Type, token);
                }
                else
                {
                    result.PutString(AccountManager.KeyErrorMessage, "account_already_exists");
                }
                ContentResolver.SetSyncAutomatically(account, SyncConstants.CONTENT_AUTHORITY, true);
                mActivity.SetAccountAuthenticatorResult(result);
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                if (mActivity!= null && !mActivity.IsFinishing)
                {
                    //mActivity.mAuthTask = null;
                    //mActivity.ShowProgress(false);
                    mActivity.SetResult(Result.Ok);
                    mActivity.Finish();
                }
            }

            protected override void OnCancelled()
            {
                mActivity.mAuthTask = null;
                mActivity.ShowProgress(false);
            }
        }

        private const int REQUEST_READ_CONTACTS = 0;
        private UserLoginTask mAuthTask = null;
        private AutoCompleteTextView mEmailView;
        private EditText mPasswordView;
        private View mProgressView;
        private View mLoginFormView;
        public const string LoginFragmentTag = "LoginFragmentTag";
    }

    public class AccountAuthenticatorActivity : AppCompatActivity
    {
        private AccountAuthenticatorResponse mAccountAuthenticatorResponse = null;
        private Bundle mResultBundle = null;

        public void SetAccountAuthenticatorResult(Bundle result)
        {
            mResultBundle = result;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mAccountAuthenticatorResponse = Intent.GetParcelableExtra(AccountManager.KeyAccountManagerResponse) as AccountAuthenticatorResponse;
            if (mAccountAuthenticatorResponse != null)
            {
                mAccountAuthenticatorResponse.OnRequestContinued();
            }
        }

        public override void Finish()
        {
            if (mAccountAuthenticatorResponse != null)
            {
                // send the result bundle back if set, otherwise send an error.
                if (mResultBundle != null)
                {
                    mAccountAuthenticatorResponse.OnResult(mResultBundle);
                }
                else
                {
                    mAccountAuthenticatorResponse.OnError(ErrorCode.Canceled, "canceled");
                }
                mAccountAuthenticatorResponse = null;
            }
            base.Finish();
        }
    }
}


