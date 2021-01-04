using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.FloatingActionButton;
using Unity;

namespace NativeVyatka
{
    public class MainFlowFragment : Fragment, IBackFragment, IMainObserver
    {
        public readonly IMainPresenter presenter;
        private Fragment fragment;
        private BottomNavigationView bottomNavigation;
        private FloatingActionButton fabNewPhoto;
        private TextView tvGpsState;

        public static MainFlowFragment NewInstance() {
            IMainPresenter presenter = App.Container.Resolve<IMainPresenter>();
            return new MainFlowFragment(presenter) {
                HasOptionsMenu = true,
                RetainInstance = true
            };
        }

        public MainFlowFragment(IMainPresenter presenter) {
            this.presenter = presenter;
        }

        public override void OnDestroy() {
            base.OnDestroy();
            this.presenter.Dispose();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            return inflater.Inflate(Resource.Layout.FlowFragment_Dashboard, null);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);
            tvGpsState = view.FindViewById<TextView>(Resource.Id.tvGpsState);
            bottomNavigation = view.FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigation.NavigationItemSelected += OnNavigationItemSelected;
            fabNewPhoto = view.FindViewById<FloatingActionButton>(Resource.Id.fabNewPhoto);
            fabNewPhoto.Click += (sender, e) => presenter.CreateNewBurial();
            var mToolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            (this.Activity as AppCompatActivity).SetSupportActionBar(mToolbar);
            presenter.AddObserver(this);
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {
            base.OnActivityCreated(savedInstanceState);
            presenter.InitFragmentState();
        }

        public override void OnDestroyView() {
            base.OnDestroyView();
            presenter.RemoveObserver(this);
        }

        public void OnNavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e) {
            UpdateScreenState(e.Item.ItemId);
        }

        public void UpdateScreenState(int tab) {
            string tag = null;
            switch (tab) {
                case Resource.Id.navigation_my_records:
                    tag = typeof(RecordsFragment).ToString();
                    fragment = (ChildFragmentManager.FindFragmentByTag(tag) as RecordsFragment) ?? RecordsFragment.NewInstance(presenter);
                    fabNewPhoto.Visibility = ViewStates.Visible;
                    break;
                case Resource.Id.navigation_favorites:
                    tag = typeof(FavoritesFragment).ToString();
                    fragment = (ChildFragmentManager.FindFragmentByTag(tag) as FavoritesFragment) ?? FavoritesFragment.NewInstance(presenter);
                    fabNewPhoto.Visibility = ViewStates.Visible;
                    break;
                case Resource.Id.navigation_map_records:
                    tag = typeof(MapFragment).ToString();
                    fragment = (ChildFragmentManager.FindFragmentByTag(tag) as MapFragment) ?? MapFragment.NewInstance(presenter);
                    fabNewPhoto.Visibility = ViewStates.Visible;
                    break;
                case Resource.Id.navigation_profile:
                    tag = typeof(ProfileFragment).ToString();
                    fragment = (ChildFragmentManager.FindFragmentByTag(tag) as ProfileFragment) ?? ProfileFragment.NewInstance(presenter);
                    fabNewPhoto.Visibility = ViewStates.Gone;
                    break;
            }
            if (fragment != null) {
                ChildFragmentManager
                     .BeginTransaction()
                     .Replace(Resource.Id.content_frame, fragment, tag)
                     .AddToBackStack(tag)
                     .Commit();
            }
        }

        public void OpenSettiongs() {
            var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
            var uri = Android.Net.Uri.FromParts("package", Context.ApplicationContext.PackageName, null);
            intent.SetData(uri);
            StartActivity(intent);
        }

        public void UpadateGpsEnableState(GpsState e) {
            tvGpsState.Text = $"Gps: {e.Satetiles}/{ ((e.Satetiles == 0) ? 0 : (e.Accuracy ?? 0)):0.#}";
            if (e.Satetiles > 4 && e.Accuracy <= 2) {
                fabNewPhoto.Enabled = true;
                tvGpsState.SetBackgroundResource(Resource.Drawable.small_rounded_corner_green);
            }
            else if (e.Satetiles > 3 && e.Accuracy <= 7) {
                fabNewPhoto.Enabled = true;
                tvGpsState.SetBackgroundResource(Resource.Drawable.small_rounded_corner_yellow);
            }
            else {
                fabNewPhoto.Enabled = false;
                tvGpsState.SetBackgroundResource(Resource.Drawable.small_rounded_corner_red);
            }
#if DEBUG
            fabNewPhoto.Enabled = true;
#endif
        }

        public bool OnBackPressed() {
            presenter.GoBack();
            return true;
        }
    }
}
