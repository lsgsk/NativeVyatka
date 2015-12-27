using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using IT.Sephiroth.Android.Library.Picasso;
using System.Threading.Tasks;
using Android.Support.V4.Widget;
using Android.Content.PM;
using NativeVyatkaCore;
using Microsoft.Practices.Unity;
using Abstractions;
using Android.Views.Animations;
using Android.Graphics;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "BurialDetailActivity", Theme = "@style/AppTheme", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden)]            
    public class BurialDetailActivity : BaseAppCompatActivity
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_BurialDetailActivity);
            var id = Intent.GetIntExtra(BURIAL_ID, -1);
            if (id == -1)
            {
                SetResult(Result.Canceled);
                Intent.PutExtra(MainActivity.BURIAL_ACTIVITY_MESSAGE, "Ошибка открытия, неверный идентификатор");
                Finish();
                return;
            }
            FindAndBindViews();
            await InitBurial(id);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                case Resource.Id.action_delete:
                    return true;
                case Resource.Id.action_edit:
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_detailes_bar,menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private void FindAndBindViews()
        {          
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            var collapsingToolbarLayout = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
            var fabChangePhoto = FindViewById<FloatingActionButton>(Resource.Id.fabChangePhoto);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = "";
            //collapsingToolbarLayout.SetTitle(toolbar.Title);
            collapsingToolbarLayout.SetCollapsedTitleTextColor(Color.White); 
            collapsingToolbarLayout.SetExpandedTitleColor(Resources.GetColor(Resource.Color.color_accent));

            imgPhoto = FindViewById<ImageView>(Resource.Id.image);
            tvDesc = FindViewById<TextView>(Resource.Id.tvDesc);
            tvLivingTime = FindViewById<TextView>(Resource.Id.tvLivingTime);
            tvPlace = FindViewById<TextView>(Resource.Id.tvPlace);
            tvPhotoTime = FindViewById<TextView>(Resource.Id.tvPhotoTime);
            nestedscroll = FindViewById<NestedScrollView>(Resource.Id.nestedscroll); 
            rlProgressPanel = FindViewById<RelativeLayout>(Resource.Id.rlProgressPanel);
        }

        private async Task InitBurial(int id)
        {
            var ct = MainApplication.Container;
            mBurial = await BurialEssence.GetAsync(id, ct.Resolve<IBurialsManager>(),  ct.Resolve<IImageFactor>());
            var item = mBurial.Item;
            SupportActionBar.Title = item.Name;
            Picasso.With(Application.Context).Load(item.PicturePath).Into(imgPhoto);

            tvDesc.Text = string.IsNullOrEmpty(item.Desctiption) ? "Нет описания" : item.Desctiption;
            if (!item.BirthTime.HasValue && !item.DeathTime.HasValue)
            {
                tvLivingTime.Text = GetString(Resource.String.desciption_unknown);
            }
            else
            {
                var start = (item.BirthTime.HasValue) ? item.BirthTime.Value.ToShortDateString() : GetString(Resource.String.desciption_unknown);
                var finish = item.DeathTime.HasValue ? item.DeathTime.Value.ToShortDateString() : GetString(Resource.String.desciption_unknown);
                tvLivingTime.Text = string.Format("{0} - {1}", start, finish);
            } 
            if (string.IsNullOrEmpty(item.Address) && item.Longitude * item.Latitude == 0)
            {
                tvPlace.Text = "Место съемки неизвестно";
            }
            else
            {
                tvPlace.Text = string.Format("{0} {1}", item.Address, (item.Longitude * item.Latitude != 0) ? (item.Longitude + "/" + item.Latitude) : string.Empty);
            }
            tvPhotoTime.Text = item.Time.ToLongDateString(); 
            rlProgressPanel.StartAnimation(AnimationUtils.LoadAnimation(this, Android.Resource.Animation.FadeOut));
            rlProgressPanel.Visibility = ViewStates.Gone;
        }

        private BurialEssence mBurial;

        private ImageView imgPhoto;
        private TextView tvDesc;
        private TextView tvLivingTime;
        private TextView tvPlace;
        private TextView tvPhotoTime;
        private NestedScrollView nestedscroll;
        private RelativeLayout rlProgressPanel;

        public const string BURIAL_ID = "burial_id";
    }
}

