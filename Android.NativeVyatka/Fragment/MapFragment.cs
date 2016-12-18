using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Gms.Maps;
using System.Threading.Tasks;
using Android.Locations;
using Android.Gms.Maps.Model;
using Android.Graphics;
using System.Collections.Generic;
using Abstractions.Models.AppModels;
using Plugin.Geolocator;

namespace NativeVyatkaAndroid
{
    public class MapFragment : ProgressFragment, IOnMapReadyCallback
    {
        public static MapFragment NewInstance()
        {
            return new MapFragment();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContentView = inflater.Inflate(Resource.Layout.Fragment_Map, container, false);
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            carmaMap = mContentView.FindViewById<MapView>(Resource.Id.mapView);
            carmaMap.OnCreate(savedInstanceState);
            carmaMap.GetMapAsync(this);
            Refresher.Enabled = false;
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            SetContentView(mContentView);
            SetEmptyText(Resource.String.null_content);
            await ObtainData(savedInstanceState == null);
        }

        protected async Task ObtainData(bool force = false)
        {
            try
            {
                SetContentShown(false);

                //****

                SetContentEmpty(false);
                SetContentShown(true);
            }
            catch
            {
                ShowErrorAction();
            }
        }

        private void ShowErrorAction()
        {
            SetContentEmpty(true);
            SetContentShown(true);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            carmaGoogleMap = googleMap;
            carmaGoogleMap.MyLocationEnabled = true;
            moveMapToMyLocation();
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
                SetMarkers();
            }
        }

        public async Task UpdatePoints()
        {
            await SetMarkers();
        }

        private async Task SetMarkers()
        {
            carmaGoogleMap.Clear();
            var position = await CrossGeolocator.Current.GetPositionAsync(10000);
            var collection = new List<BurialModel>()
            {
                new BurialModel()
                {
                    Name = "Игнатий",
                    Location = new BurialModel.Position()
                    {
                        Latitude = position.Latitude,
                        Longitude = position.Longitude
                    }
                }
            };     
            foreach (var item in collection)
            {
                var marker = new MarkerOptions();
                marker.SetPosition(new LatLng(item.Location.Latitude, item.Location.Longitude));
                marker.SetTitle(item.Name);
                carmaGoogleMap.AddMarker(marker); 
            }
        }

        private static Bitmap GetCircle()
        {
            var areaRect = new RectF(0, 0, 70, 70);
            var circle = Bitmap.CreateBitmap((int)areaRect.Width(), (int)areaRect.Height(), Bitmap.Config.Argb4444);                
            var canvas = new Canvas(circle);
            var paint = new Paint();
            paint.AntiAlias = true;
            paint.Color = Color.LightBlue;
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawRoundRect(areaRect, 70, 70, paint);
            paint.Color = Color.White;
            canvas.DrawRoundRect(new RectF(10, 10, 60, 60), 50, 50, paint);
            paint.Color = Color.LightBlue;
            canvas.DrawRoundRect(new RectF(20, 20, 50, 50), 30, 30, paint);  
            return circle;            
        }

        public override void OnResume()
        {
            base.OnResume();
            carmaMap.OnResume();
            if (carmaGoogleMap != null)
            {
                carmaGoogleMap.MyLocationEnabled = true;
                carmaGoogleMap.MarkerClick += MapClick;    
            }
        }

        void MapClick (object sender, GoogleMap.MarkerClickEventArgs e)
        {
            var areaRect = new RectF(0, 0, 100, 100);
            var circle = Bitmap.CreateBitmap((int)areaRect.Width(), (int)areaRect.Height(), Bitmap.Config.Argb4444);                
            var canvas = new Canvas(circle);
            var paint = new Paint();
            paint.AntiAlias = true;
            paint.Color = Color.Red;
            paint.SetStyle(Paint.Style.Fill);
            canvas.DrawRoundRect(areaRect, 100, 100, paint);
            canvas.DrawBitmap(BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.Icon), 0,0, null);


            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(e.Marker.Position.Latitude + 0.001, e.Marker.Position.Longitude + 0.001));
            marker.SetIcon(BitmapDescriptorFactory.FromBitmap(circle));
            carmaGoogleMap.AddMarker(marker); 

        }

        public override void OnPause()
        {
            base.OnPause();
            carmaMap.OnPause();
            if (carmaGoogleMap != null)
            {
                carmaGoogleMap.MyLocationEnabled = false;
                carmaGoogleMap.MarkerClick-= MapClick;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            carmaMap.OnDestroy();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            carmaMap.OnLowMemory();
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {          
            base.OnPrepareOptionsMenu(menu);
        }

        private MapView carmaMap;
        private GoogleMap carmaGoogleMap;
        protected View mContentView;
        public const string MapFragmentTag = "MapFragmentTag";
    }
}

