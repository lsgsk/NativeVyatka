using Android.App;
using Android.Views;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Java.Interop;
using Android.Content;
using Android.Provider;
using System;
using Android.OS;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Support.V7.App;
using NativeVyatkaCore;
using Android.Support.V4.View;
using Android.Content.PM;
using Plugin.Geolocator;
using Microsoft.Practices.Unity;
using Abstractions;
using Com.Github.Jorgecastilloprz;


namespace NativeVyatkaAndroid
{
    [Activity(Label = "MainActivity", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]            
    public class MainActivity : BaseAppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, QuestionAlertDialog.IQuestionAlertDialogListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_MainActivity);
            FindAndBindViews();
            if (savedInstanceState == null)
            {
                SelectItem(Resource.Id.navigation_my_records);
                mNavigationView.Menu.GetItem(0).SetChecked(true);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            fabNewPhoto.Click += OnTakeNewPhoto;
        }

        protected override void OnPause()
        {
            base.OnPause();
            fabNewPhoto.Click -= OnTakeNewPhoto;
        }

        private void FindAndBindViews()
        {
            mToolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mNavigationView = FindViewById<NavigationView>(Resource.Id.navigation_drawer);
            fabNewPhoto = FindViewById<FloatingActionButton>(Resource.Id.fabNewPhoto);
            fabProgressCircle = FindViewById<FABProgressCircle>(Resource.Id.fabProgressCircle);
            var toggle = new ActionBarDrawerToggle(this, mDrawerLayout, mToolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);


            mDrawerLayout.SetDrawerListener(toggle);
            toggle.SyncState();
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mNavigationView.SetNavigationItemSelectedListener(this);
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            menuItem.SetChecked(true);
            SelectItem(menuItem.ItemId);
            mDrawerLayout.CloseDrawers();
            return true;
        }

        protected void SelectItem(int itemId)
        {
            Fragment fragment = null;
            string tag = null;
            switch (itemId)
            { 
                case Resource.Id.navigation_my_records:
                    fragment = RecordsFragment.NewInstance();
                    tag = RecordsFragment.RecordsFragmentTag;
                    break;
                case Resource.Id.navigation_map_records:
                    fragment = MapFragment.NewInstance();
                    tag = MapFragment.MapFragmentTag;
                    break;
                case Resource.Id.navigation_settings:
                    break;
                case Resource.Id.navigation_about:
                    break;
            }
            if (fragment != null)
            { 
                FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment, tag).AddToBackStack(tag).Commit();
            }
        }

        public override void OnBackPressed()
        {
            if (mDrawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                mDrawerLayout.CloseDrawers();
            }
            else
            {
                if (FragmentManager.BackStackEntryCount > 1)
                {
                    FragmentManager.PopBackStack();
                }
                else
                {
                    base.OnBackPressed();
                }
            }
        }

        private void OnTakeNewPhoto(object sender, EventArgs e)
        {           
            if (mSavingPhoto)
                return;

            if (!CrossGeolocator.Current.IsGeolocationAvailable) //FIXME, то ли проверяю
            {
                var dialog = QuestionAlertDialog.NewInstance("В настоящее время gps не доступен. Вы действительно хотите добавить запись?", "Внимание", QuestionType.ContinueWithoutGps);
                dialog.Show(SupportFragmentManager, QuestionAlertDialog.QuestionAlertDialogTag);
            }
            else
            {
                OpenCamera();
            }
        }

        private void OpenCamera()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, TAKE_PHOTO);
        }

        public void OnDialogPositiveClick(QuestionType type)
        {
            switch (type)
            {
                case QuestionType.ContinueWithoutGps:
                    OpenCamera();
                    break;
            }
        }

        public void OnDialogNegitiveClick(QuestionType type)
        {
            
        }

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == TAKE_PHOTO && resultCode == Result.Ok)
            {               
                using (var bitmap = data.Extras.Get("data") as Bitmap)
                {
                    await CreateAndSaveNewBurial(bitmap);   
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        private async Task CreateAndSaveNewBurial(Bitmap bitmap)
        {
            try
            {       
                mSavingPhoto = true;
                var ct = MainApplication.Container;
                fabProgressCircle.Show(); 
                var array = await BitmapHelper.ToByteArray(bitmap);
                var burial = await BurialEssence.CreateAsync(array, ct.Resolve<IBurialsManager>(),  ct.Resolve<IImageFactor>());
                ShowSnack("Запись создана");
                SelectItem(Resource.Id.navigation_my_records);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ShowSnack("Ошибка создания записи");
            }
            finally
            {
                fabProgressCircle.Hide();
                mSavingPhoto = false;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_action_bar, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer(GravityCompat.Start); 
                    return true;

            }
            return base.OnOptionsItemSelected(item);
        }

        protected DrawerLayout mDrawerLayout;
        protected NavigationView mNavigationView;
        protected Toolbar mToolbar;
        protected FloatingActionButton fabNewPhoto;
        protected FABProgressCircle fabProgressCircle;

        private bool mSavingPhoto = false;


        public const int TAKE_PHOTO = 0;
        public const int OPEN_BURIAL = 1;

        public const string BURIAL_ACTIVITY_MESSAGE = "burial_activity_message";
    }
}

