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
using NativeVyatkaCore;
using Microsoft.Practices.Unity;
using Android.Support.Design.Widget;
using Android.Accounts;

namespace NativeVyatkaAndroid
{        
    public abstract class BaseAppCompatActivity : AppCompatActivity
    {       
        public static void OnRefresh(Account account)
        {
            var extras = new Bundle();
            extras.PutBoolean(ContentResolver.SyncExtrasManual, true);
            extras.PutBoolean(ContentResolver.SyncExtrasExpedited, true); 
            ContentResolver.RequestSync(account, SyncConstants.CONTENT_AUTHORITY, extras);
        }

        public void ShowSnack(string message)
        {
            Snackbar.Make(FindViewById(Resource.Id.content_frame), message, Snackbar.LengthShort).Show();
        }
    }
}

