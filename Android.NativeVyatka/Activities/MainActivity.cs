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
using System.Threading.Tasks;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Microsoft.Practices.Unity;
using Android.Content.PM;
using Java.Interop;
using Android.Widget;
using Abstractions.Interfaces.Controllers;
using Plugin.Geolocator;

namespace NativeVyatkaAndroid
{
    [Activity(Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]            
    public class MainActivity : BaseAppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mController = App.Container.Resolve<IMainController>();
            FindAndBindViews();
            if (savedInstanceState == null)
            {
                SelectItem(Resource.Id.navigation_my_records);
                mNavigationView.Menu.GetItem(0).SetChecked(true);
            }
            await CrossGeolocator.Current.StartListeningAsync(10000, 5, true);
        }

        protected async override void OnDestroy()
        {
            base.OnDestroy();
            await CrossGeolocator.Current.StopListeningAsync();
        }

        private void FindAndBindViews()
        {
            SetContentView(Resource.Layout.Layout_MainActivity);
            mToolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mNavigationView = FindViewById<NavigationView>(Resource.Id.navigation_drawer);
            var toggle = new ActionBarDrawerToggle(this, mDrawerLayout, mToolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            toggle.SyncState();
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mNavigationView.SetNavigationItemSelectedListener(this);
            SetProfile();
        }

        private void SetProfile()
        {
            var sp = Android.Support.V7.Preferences.PreferenceManager.GetDefaultSharedPreferences(this);
            var header = mNavigationView.GetHeaderView(0);
            header.FindViewById<TextView>(Resource.Id.tvProfileName).Text = sp.GetString(GetString(Resource.String.key_user_name), GetString(Resource.String.pref_default));
            header.FindViewById<TextView>(Resource.Id.tvProfileEmail).Text = sp.GetString(GetString(Resource.String.key_user_email), GetString(Resource.String.pref_default));
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
                    StartActivity(new Intent(this, typeof(SettingsActivity)));
                    break;
                case Resource.Id.navigation_about:
                    //var aboutDialog = MessageDialog.NewInstance(Resource.String.dialogs_about_message, Resource.String.dialogs_about_title);
                    //aboutDialog.Show(SupportFragmentManager, MessageDialog.MessageDialogTag);
                    break;
            }
            if (fragment != null)
            { 
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment, tag).AddToBackStack(tag).Commit();
            }
        }

        public async Task UpdateRecordsList()
        {
            /*var records = SupportFragmentManager.FindFragmentByTag(RecordsFragment.RecordsFragmentTag) as RecordsFragment;
            if (records != null)
            {
                await records.UpdateList();
            }
            var maps = SupportFragmentManager.FindFragmentByTag(MapFragment.MapFragmentTag) as MapFragment;
            if (maps != null)
            {
                await maps.UpdatePoints();
            }*/
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
        public async void OnTakeNewPhoto(View view)
        {
            await mController.CreateNewBurial();
        }
        
        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {          
            /*switch ((ActivityActions)requestCode)
            {
                case ActivityActions.TAKE_PHOTO:   
                    if (resultCode == Result.Ok)
                    {
                        using (var bitmap = data.Extras.Get("data") as Bitmap)
                        {
                            //await CreateAndSaveNewBurial(bitmap);   
                        }
                    }
                    break;
                case ActivityActions.OPEN_BURIAL:
                    await UpdateRecordsList();
                    ShowSnack(data?.GetStringExtra(Constants.BURIAL_RESULT_MESSAGE));
                    break;               
            }
            base.OnActivityResult(requestCode, resultCode, data);*/
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
                case Resource.Id.action_sync:
                    BackgroundUploadService.UploadBurials();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public IMainController mController;
        protected DrawerLayout mDrawerLayout;
        protected NavigationView mNavigationView;
        protected Toolbar mToolbar;
    }
}

