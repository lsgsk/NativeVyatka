using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.View;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "MainActivity")]            
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Activity_Main);
            FindAndBindViews(savedInstanceState);
        }

        private void FindAndBindViews(Bundle savedInstanceState)
        {
            mToolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mNavigationView = FindViewById<NavigationView>(Resource.Id.navigation_drawer);

            SetSupportActionBar(mToolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.Icon);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            mNavigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                SelectItem(e.MenuItem.ItemId);
                mDrawerLayout.CloseDrawers();
            };     
            if (savedInstanceState == null)
                SelectItem(Resource.Id.nav_map_records); 
        }

        public override void OnBackPressed()
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

        protected void SelectItem(int itemId)
        {
            Fragment fragment = null;
            string tag = null;
            switch (itemId)
            { 
                case Resource.Id.nav_my_records:
                    fragment = RecordsFragment.NewInstance();
                    tag = RecordsFragment.RecordsFragmentTag;
                    break;
                case Resource.Id.nav_map_records:
                    break;
                case Resource.Id.nav_settings:
                    break;
                case Resource.Id.nav_about:
                    break;
            }
            if (fragment != null)
            { 
                FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment, tag).AddToBackStack(tag).Commit();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_action_bar, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected DrawerLayout mDrawerLayout;
        protected NavigationView mNavigationView;
        protected Toolbar mToolbar;
    }
}

