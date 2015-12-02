using System;

namespace NativeVyatkaCore
{
    public class AppLocationManager : ILocationManager
    {
        public AppLocationManager()
        {            
        }

        public event EventHandler<CrossLocationEventArgs> OnLocationChanged;
        public event EventHandler<bool> OnGpsStatusChanged;

        private bool gpsStatus = false;
        public bool GpsStatus
        {
            get
            {
                return gpsStatus;
            }
            set
            {
                if (gpsStatus != value)
                {
                    gpsStatus = value;
                    if (OnGpsStatusChanged != null)
                    {
                        OnGpsStatusChanged(this, value);
                    }
                }
            }
        }
        private CrossLocation location = null;
        public CrossLocation Location
        {
            get
            {
                return location ?? new CrossLocation(0, 0, 0, DateTime.UtcNow.ToBinary());
            }
            set
            {
                location = value;
                if (OnLocationChanged != null)
                {
                    OnLocationChanged(this, new CrossLocationEventArgs(value));
                }
            }
        }       

        public void Dispose()
        {
            gpsStatus = false;
            location = null;
        }
    }
}

