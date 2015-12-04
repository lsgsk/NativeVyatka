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
using IT.Sephiroth.Android.Library.Picasso;
using Java.IO;
using System.Threading.Tasks;
using Android.Support.V4.Widget;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "BurialDetailActivity")]            
    public class BurialDetailActivity : BaseAppCompatActivity
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_BurialDetailActivity);
            await FindAndBindViews(Intent.GetIntExtra(BURIAL_ID, -1));
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

        private async Task FindAndBindViews(int id)
        {
            if (id == -1)
            {
                Finish();
                return;
            }

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            var collapsingToolbarLayout = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
            var fabChangePhoto = FindViewById<FloatingActionButton>(Resource.Id.fabChangePhoto);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "";
            //collapsingToolbarLayout.SetTitle(toolbar.Title);
            collapsingToolbarLayout.SetCollapsedTitleTextColor(Resources.GetColor(Android.Resource.Color.White)); 
            collapsingToolbarLayout.SetExpandedTitleColor(Resources.GetColor(Resource.Color.color_accent));


            var imgPhoto = FindViewById<ImageView>(Resource.Id.image);
            var tvDesc = FindViewById<TextView>(Resource.Id.tvDesc);
            var tvLivingTime = FindViewById<TextView>(Resource.Id.tvLivingTime);
            var tvPlace = FindViewById<TextView>(Resource.Id.tvPlace);
            var tvPhotoTime = FindViewById<TextView>(Resource.Id.tvPhotoTime);
            var nestedscroll = FindViewById<NestedScrollView>(Resource.Id.nestedscroll);

            var item = await mBurialsManager.GetBurial(id, new System.Threading.CancellationToken());
            SupportActionBar.Title = item.Name;
            Picasso.With(Application.Context).Load(new File(Application.Context.FilesDir.AbsolutePath + "/" + item.PicturePath)).Resize(100, 100).CenterCrop().Into(imgPhoto);
            SupportActionBar.Title = item.Name;
            //collapsingToolbarLayout.Title = item.Name;
            tvDesc.Text = item.Desctiption;
            if (!item.BirthTime.HasValue && !item.DeathTime.HasValue)
            {
                tvLivingTime.Text = GetString(Resource.String.desciption_unknown);
            }
            else
            {
                var start = (item.BirthTime.HasValue) ? item.BirthTime.Value.ToShortDateString() : GetString(Resource.String.desciption_unknown);
                var finish = item.DeathTime.HasValue ? item.DeathTime.Value.ToShortDateString() : GetString(Resource.String.desciption_unknown);
                tvDesc.Text = string.Format("{0} - {1}", start, finish);
            }  
            tvPlace.Text = "Vjcrdf";
            tvPhotoTime.Text = item.Time.ToLongDateString();           
        }

        public const string BURIAL_ID = "burial_id";
    }
}

