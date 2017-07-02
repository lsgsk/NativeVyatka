using System;
using Android.App;
using Android.Content.PM;
using Android.Widget;
using Java.Interop;
using Android.Views;
using Microsoft.Practices.Unity;
using Android.Content;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Plugins;
using Newtonsoft.Json;
using Abstractions.Models.AppModels;
using NativeVyatkaCore.Utilities;
using Square.Picasso;

namespace NativeVyatkaAndroid
{
    [Activity(Theme = "@style/AppTheme", Label = "@string/burial_edit_title", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]
    public class BurialEditActivity : BaseAppCompatActivity, IOnMapReadyCallback
    {
        public BurialEditActivity()
        {
            mController = App.Container.Resolve<IBurialEditController>();
            mController.BurialUpdated += OnBurialUpdated;
        }       

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FindAndBindViews(savedInstanceState);            
            try
            {
                mController.Burial = JsonConvert.DeserializeObject<BurialModel>(Intent.GetStringExtra(FormBundleConstants.BurialModel));
                OnDisplayBurial(mController.Burial);
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                Toast.MakeText(this, NativeVyatkaCore.Properties.Resources.EditScreen_OpeningFailed, ToastLength.Short).Show();
                Finish();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            carmaMap?.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            carmaMap?.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mController.Dispose();
            carmaMap?.OnDestroy();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            carmaMap?.OnLowMemory();
        }

        private void OnDisplayBurial(BurialModel burial)
        {
            Picasso.With(BaseContext).Load(new Java.IO.File(burial.PicturePath)).ResizeDimen(Resource.Dimension.photo_size, Resource.Dimension.photo_size).CenterInside().OnlyScaleDown().Into(imgPhoto);
            SupportActionBar.Title = $"{burial.Name} {burial.Surname} {burial.Patronymic}";
            etName.Text = burial.Name;
            etSurname.Text = burial.Surname;
            etPatronymic.Text = burial.Patronymic;
            etDescription.Text = burial.Description;
            DisplayDate(etPhotoTime, burial.RecordTime);
            DisplayDate(etBirthTime, burial.BirthDay);
            DisplayDate(etDeathTime, burial.DeathDay);
            carmaMap.GetMapAsync(this);
        }

        private void DisplayDate(EditText et, DateTime? time)
        {
            et.Text = time.HasValue ? time.Value.ToShortDateString() : GetString(Resource.String.desciption_unknown);
        }

        private void FindAndBindViews(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Layout_BurialEditDetailActivity);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            imgPhoto = FindViewById<ImageView>(Resource.Id.imgPhoto);
            etName = FindViewById<EditText>(Resource.Id.etName);
            etName.TextChanged += (s, e) =>
            {
                mController.Burial.Name = e.Text.ToString();
                BurialNeedToBeUpdated(etName);
            };
            etSurname = FindViewById<EditText>(Resource.Id.etSurname);
            etSurname.TextChanged += (s, e) =>
            {
                mController.Burial.Surname = e.Text.ToString();
                BurialNeedToBeUpdated(etSurname);
            };
            etPatronymic = FindViewById<EditText>(Resource.Id.etPatronymic);
            etPatronymic.TextChanged += (s, e) =>
            {
                mController.Burial.Patronymic = e.Text.ToString();
                BurialNeedToBeUpdated(etPatronymic);
            };
            etDescription = FindViewById<EditText>(Resource.Id.etDescription);
            etDescription.TextChanged += (s, e) =>
            {
                mController.Burial.Description = e.Text.ToString();
                BurialNeedToBeUpdated(etDescription);
            };
            etPhotoTime = FindViewById<EditText>(Resource.Id.etPhotoTime);
            etBirthTime = FindViewById<EditText>(Resource.Id.etBirthTime);
            etDeathTime = FindViewById<EditText>(Resource.Id.etDeathTime);
            carmaMap = FindViewById<MapView>(Resource.Id.mapView);
            carmaMap.OnCreate(savedInstanceState);
        }

        private void BurialNeedToBeUpdated(EditText view)
        {
            if (view.Tag == null)
            {
                view.Tag = true;
            }
            else
            {
                mController.Updated = true;
            }
        }

        public async void OnRetakePhoto()
        {
            var path = await mController.RetakePhotoAsync();
            Picasso.With(BaseContext).Load(new Java.IO.File(path)).Into(imgPhoto);
        }

        [Export("OnSetBirthTime")]
        public async void OnSetBirthTime(View view)
        {
            DisplayDate(etBirthTime, await mController.SetBirthTimeAsync());
        }

        [Export("OnSetDeathTime")]
        public async void OnSetDeathTime(View view)
        {
            DisplayDate(etDeathTime, await mController.SetDeathTimeAsync());
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_edit_bar, menu);
            mSaveIcon = menu.FindItem(Resource.Id.action_save);
            mSaveIcon.SetVisible(mController.Updated);
            menu.FindItem(Resource.Id.action_delete).SetVisible(!mController.Creating);
            return base.OnCreateOptionsMenu(menu);
        }

        private void OnBurialUpdated(object sender, bool e)
        {
            mSaveIcon?.SetVisible(e);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mController.SaveAndUploadBurialAndGoBackAsync();
                    return true;
                case Resource.Id.action_save:
                    mController.SaveAndUploadBurialAsync();
                    break;
                case Resource.Id.action_delete:
                    mController.DeleteRecordAsync();
                    break;
                case Resource.Id.action_rephoto:
                    OnRetakePhoto();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public async override void OnBackPressed()
        {
            await mController.SaveAndUploadBurialAndGoBackAsync();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var item = mController.Burial;
            if (googleMap != null && item != BurialModel.Null)
            {
                carmaMap?.OnResume();
                var position = new LatLng(item.Location.Latitude, item.Location.Longitude);
                var camPos = new CameraPosition.Builder().Target(position).Zoom(15f).Build();
                var camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);
                googleMap.MoveCamera(camUpdate);
                googleMap.UiSettings.ScrollGesturesEnabled = false;

                var marker = new MarkerOptions();
                marker.SetPosition(position);
                marker.SetTitle(item.Name);
                googleMap.AddMarker(marker); 
            }
        }


        private readonly IBurialEditController mController;
        private ImageView imgPhoto;
        private EditText etName, etSurname, etPatronymic, etDescription, etPhotoTime, etBirthTime, etDeathTime;
        private MapView carmaMap;
        private IMenuItem mSaveIcon;
    }
}

