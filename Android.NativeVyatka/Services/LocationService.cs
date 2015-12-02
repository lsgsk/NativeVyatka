using System;
using Android.App;
using Android.Locations;
using NativeVyatkaCore;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using Android.Content;
using System.Collections.Generic;

namespace NativeVyatkaAndroid
{
    [Service]
    public class LocationService : Service, ILocationListener, GpsStatus.IListener
    {
        public override void OnCreate()
        {
            base.OnCreate();
            Console.WriteLine("LocationService OnCreate");
            mLocalization = AppApplication.Container.Resolve<ILocationManager>();
            try
            {
                mLocManager = GetSystemService(Context.LocationService) as LocationManager;
                if (mLocManager != null)
                {
                    mLocManager.RequestLocationUpdates(LocationManager.GpsProvider, 15000, 10, this);
                    mLocManager.AddGpsStatusListener(this);
                }
            }
            catch (Exception ex)
            {
                mLocManager = null;
                Debug.WriteLine(ex.ToString());
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Console.WriteLine("LocationService OnDestroy");
        }

        public override Android.OS.IBinder OnBind (Intent intent)
        {
            return new LocationServiceBinder (this);
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        public void ReturnLastKnowsCoordinates()
        {
            IList<String> providers = mLocManager.AllProviders;
            Location l = null;
            Location best = null; 
            for (int i = providers.Count - 1; i >= 0; i--)
            {
                l = mLocManager.GetLastKnownLocation(providers[i]); 
                if (l == null)
                    continue;
                if (best == null || best.Accuracy > l.Accuracy)
                    best = l;
            }
            if (best != null)
            {
                //SendCoordinates(l, IntentMessageFlags.SendFirstLocation);
                Console.WriteLine(l.ToString());
            }
        }

        public void OnLocationChanged(Location location)
        {
            if (mLocalization != null)
            {
                mLocalization.Location = new CrossLocation(location.Longitude, location.Latitude, location.Accuracy, location.Time);
            }
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Android.OS.Bundle extras)
        {
        }

        public void OnGpsStatusChanged(GpsEvent e)
        {
            if (e == GpsEvent.SatelliteStatus && mLocManager != null)
            {
                GpsStatus status = mLocManager.GetGpsStatus(null);
                var statusIterator = status.Satellites.Iterator();
                int fx = 0;
                while (statusIterator.HasNext)
                {
                    var sat = (GpsSatellite)statusIterator.Next();
                    if (sat.UsedInFix())
                        fx++;
                }
                if (mLocalization != null)
                {
                    mLocalization.GpsStatus = fx != 0;
                }
            }
        }

        private LocationManager mLocManager;
        private ILocationManager mLocalization;
    }

    public class LocationServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public event EventHandler<LocationServiceBinder> OnServiceConnectedEvent;
        public void OnServiceConnected (ComponentName name, Android.OS.IBinder service)
        {
            var demoServiceBinder = service as LocationServiceBinder;
            if (demoServiceBinder != null) 
            {
                if (OnServiceConnectedEvent != null)
                    OnServiceConnectedEvent(this, demoServiceBinder);
            }
        }

        public event EventHandler<ComponentName> OnServiceDisconnectedEvent;
        public void OnServiceDisconnected (ComponentName name)
        {
            if (OnServiceDisconnectedEvent != null)
                OnServiceDisconnectedEvent(this, name);
        }
    }

    public class LocationServiceBinder : Android.OS.Binder
    {
        readonly LocationService mService;

        public LocationServiceBinder (LocationService service)
        {
            this.mService = service;
        }

        public LocationService GetDemoService ()
        {
            return mService;
        }
    }
}

