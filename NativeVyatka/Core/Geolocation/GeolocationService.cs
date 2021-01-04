using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Locations;
using AndroidX.Lifecycle;
using Java.Interop;

namespace NativeVyatka
{
    public interface IGeolocationService
    {
        bool IsGeolocationAvailable { get; }
        void Connect();
        void Disconnect();
        Task StartGpsMonitoring();
        Task<LocationState> GetPositionAsync();
        event EventHandler<GpsState> OnGpsEnableChanged;
        event EventHandler<LocationState> OnPositionChanged;
    }

    public class GeolocationService : Java.Lang.Object, IGeolocationService, ILifecycleObserver
    {
        private readonly IPermissionsProvider permissions;
        private readonly IGpsProvider gpsManager;
        private readonly ICompassProvider compassManager;

        private int mSatelites = 0;
        private double? mHeading = null;
        private double? mAccuracy = null;

        private readonly object positionSync = new object();

        public event EventHandler<GpsState> OnGpsEnableChanged;
        public event EventHandler<LocationState> OnPositionChanged;

        public bool IsGeolocationAvailable
        {
            get
            {
                return gpsManager.IsMonitoring && gpsManager.IsAvailable;
            }
        }

        public GeolocationService(IPermissionsProvider permissions, IGpsProvider gpsManager, ICompassProvider compassManager) {
            this.permissions = permissions;
            this.gpsManager = gpsManager;
            this.compassManager = compassManager;
            this.compassManager.OnDegreeChanged += OnSensorValueChanged;
            this.gpsManager.OnGpsEnableChanged += GpsEnableChanged;
            this.gpsManager.OnPositionChanged += PositionChanged;
            ProcessLifecycleOwner.Get().Lifecycle.AddObserver(this);
        }

        [Export, Lifecycle.Event.OnResume]
        public async void Connect() {
            if (gpsManager.IsMonitoring == false && await permissions.IsLocationPermissionGrantedAsync()) {
                gpsManager.StartGpsMonitoring();
                compassManager.StartCompassMonitoring();
            }
        }

        [Export, Lifecycle.Event.OnPause]
        public async void Disconnect() {
            if (gpsManager.IsMonitoring == true && await permissions.IsLocationPermissionGrantedAsync()) {
                gpsManager.StopGpsMonitoring();
                compassManager.StopCompassMonitoring();
            }
        }

        public async Task StartGpsMonitoring() {
            if (gpsManager.IsMonitoring == false) {
                if (await permissions.IsLocationPermissionGrantedAsync()) {
                    gpsManager.StartGpsMonitoring();
                    compassManager.StartCompassMonitoring();
                }
                else {
                    throw new PermissionsException();
                }
            }
        }

        private void GpsEnableChanged(object sender, int e) {
            lock (positionSync) {
                OnGpsEnableChanged?.Invoke(this, new GpsState(mSatelites = e, mAccuracy));
            }
        }

        private void PositionChanged(object sender, Location e) {
            lock (positionSync) {
                OnPositionChanged?.Invoke(this, new LocationState(e, mHeading));
                OnGpsEnableChanged?.Invoke(this, new GpsState(mSatelites, mAccuracy = e.Accuracy));
            }
        }

        private void OnSensorValueChanged(object sender, double e) {
            mHeading = e;
        }

        public async Task<LocationState> GetPositionAsync() {
            if (await permissions.IsLocationPermissionGrantedAsync() == false) {
                throw new GeolocationException();
            }
            if (gpsManager.IsMonitoring == false || gpsManager.IsAvailable == false) {
                throw new InvalidOperationException("This Geolocator is not started");
            }

            var s_cts = new CancellationTokenSource();
            s_cts.CancelAfter(TimeSpan.FromSeconds(7));
            var tcs = new TaskCompletionSource<LocationState>();
            lock (positionSync) {
                void gotPosition(object s, LocationState e) {
                    tcs.TrySetResult(e);
                    OnPositionChanged -= gotPosition;
                }
                OnPositionChanged += gotPosition;
            }
            s_cts.Token.Register(() => { tcs.TrySetCanceled(); });
            return await tcs.Task;
        }
    }

    public class GpsState : EventArgs
    {
        public int Satetiles { get; init; }
        public double? Accuracy { get; init; }

        public GpsState(int satetiles, double? accuracy) {
            this.Satetiles = satetiles;
            this.Accuracy = accuracy;
        }
    }

    public class LocationState : EventArgs
    {
        public Location Location { get; init; }
        public double? Heading { get; init; }

        public LocationState(Location location, double? heading) {
            this.Location = location;
            this.Heading = heading;
        }
    }
}