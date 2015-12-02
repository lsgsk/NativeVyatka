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

namespace NativeVyatkaAndroid
{
    [Activity(Label = "MainActivity")]            
    public class MainActivity : BaseAppCompatActivity, QuestionAlertDialog.IQuestionAlertDialogListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Layout_MainActivity);
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
                SelectItem(Resource.Id.navigation_my_records); 
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

        [Export("OnButtonClick")]
        public void OnButtonClick(View view)
        {
            switch (view.Id)
            {
                case Resource.Id.fbNewPhoto:
                    TakeBurialPhoto();
                    break;
            }
        }

        private void TakeBurialPhoto()
        {
            if (!mLocalization.GpsStatus)
            {
                var dialog = QuestionAlertDialog.NewInstance("gps не доступен", "внимание");
                dialog.Show(SupportFragmentManager, QuestionAlertDialog.QuestionAlertDialogTag);
            }
            else
            {
                OnDialogPositiveClick();
            }
        }

        public void OnDialogPositiveClick()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, TAKE_PHOTO);
        }

        public void OnDialogNegitiveClick()
        {
        }

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == TAKE_PHOTO && resultCode == Result.Ok)
            {                
                using (var bitmap = data.Extras.Get("data").JavaCast<Bitmap>())
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
                var imageName = System.IO.Path.GetRandomFileName();
                var array = await BitmapHelper.ResizeImage(bitmap);
                await new PhotoStorageManager(this).SaveBurialImageToFileSystemAsync(imageName, array);
                await mBurialsManager.InsertBurial(imageName, mLocalization.Location);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ListInvalidate()
        {
            
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

