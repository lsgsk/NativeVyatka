using Android.App;
using Android.Views;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Content;
using Android.Provider;
using System;
using Android.OS;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Support.V7.App;
using NativeVyatkaCore;
using Android.Support.V4.View;
using Plugin.Geolocator;
using Microsoft.Practices.Unity;
using Abstractions;
using Android.Content.PM;
using Java.Interop;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "MainActivity", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]            
    public class MainActivity : BaseAppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, IQuestionAlertDialogListener
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

        protected override void OnSaveInstanceState(Bundle outState)
        {
            //No call for super(). Bug on API Level > 11.
        }

        private void FindAndBindViews()
        {
            mToolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mNavigationView = FindViewById<NavigationView>(Resource.Id.navigation_drawer);
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
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment, tag).AddToBackStack(tag).Commit();
            }
        }

        public async Task UpdateRecordsList()
        {
            var records = SupportFragmentManager.FindFragmentByTag(RecordsFragment.RecordsFragmentTag) as RecordsFragment;
            if (records != null)
            {
                await records.UpdateList();
            }
            var maps = SupportFragmentManager.FindFragmentByTag(MapFragment.MapFragmentTag) as MapFragment;
            if (maps != null)
            {
                await maps.UpdatePoints();
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

        [Export("OnTakeNewPhoto")]
        public void OnTakeNewPhoto(View view)
        {           
            if (!CrossGeolocator.Current.IsGeolocationAvailable)
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
            StartActivityForResult(intent, (int)ActivityActions.TAKE_PHOTO);
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
            switch ((ActivityActions)requestCode)
            {
                case ActivityActions.TAKE_PHOTO:   
                    if (resultCode == Result.Ok)
                    {
                        using (var bitmap = data.Extras.Get("data") as Bitmap)
                        {
                            await CreateAndSaveNewBurial(bitmap);   
                        }
                    }
                    break;
                case ActivityActions.OPEN_BURIAL:
                    await UpdateRecordsList();
                    ShowSnack(data?.GetStringExtra(Constants.BURIAL_RESULT_MESSAGE));
                    break;               
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        private async Task CreateAndSaveNewBurial(Bitmap bitmap)
        {
            var progress = MaterialProgressDialog.NewInstance();
            try
            {       
                var ct = MainApplication.Container;
                SupportFragmentManager.BeginTransaction().Add(progress, MaterialProgressDialog.MaterialProgressDialogTag).CommitAllowingStateLoss();
                var burial = await BurialEssence.CreateAsync(BitmapHelper.ToByteArray(bitmap), ct.Resolve<IBurialsManager>(), ct.Resolve<IImageFactor>());
                ShowSnack("Запись создана");
                await UpdateRecordsList();
                var intent = new Intent(this, typeof(BuriaEditActivity));
                intent.PutExtra(Constants.BURIAL_ID, burial.Item.Id);
                StartActivityForResult(intent, (int)ActivityActions.OPEN_BURIAL);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ShowSnack("Ошибка создания записи");
            }
            finally
            {
                progress.Dismiss();
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
    }
}

