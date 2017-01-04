using Android.App;
using Android.Views;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Microsoft.Practices.Unity;
using Android.Content.PM;
using Java.Interop;
using Android.Widget;
using Abstractions.Interfaces.Controllers;
using Plugin.Geolocator;
using Square.Picasso;

namespace NativeVyatkaAndroid
{
    [Activity(Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]            
    public class MainActivity : BaseAppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        public MainActivity()
        {
            mController = App.Container.Resolve<IMainController>();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FindAndBindViews();
            SetProfile();
            if (savedInstanceState == null)
            {
                SelectItem(Resource.Id.navigation_my_records);
                mNavigationView.Menu.GetItem(0).SetChecked(true);
            }            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mController.Dispose();
        }

        private void FindAndBindViews()
        {
            SetContentView(Resource.Layout.Layout_MainActivity);
            var mToolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mNavigationView = FindViewById<NavigationView>(Resource.Id.navigation_drawer);
            new ActionBarDrawerToggle(this, mDrawerLayout, mToolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close).SyncState();
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mNavigationView.SetNavigationItemSelectedListener(this);            
        }

        private void SetProfile()
        {
            View header = mNavigationView.GetHeaderView(0);
            header.FindViewById<TextView>(Resource.Id.tvProfileName).Text = mController.Profile.Name;
            header.FindViewById<TextView>(Resource.Id.tvProfileEmail).Text = mController.Profile.Email;
            Picasso.With(BaseContext).Load(mController.Profile.PictureUrl).Resize(200, 200).CenterCrop().Into(header.FindViewById<ImageView>(Resource.Id.imgProfilePhoto));
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
            }
            if (fragment != null)
            { 
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment, tag).AddToBackStack(tag).Commit();
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
        public async void OnTakeNewPhoto(View view)
        {
            await mController.CreateNewBurial();
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
        public readonly IMainController mController;
        protected DrawerLayout mDrawerLayout;
        protected NavigationView mNavigationView;
    }
}

