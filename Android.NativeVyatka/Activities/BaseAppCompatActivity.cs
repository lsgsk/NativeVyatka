
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
        public ILocationManager mLocalization
        {
            get
            {
                return AppApplication.Container.Resolve<ILocationManager>();
            }
        }

        public IBurialsManager mBurialsManager
        {
            get
            {
                return AppApplication.Container.Resolve<IBurialsManager>();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mBindConnection = new LocationServiceConnection();
            mBindConnection.OnServiceConnectedEvent += OnServiceConnected;
            mBindConnection.OnServiceDisconnectedEvent+= OnServiceDisconnected;
            mLocalization.OnGpsStatusChanged += OnGpsStatusChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mBindConnection.OnServiceConnectedEvent -= OnServiceConnected;
            mBindConnection.OnServiceDisconnectedEvent -= OnServiceDisconnected;
            mBindConnection.Dispose();
            mLocalization.OnGpsStatusChanged -= OnGpsStatusChanged;
        }

        void OnGpsStatusChanged (object sender, bool e)
        {
            var message = (e) ? "Gps доступен" : "Gps не доступен";
            Snackbar.Make(Window.DecorView, message, Snackbar.LengthShort).Show();
        }

        private void OnServiceConnected (object sender, LocationServiceBinder e)
        {
            mBounded |= e != null;
        }

        private void OnServiceDisconnected (object sender, ComponentName e)
        {
            mBounded = false;
        }

        protected override void OnStart()
        {
            base.OnStart();
            var intent = new Intent(this, typeof(LocationService));
            BindService(intent, mBindConnection, Bind.AutoCreate);
        }
        protected override void OnStop()
        {
            base.OnStop();
            if (mBounded)
            {
                mBounded = false;
                UnbindService(mBindConnection);
            }
        }
        private bool mBounded = false;
        private LocationServiceConnection mBindConnection;

        public static void OnRefresh(Account account)
        {
            var extras = new Bundle();
            //http://catinean.com/2014/08/03/force-your-syncadapter-to-sync/ to force resync
            extras.PutBoolean(ContentResolver.SyncExtrasManual, true);
            extras.PutBoolean(ContentResolver.SyncExtrasExpedited, true); 
            ContentResolver.RequestSync(account, SyncConstants.CONTENT_AUTHORITY, extras);
        }
    }
}

