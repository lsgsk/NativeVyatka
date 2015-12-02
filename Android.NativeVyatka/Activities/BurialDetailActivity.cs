
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
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.App;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "BurialDetailActivity")]            
    public class BurialDetailActivity : BaseAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_BurialDetailActivity);
            FindAndBindViews();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void FindAndBindViews()
        {
            image = FindViewById<ImageView>(Resource.Id.image);
            image.SetImageResource(Resource.Drawable.katya);
            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            collapsingToolbarLayout = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
            collapsingToolbarLayout.Title = ("Collapsing");
            collapsingToolbarLayout.SetExpandedTitleColor(Resources.GetColor(Android.Resource.Color.Transparent));
        }

        private CollapsingToolbarLayout collapsingToolbarLayout;
        private ImageView image;
    }
}

