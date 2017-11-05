using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Locations;
using Abstractions.Interfaces.Utilities;

namespace NativeVyatkaAndroid.Utilities
{  
#pragma warning disable CS0618 // Type or member is obsolete
    public class GpsSatelliteManager : Java.Lang.Object, GpsStatus.IListener, IGpsSatelliteManager
    {
        public GpsSatelliteManager()
        {
            mLocManager = (LocationManager)Application.Context.GetSystemService(Context.LocationService);
            mLocManager.AddGpsStatusListener(this);
        }

        public void OnGpsStatusChanged([GeneratedEnum] GpsEvent e)
        {
            if (e == GpsEvent.SatelliteStatus)
            {
                var status = mLocManager.GetGpsStatus(null);
                var statusIterator = status.Satellites.Iterator();
                int catched = 0;
                while (statusIterator.HasNext)
                {
                    var sat = (GpsSatellite)statusIterator.Next();
                    if (sat.UsedInFix())
                    {
                        catched++;
                    }
                }
                int send = Math.Max(catched, mLastFix);
                mLastFix = catched;//костыть устранения краткосрочных скачков
                OnGpsEnableChanged?.Invoke(this, send);
            }
        }

        private int mLastFix = 0;
        private readonly LocationManager mLocManager;
        public event EventHandler<int> OnGpsEnableChanged;
    }
#pragma warning restore CS0618 // Type or member is obsolete
}