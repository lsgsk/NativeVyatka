using Android.OS;
using Android.Views;
using Android.Widget;
using Square.Picasso;

namespace NativeVyatka
{
    public class ProfileFragment : ProgressFragment, IBackFragment, IProfileObserver
    {
        public readonly IProfilePresenter presenter;
        private View contentView;
        private TextView tvProfileName;
        private TextView tvProfileEmail;
        private ImageView imgProfilePhoto;
        private Button btLogout;

        public static ProfileFragment NewInstance(IProfilePresenter presenter) {
            return new ProfileFragment(presenter) {
                RetainInstance = true
            };
        }

        public ProfileFragment(IProfilePresenter presenter) {
            this.presenter = presenter;
        }

        public override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            this.contentView = inflater.Inflate(Resource.Layout.Fragment_Profile, null);
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);
            tvProfileName = contentView.FindViewById<TextView>(Resource.Id.tvProfileName);
            tvProfileEmail = contentView.FindViewById<TextView>(Resource.Id.tvProfileEmail);
            imgProfilePhoto = contentView.FindViewById<ImageView>(Resource.Id.imgProfilePhoto);
            btLogout = contentView.FindViewById<Button>(Resource.Id.btLogout);
            btLogout.Click += (sender, e) => presenter.Logout();
            Refresher.Enabled = false;
            presenter.AddObserver(this);
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {
            base.OnActivityCreated(savedInstanceState);
            SetContentView(contentView);
            presenter.DisplayProfile();
        }

        public override void OnDestroyView() {
            base.OnDestroyView();
            this.presenter.RemoveObserver(this);
        }

        public void OnProfileChanged(ProfileModel profile) {
            tvProfileName.Text = profile.Name;
            tvProfileEmail.Text = profile.Email;
            if (!string.IsNullOrEmpty(profile.PictureUrl)) {
                Picasso.Get()
                       .Load(profile.PictureUrl)
                       .Placeholder(Resource.Drawable.nophoto)
                       .Resize(200, 200)
                       .CenterCrop()
                       .Into(imgProfilePhoto);
            }
            SetContentShown(true);
        }

        public void OnProfileFailed(string message) {
            SetEmptyText(message);
            SetContentEmpty(true);
            SetContentShown(true);
        }

        public bool OnBackPressed() {
            return false;
        }
    }
}
