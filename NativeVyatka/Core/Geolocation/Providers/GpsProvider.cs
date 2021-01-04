using System;
using Android.Content;
using Android.Runtime;
using Android.Locations;
using Android.OS;

namespace NativeVyatka
{
    public interface IGpsProvider
    {
        bool IsAvailable { get; }
        bool IsMonitoring { get; }
        void StartGpsMonitoring();
        void StopGpsMonitoring();
        event EventHandler<int> OnGpsEnableChanged;
        event EventHandler<Location> OnPositionChanged;
    }

    public class GpsProvider : GnssStatus.Callback, ILocationListener, IGpsProvider
    {
        private readonly LocationManager locManager;
        private int mLastFix = 0;
        public event EventHandler<int> OnGpsEnableChanged;
        public event EventHandler<Location> OnPositionChanged;
        public bool IsMonitoring { get; private set; }
        public bool IsAvailable { get { return locManager.IsLocationEnabled; } }

        public GpsProvider(Context context) {
            this.locManager = (LocationManager)context.GetSystemService(Context.LocationService);
        }

        public void StartGpsMonitoring() {
            this.locManager.RequestLocationUpdates(LocationManager.GpsProvider, 3000, 0, this);
            this.locManager.RegisterGnssStatusCallback(this);
            this.IsMonitoring = true;
        }

        public void StopGpsMonitoring() {
            this.locManager.RemoveUpdates(this);
            this.locManager.UnregisterGnssStatusCallback(this);
            this.IsMonitoring = false;
        }

        public override void OnSatelliteStatusChanged(GnssStatus status) {
            if (!IsMonitoring) return;
            int usedSatellites = 0;
            for (int i = 0; i < status.SatelliteCount; i++) {
                if (status.UsedInFix(i)) {
                    usedSatellites++;
                }
            }
            int send = Math.Max(usedSatellites, mLastFix);
            mLastFix = usedSatellites;//костыть устранения краткосрочных скачков
            OnGpsEnableChanged?.Invoke(this, send);
        }

        public void OnLocationChanged(Location location) {
            if (!IsMonitoring) return;
            OnPositionChanged?.Invoke(this, location);
        }

        public void OnProviderDisabled(string provider) {
        }

        public void OnProviderEnabled(string provider) {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) {
        }
    }
}