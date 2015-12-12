using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Content.PM;
using Android.Provider;
using Android.Database;
using System.Threading;
using Fragment = Android.Support.V4.App.Fragment;
using LoaderManager = Android.Support.V4.App.LoaderManager;
using Loader = Android.Support.V4.Content.Loader;
using CursorLoader = Android.Support.V4.Content.CursorLoader;
using Android.Locations;

namespace NativeVyatkaAndroid
{
    public class LoginFragment : Fragment, LoaderManager.ILoaderCallbacks
    {
        public static LoginFragment NewInstance()
        {
            return new LoginFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Fragment_Login, container, false);
            // Set up the login form.
            mEmailView = (AutoCompleteTextView)view.FindViewById(Resource.Id.email);
            PopulateAutoComplete();
            mPasswordView = (EditText)view.FindViewById(Resource.Id.password);
            mPasswordView.EditorAction += delegate(object sender, TextView.EditorActionEventArgs e)
            {
                if ((int)e.ActionId == Resource.Id.login || e.ActionId == Android.Views.InputMethods.ImeAction.ImeNull)
                {
                    AttemptLogin();                       
                }                 
            };
            var mEmailSignInButton = (Button)view.FindViewById(Resource.Id.email_sign_in_button);
            mEmailSignInButton.Click += (sender, e) => AttemptLogin();
            mLoginFormView = view.FindViewById(Resource.Id.login_form);
            mProgressView = view.FindViewById(Resource.Id.login_progress);
            ShowProgress(mProgress);
            #if DEBUG
            mEmailView.Text = "qwe@qwe.ru";
            #endif
            return view;
        }

        private void PopulateAutoComplete()
        {
            if (!MayRequestContacts())
            {
                return;
            }
            Activity.SupportLoaderManager.InitLoader(0, null, this);
        }

        private bool MayRequestContacts()
        {           
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                return true;
            }
            if (Activity.CheckSelfPermission(Android.Manifest.Permission.ReadContacts) == Permission.Granted)
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
            var location = new Location();

            FetchAddressIntentService.StartFetchAddressIntentService(

            return;


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
            return new Android.Support.V4.Content.CursorLoader(Context,
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
            var adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleDropDownItem1Line, emailAddressCollection);
            mEmailView.Adapter = adapter;
        }

        public class UserLoginTask : AsyncTask
        {
            private readonly string mEmail;
            private readonly string mPassword;
            private readonly LoginFragment mFragment;

            public UserLoginTask(LoginFragment fragment, string email, string password)
            {
                mFragment = fragment;
                mEmail = email;
                mPassword = password;
            }

            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] native_parms)
            {    
                Thread.Sleep(500);
                        //ContentResolver.SetSyncAutomatically(sAccount, AUTHORITY, true);
                return true;
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
                if (mFragment.Activity != null &&  !mFragment.Activity.IsFinishing)
                {
                    mFragment.mAuthTask = null;
                    mFragment.ShowProgress(false);
                    if ((bool)result)
                    {                    
                        mFragment.StartActivity(new Android.Content.Intent(mFragment.Activity, typeof(MainActivity)));
                        mFragment.Activity.Finish();                    
                    }
                    else
                    {
                        mFragment.mPasswordView.Error = (mFragment.GetString(Resource.String.error_incorrect_password));
                        mFragment.mPasswordView.RequestFocus();
                    }
                }
            }

            protected override void OnCancelled()
            {
                mFragment.mAuthTask = null;
                mFragment.ShowProgress(false);
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
}

