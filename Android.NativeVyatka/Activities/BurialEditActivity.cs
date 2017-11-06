using System;
using Android.App;
using Android.Content.PM;
using Android.Widget;
using Java.Interop;
using Android.Views;
using Microsoft.Practices.Unity;
using Android.Content;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.OS;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Plugins;
using Newtonsoft.Json;
using Abstractions.Models.AppModels;
using NativeVyatkaCore.Utilities;
using Square.Picasso;
using Android.Support.Design.Widget;

namespace NativeVyatkaAndroid
{
    [Activity(Theme = "@style/AppTheme", Label = "@string/burial_edit_title", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class BurialEditActivity : BaseAppCompatActivity
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mController.Dispose();
        }

        private void OnDisplayBurial(BurialModel burial)
        {
            var picasso = Picasso.With(BaseContext);
            var creator = (burial.PicturePath.StartsWith("http"))
                ? picasso.Load(burial.PicturePath)
                : picasso.Load(new Java.IO.File(burial.PicturePath));
            creator.ResizeDimen(Resource.Dimension.photo_size, Resource.Dimension.photo_size).CenterInside().OnlyScaleDown().Into(imgPhoto);
            SupportActionBar.Title = $"{burial.Name} {burial.Surname} {burial.Patronymic}";
            etName.Text = burial.Name;
            etSurname.Text = burial.Surname;
            etPatronymic.Text = burial.Patronymic;
            etDescription.Text = burial.Description;
            DisplayDate(etPhotoTime, burial.RecordTime);
            DisplayDate(etBirthTime, burial.BirthDay);
            DisplayDate(etDeathTime, burial.DeathDay);
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
            mSaveIcon = FindViewById<FloatingActionButton>(Resource.Id.fabSave);
            OnBurialUpdated(null, mController.Updated);
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

        [Export("OnSaveClick")]
        public async void OnSaveClick(View view)
        {
           await mController.SaveAndUploadBurialAsync();
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

        private void OnBurialUpdated(object sender, bool e)
        {
            mSaveIcon.Visibility = e ? ViewStates.Visible : ViewStates.Gone;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mController.SaveAndUploadBurialAndGoBackAsync();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public async override void OnBackPressed()
        {
            await mController.SaveAndUploadBurialAndGoBackAsync();
        }

        private readonly IBurialEditController mController;
        private ImageView imgPhoto;
        private EditText etName, etSurname, etPatronymic, etDescription, etPhotoTime, etBirthTime, etDeathTime;
        private FloatingActionButton mSaveIcon;
    }
}

