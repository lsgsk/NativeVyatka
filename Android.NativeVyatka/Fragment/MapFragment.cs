using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Gms.Maps;
using Android.Locations;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Abstractions.Interfaces.Controllers;
using System.Linq;

namespace NativeVyatkaAndroid
{
    public class MapFragment : ProgressFragment, IOnMapReadyCallback
    {
        public static MapFragment NewInstance()
        {
            return new MapFragment();
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            mController = (context as MainActivity).mController;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContentView = inflater.Inflate(Resource.Layout.Fragment_Map, container, false);
            return base.OnCreateView(inflater, container, savedInstanceState);
        }        

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            carmaMap = mContentView.FindViewById<MapView>(Resource.Id.mapView);
            carmaMap.OnCreate(savedInstanceState);
            carmaMap.GetMapAsync(this);
            Refresher.Enabled = false;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            SetContentView(mContentView);
            SetEmptyText(Resource.String.null_content);
            SetContentEmpty(false);
            SetContentShown(true);
            DisplayMarkers();
        }

        protected void DisplayMarkers()
        {
            if (carmaGoogleMap != null)
            {
                carmaGoogleMap.Clear();
                var collection = mController.GetBurials();
                foreach (var item in collection)
                {
                    var marker = new MarkerOptions();
                    marker.SetPosition(new LatLng(item.Location.Latitude, item.Location.Longitude));
                    marker.SetTitle(item.Name);
                    var mrk = carmaGoogleMap.AddMarker(marker);
                    mrk.Tag = item.CloudId;
                }
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            carmaGoogleMap = googleMap;
            carmaGoogleMap.MyLocationEnabled = true;
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            carmaGoogleMap.MarkerClick += MapClick;
            moveMapToMyLocation();
            DisplayMarkers();
        }

        private void moveMapToMyLocation()
        {
            if (carmaGoogleMap != null)
            {
                var locMan = Activity.GetSystemService(Context.LocationService) as LocationManager;
                var crit = new Criteria();
                Location loc = locMan.GetLastKnownLocation(locMan.GetBestProvider(crit, false));
                if (loc != null)
                {
                    CameraPosition camPos = new CameraPosition.Builder().Target(new LatLng(loc.Latitude, loc.Longitude)).Zoom(15f).Build();
                    CameraUpdate camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);
                    carmaGoogleMap.MoveCamera(camUpdate);
                }
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            carmaMap?.OnResume();
        }

        private void MapClick (object sender, GoogleMap.MarkerClickEventArgs e)
        {
            var item = mController.GetBurials().FirstOrDefault(x => x.CloudId == e.Marker.Tag.ToString());
            if(item != null)
            {
                mController.DisplayBurial(item);
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            carmaMap?.OnPause();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            carmaMap?.OnDestroy();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            carmaMap?.OnLowMemory();
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {          
            base.OnPrepareOptionsMenu(menu);
        }

        private IMainMapController mController;
        private MapView carmaMap;
        private GoogleMap carmaGoogleMap;
        protected View mContentView;
        public const string MapFragmentTag = "MapFragmentTag";
    }
}

