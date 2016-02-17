using System;
using Android.App;
using Android.Content.PM;
using System.Threading.Tasks;
using Android.Widget;
using Java.Interop;
using Android.Views;
using NativeVyatkaCore;
using Microsoft.Practices.Unity;
using Abstractions;
using Plugins;
using Android.Content;
using Android.Provider;
using Android.Graphics;
using IT.Sephiroth.Android.Library.Picasso;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Gms.Maps;
using Android.Locations;
using Android.Gms.Maps.Model;
using Android.OS;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "BurialDetailActivity", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]            
    public class BuriaEditActivity : BaseAppCompatActivity, IOnMapReadyCallback, IQuestionAlertDialogListener
    {
        //http://www.icons4android.com/ - иконки
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var id = Intent.GetIntExtra(Constants.BURIAL_ID, -1);
            if (id != -1 && await InitBurial(id))
            {    
                FindAndBindViews();
                SetMap(savedInstanceState);
            }
            else
            {
                FinishActivitySession();
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
            carmaMap?.OnDestroy();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            carmaMap?.OnLowMemory();
        }

        protected override void OnStop()
        {
            base.OnStop();
            SetResult(Result.Ok, Intent);
        }

        private void FinishActivitySession()
        {
            Intent.PutExtra(Constants.BURIAL_RESULT_MESSAGE, "Ошибка открытия, неверный идентификатор");
            Finish();
        }

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == (int)ActivityActions.RETAKE_PHOTO && resultCode == Result.Ok)
            {               
                using (var bitmap = data.Extras.Get("data") as Bitmap)
                {
                    await SaveNewPhoto(bitmap);   
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void FindAndBindViews()
        {    
            SetContentView(Resource.Layout.Layout_BurialEditDetailActivity);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            imgPhoto = FindViewById<ImageView>(Resource.Id.imgPhoto);
            etName = FindViewById<EditText>(Resource.Id.etName);       
            etDescription = FindViewById<EditText>(Resource.Id.etDescription);
            etPhotoTime = FindViewById<EditText>(Resource.Id.etPhotoTime);
            etBirthTime = FindViewById<EditText>(Resource.Id.etBirthTime);    
            etDeathTime = FindViewById<EditText>(Resource.Id.etDeathTime);
            etPlaceTime = FindViewById<EditText>(Resource.Id.etPlaceTime);

            var item = mBurial.Item;
            Picasso.With(Application.Context).Load(item.PicturePath).Into(imgPhoto);
            etName.Text = item.Name;
            etDescription.Text = item.Desctiption;
            etPhotoTime.Text = item.Time.ToLongDateString();
            etBirthTime.Text = item.BirthTime.HasValue ? item.BirthTime.Value.ToShortDateString() : "Неизвестно";   
            etDeathTime.Text = item.DeathTime.HasValue ? item.DeathTime.Value.ToShortDateString() : "Неизвестно";   
            etPlaceTime.Text = string.Format("широта: {0} / долгота: {1}", Math.Round(item.Latitude, 5), Math.Round(item.Longitude, 5));
        }

        private void SetMap(Bundle savedInstanceState)
        {
            carmaMap = FindViewById<MapView>(Resource.Id.mapView);
            carmaMap.OnCreate(savedInstanceState); 
            carmaMap.GetMapAsync(this);
        }

        private async Task<bool> InitBurial(int id)
        {
            try
            {                
                mBurial = await BurialEssence.GetAsync(id, mBurialsManager, MainApplication.Container.Resolve<IImageFactor>());
                return mBurial.Item != null;
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                FinishActivitySession();
                return false;
            }
        }

        [Export("OnRetakePhoto")]
        public void OnRetakePhoto(View view)
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, (int)ActivityActions.RETAKE_PHOTO);
        }

        [Export("OnSetBirthTime")]
        public void OnSetBirthTime(View view)
        {
            DateTime time = DateTime.UtcNow;
            DateTime.TryParse(etBirthTime.Text, out time);                   
            var tpd = new DatePickerDialog(this, HandleDateBirthSet, time.Year, time.Month, time.Day);
            tpd.Show();
        }

        [Export("OnSetDeathTime")]
        public void OnSetDeathTime(View view)
        {
            DateTime time = DateTime.UtcNow;
            DateTime.TryParse(etDeathTime.Text, out time);                
            var tpd = new DatePickerDialog(this, HandleDateDeathSet, time.Year, time.Month, time.Day);
            tpd.Show();
        }

        private void HandleDateBirthSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            mBurial.Item.BirthTime = e.Date;
            etBirthTime.Text = e.Date.ToShortDateString();
        }

        private void HandleDateDeathSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            mBurial.Item.DeathTime = e.Date;
            etDeathTime.Text = e.Date.ToShortDateString();
        }

        private async Task SaveNewPhoto(Bitmap bitmap)
        {
            try
            {                
                var array = BitmapHelper.ToByteArray(bitmap);
                var name = System.IO.Path.GetRandomFileName() + ".png";
                var fuctor = MainApplication.Container.Resolve<IImageFactor>();
                await fuctor.SaveImageToFileSystemAsync(array, name);
                mBurial.Item.PicturePath = fuctor.GetImagePath(name);
                Picasso.With(Application.Context).Load(mBurial.Item.PicturePath).Into(imgPhoto);
                bitmap.Recycle();
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_detailes_bar, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                case Resource.Id.action_save:
                    Task.Run(async () => await SaveRecordChanges(mBurial.Item));
                    break;
                case Resource.Id.action_delete:
                    AskDeleteRecords();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private async Task SaveRecordChanges(BurialEntity burial)
        {
            var progress = MaterialProgressDialog.NewInstance();
            try
            {
                SupportFragmentManager.BeginTransaction().Add(progress, MaterialProgressDialog.MaterialProgressDialogTag).CommitAllowingStateLoss();
                burial.Name = etName.Text;
                burial.Desctiption = etDescription.Text;
                DateTime time;
                if (DateTime.TryParse(etBirthTime.Text, out time))
                {
                    burial.BirthTime = time;
                }
                if (DateTime.TryParse(etDeathTime.Text, out time))
                {
                    burial.DeathTime = time;
                }
                await mBurialsManager.UpdateBurial(burial);     
                Intent.PutExtra(Constants.BURIAL_RESULT_MESSAGE, "Запись обновлена");
               
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                Intent.PutExtra(Constants.BURIAL_RESULT_MESSAGE, "Ошибка сохранения");
            }
            finally
            {                
                Finish();
                progress.Dismiss();
            }
        }

        private void AskDeleteRecords()
        {
            var dialog = QuestionAlertDialog.NewInstance("Вы действительно хотите удалить запись?", "Внимание", QuestionType.DeleteRecord);
            dialog.Show(SupportFragmentManager, QuestionAlertDialog.QuestionAlertDialogTag);
        }

        private async Task DeleteRecord(BurialEntity burial)
        {
            var progress = MaterialProgressDialog.NewInstance();
            try
            {               
                SupportFragmentManager.BeginTransaction().Add(progress, MaterialProgressDialog.MaterialProgressDialogTag).CommitAllowingStateLoss();
                await mBurialsManager.DeleteBurial(burial);  
                Intent.PutExtra(Constants.BURIAL_RESULT_MESSAGE, "Запись удалена");
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                Intent.PutExtra(Constants.BURIAL_RESULT_MESSAGE, "Ошибка удаления");
            }
            finally
            {  
                Finish();
                progress.Dismiss();
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var item = mBurial.Item;
            if (googleMap != null)
            {
                carmaMap?.OnResume();
                var position = new LatLng(item.Latitude, item.Longitude);          
                var camPos = new CameraPosition.Builder().Target(position).Zoom(15f).Build();
                var camUpdate = CameraUpdateFactory.NewCameraPosition(camPos);
                googleMap.MoveCamera(camUpdate);
              
                var marker = new MarkerOptions();
                marker.SetPosition(position);
                marker.SetTitle(item.Name);
                googleMap.AddMarker(marker);             
            }
        }

        public async void OnDialogPositiveClick(QuestionType type)
        {
            switch (type)
            {
                case QuestionType.DeleteRecord:
                    await DeleteRecord(mBurial.Item);
                    break;
            }
        }

        public void OnDialogNegitiveClick(QuestionType type)
        {

        }

        private static IBurialsManager mBurialsManager
        {
            get
            {
                return MainApplication.Container.Resolve<IBurialsManager>();
            }
        }

        private BurialEssence mBurial;
        private ImageView imgPhoto;
        private EditText etName;
        private EditText etDescription;
        private EditText etPhotoTime;
        private EditText etBirthTime;
        private EditText etDeathTime;
        private EditText etPlaceTime;

        private MapView carmaMap;
    }
}

