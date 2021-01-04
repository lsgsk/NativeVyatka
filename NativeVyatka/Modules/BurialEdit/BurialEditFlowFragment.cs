using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Google.Android.Material.FloatingActionButton;
using Square.Picasso;
using Unity;

namespace NativeVyatka
{
    public class BurialEditFlowFragment : Fragment, IBackFragment, IBurialEditObserver
    {
        private readonly IBurialEditPresenter presenter;
        private readonly BurialModel burial;
        private readonly Action closeCallback;
        private CheckBox cbFavorite;
        private ImageView imgPhoto;
        private EditText etName, etSurname, etPatronymic, etDescription, etPhotoTime;
        private EditText etBirthTimeDay, etBirthTimeMonth, etBirthTimeYear;
        private EditText etDeathTimeDay, etDeathTimeMonth, etDeathTimeYear;
        private FloatingActionButton mSaveIcon;

        public static BurialEditFlowFragment NewInstance(BurialModel burial, Action closeCallback) {
            IBurialEditPresenter presenter = App.Container.Resolve<IBurialEditPresenter>();
            return new BurialEditFlowFragment(presenter, burial, closeCallback) {
                HasOptionsMenu = true
            };
        }

        public BurialEditFlowFragment(IBurialEditPresenter presenter, BurialModel burial, Action closeCallback) {
            this.presenter = presenter;
            this.burial = burial;
            this.closeCallback = closeCallback;
        }

        public override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroy() {
            base.OnDestroy();
            presenter.Dispose();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            var view = inflater.Inflate(Resource.Layout.FlowFragment_BurialEditDetail, null);
            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            var activity = this.Activity as AppCompatActivity;
            activity.SetSupportActionBar(toolbar);
            activity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);
            cbFavorite = view.FindViewById<CheckBox>(Resource.Id.cbFavorite);
            cbFavorite.CheckedChange += (s, e) => presenter.UpdateBurial(e.IsChecked);
            imgPhoto = view.FindViewById<ImageView>(Resource.Id.imgPhoto);

            etSurname = view.FindViewById<EditText>(Resource.Id.etSurname);
            etSurname.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, e.Text.ToString());
            etName = view.FindViewById<EditText>(Resource.Id.etName);
            etName.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, e.Text.ToString());
            etPatronymic = view.FindViewById<EditText>(Resource.Id.etPatronymic);
            etPatronymic.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, e.Text.ToString());
            etDescription = view.FindViewById<EditText>(Resource.Id.etDescription);
            etDescription.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, e.Text.ToString());
            etPhotoTime = view.FindViewById<EditText>(Resource.Id.etPhotoTime);

            etBirthTimeDay = view.FindViewById<EditText>(Resource.Id.etBirthTimeDay);
            etBirthTimeMonth = view.FindViewById<EditText>(Resource.Id.etBirthTimeMonth);
            etBirthTimeYear = view.FindViewById<EditText>(Resource.Id.etBirthTimeYear);
            etBirthTimeDay.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, etBirthTimeDay.Text, etBirthTimeMonth.Text, etBirthTimeYear.Text);
            etBirthTimeMonth.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, etBirthTimeDay.Text, etBirthTimeMonth.Text, etBirthTimeYear.Text);
            etBirthTimeYear.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, etBirthTimeDay.Text, etBirthTimeMonth.Text, etBirthTimeYear.Text);

            etDeathTimeDay = view.FindViewById<EditText>(Resource.Id.etDeathTimeDay);
            etDeathTimeMonth = view.FindViewById<EditText>(Resource.Id.etDeathTimeMonth);
            etDeathTimeYear = view.FindViewById<EditText>(Resource.Id.etDeathTimeYear);
            etDeathTimeDay.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, etDeathTimeDay.Text, etDeathTimeMonth.Text, etDeathTimeYear.Text);
            etDeathTimeMonth.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, etDeathTimeDay.Text, etDeathTimeMonth.Text, etDeathTimeYear.Text);
            etDeathTimeYear.TextChanged += (s, e) => presenter.UpdateBurial((s as View).Id, etDeathTimeDay.Text, etDeathTimeMonth.Text, etDeathTimeYear.Text);

            mSaveIcon = view.FindViewById<FloatingActionButton>(Resource.Id.fabSave);
            mSaveIcon.Click += async (s, e) => await presenter.SaveAndUploadBurialAsync();
            presenter.AddObserver(this);
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {
            base.OnActivityCreated(savedInstanceState);
            presenter.DisplayBurial(burial, closeCallback);
        }

        public override void OnDestroyView() {
            base.OnDestroyView();
            this.presenter.RemoveObserver(this);
        }

        public void DisplayBurial(BurialViewModel burial) {
            RequestCreator creator = null;
            if (burial.Image.UrlPicture != null) {
                creator = Picasso.Get().Load(burial.Image.UrlPicture);
            }
            if (burial.Image.LocalPicture != null) {
                creator = Picasso.Get().Load(burial.Image.LocalPicture);
            }
            if (creator != null) {
                creator.Placeholder(Resource.Drawable.nophoto)
                       .ResizeDimen(Resource.Dimension.photo_size, Resource.Dimension.photo_size)
                       .CenterInside()
                       .OnlyScaleDown()
                       .Into(imgPhoto);
            }
            (this.Activity as AppCompatActivity).SupportActionBar.Title = burial.Title;
            etSurname.Text = burial.Surname;
            etName.Text = burial.Name;
            etPatronymic.Text = burial.Patronymic;
            etDescription.Text = burial.Description;
            etPhotoTime.Text = burial.RecordTime;

            etBirthTimeDay.Text = burial.BirthDay.Day;
            etBirthTimeMonth.Text = burial.BirthDay.Month;
            etBirthTimeYear.Text = burial.BirthDay.Year;
            etDeathTimeDay.Text = burial.DeathDay.Day;
            etDeathTimeMonth.Text = burial.DeathDay.Month;
            etDeathTimeYear.Text = burial.DeathDay.Year;

            cbFavorite.Checked = burial.IsFavorite;
        }

        public void UpdatedBurialSaveState(bool value) {
            mSaveIcon.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater) {
            inflater.Inflate(Resource.Menu.menu_edit_bar, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            switch (item.ItemId) {
                case Android.Resource.Id.Home:
                    presenter.SaveAndUploadBurialAndGoBackAsync();
                    return true;
                case Resource.Id.action_delete:
                    presenter.DeleteRecordAsync();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public bool OnBackPressed() {
            presenter.SaveAndUploadBurialAndGoBackAsync();
            return true;
        }
    }
}