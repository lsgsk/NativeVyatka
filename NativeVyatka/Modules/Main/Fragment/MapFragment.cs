using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Views;

namespace NativeVyatka
{
    public class MapFragment : ProgressFragment, IOnMapReadyCallback, IMapObserver
    {
        private readonly IMainMapPresenter presenter;
        private MapView carmaMap;
        private GoogleMap carmaGoogleMap;
        protected View mContentView;

        public static MapFragment NewInstance(IMainMapPresenter presenter) {
            return new MapFragment(presenter) {
                RetainInstance = true
            };
        }

        public MapFragment(IMainMapPresenter presenter) {
            this.presenter = presenter;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            mContentView = inflater.Inflate(Resource.Layout.Fragment_Map, container, false);
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);
            carmaMap = mContentView.FindViewById<MapView>(Resource.Id.mapView);
            carmaMap.OnCreate(savedInstanceState);
            carmaMap.GetMapAsync(this);
            Refresher.Enabled = false;
            presenter.AddObserver(this);
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {
            base.OnActivityCreated(savedInstanceState);
            SetContentView(mContentView);
            SetContentEmpty(false);
            SetContentShown(true);
        }

        public void OnMapReady(GoogleMap googleMap) {
            carmaGoogleMap = googleMap;
            carmaGoogleMap.MyLocationEnabled = true;
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            carmaGoogleMap.MarkerClick += MapClick;
            MoveMapToMyLocation();
            presenter.DisplayBurialsOnMap();
        }

        private void MoveMapToMyLocation() {
            if (carmaGoogleMap != null) {
                var locMan = Activity.GetSystemService(Context.LocationService) as LocationManager;
                var crit = new Criteria();
                Location loc = locMan.GetLastKnownLocation(locMan.GetBestProvider(crit, false));
                if (loc != null) {
                    CameraPosition camPos = new CameraPosition.Builder().Target(new LatLng(loc.Latitude, loc.Longitude)).Zoom(15f).Build();
                    CameraUpdate camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);
                    carmaGoogleMap.MoveCamera(camUpdate);
                }
            }
        }

        public override void OnResume() {
            base.OnResume();
            carmaMap?.OnResume();
        }

        private void MapClick(object sender, GoogleMap.MarkerClickEventArgs e) {
            //var item = presenter.GetBurials().FirstOrDefault(x => x.CloudId == e.Marker.Tag.ToString());
            //if (item != null) {
            //    presenter.DisplayBurial(item);
            //}
        }

        public override void OnPause() {
            base.OnPause();
            carmaMap?.OnPause();
        }

        public override void OnDestroy() {
            base.OnDestroy();
            carmaMap?.OnDestroy();
        }

        public override void OnLowMemory() {
            base.OnLowMemory();
            carmaMap?.OnLowMemory();
        }

        public void OnRecordsLoaded(List<BurialModel> burials) {
            if (carmaGoogleMap != null) {
                carmaGoogleMap.Clear();
                foreach (var item in burials) {
                    var marker = new MarkerOptions();
                    marker.SetPosition(new LatLng(item.Location.Latitude, item.Location.Longitude));
                    marker.SetTitle(item.Name);
                    var mrk = carmaGoogleMap.AddMarker(marker);
                    mrk.Tag = item.CloudId;
                }
            }
        }
    }
}

