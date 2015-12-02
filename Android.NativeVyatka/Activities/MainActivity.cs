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
using System.Threading;

namespace NativeVyatkaAndroid
{
    [Activity(Label = "MainActivity")]            
    public class MainActivity : BaseAppCompatActivity, QuestionAlertDialog.IQuestionAlertDialogListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_MainActivity);
            FindAndBindViews();
            if (savedInstanceState == null)
                SelectItem(Resource.Id.navigation_my_records);
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
            mNavigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);
                SelectItem(e.MenuItem.ItemId);
                mDrawerLayout.CloseDrawers();
            };    
        }

        public override void OnBackPressed()
        {
            if (mDrawerLayout.IsDrawerOpen(Android.Support.V4.View.GravityCompat.Start))
            {
                mDrawerLayout.CloseDrawer(Android.Support.V4.View.GravityCompat.Start);
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

        [Export("OnButtonClick")]
        public void OnButtonClick(View view)
        {
            switch (view.Id)
            {
                case Resource.Id.fbNewPhoto:
                    AskCamera();
                    break;
            }
        }

        private void AskCamera()
        {
            if (!mLocalization.GpsStatus)
            {
                var dialog = QuestionAlertDialog.NewInstance("В настоящее время gps не доступен. Вы действительно хотите добавить запись?", "Внимание", QuestionType.ContinueWithoutGps);
                dialog.Show(SupportFragmentManager, QuestionAlertDialog.QuestionAlertDialogTag);
            }
            else
            {
                OpenCamera();
            }
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

        private void OpenCamera()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, TAKE_PHOTO);
        }

        public void OnDialogNegitiveClick(QuestionType type)
        {
            
        }

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == TAKE_PHOTO && resultCode == Result.Ok)
            {               
                await CreateAndSaveNewBurial(data.Extras.Get("data") as Bitmap);          
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        private async Task CreateAndSaveNewBurial(Bitmap bitmap)
        {
            try
            {
                var imagepath = System.IO.Path.GetRandomFileName();
                var array = await BitmapHelper.ResizeImage(bitmap);
                new PhotoStorageManager(this).SaveBurialImageToFileSystemAsync(imagepath, array);
                var unknown = GetString(Resource.String.unknown); 
                var item = new BurialEntity()
                {
                    HashId = Guid.NewGuid().ToString(),  
                    Name = unknown,           
                    Desctiption = unknown,
                    Time = DateTime.UtcNow,             
                    Latitude = mLocalization.Location.Latitude,             
                    Longitude = mLocalization.Location.Longitude,                 
                    PicturePath = imagepath,                  
                    IsSended = false
                };
                await mBurialsManager.InsertBurial(item, new CancellationToken());
                SelectItem(Resource.Id.navigation_my_records);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_action_bar, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected DrawerLayout mDrawerLayout;
        protected NavigationView mNavigationView;
        protected Toolbar mToolbar;
        public const int TAKE_PHOTO = 16188;
    }
}

