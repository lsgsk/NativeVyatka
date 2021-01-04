using Android.Widget;
using Android.OS;
using Android.Views;
using Unity;
using Google.Android.Material.TextField;
using Android.Support.V4.App;

namespace NativeVyatka
{
    public class LoginFlowFragment : Fragment, ILoginObserver
    {
        private readonly ILoginPresenter presenter;
        private TextInputEditText etEmailView;
        private TextInputEditText etPasswordView;
        private TextInputLayout tilEmailView;
        private TextInputLayout tilPasswordView;
        private ProgressBar progressBar;
        private Button btSignInClick;

        public static LoginFlowFragment NewInstance() {
            ILoginPresenter presenter = App.Container.Resolve<ILoginPresenter>();
            return new LoginFlowFragment(presenter) {
                RetainInstance = true
            };
        }

        public LoginFlowFragment(ILoginPresenter presenter) {
            this.presenter = presenter;
        }

        public override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
        }

        public override void OnDestroy() {
            base.OnDestroy();
            presenter.Dispose();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            return inflater.Inflate(Resource.Layout.FlowFragment_Login, null);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) {
            base.OnViewCreated(view, savedInstanceState);
            etEmailView = view.FindViewById<TextInputEditText>(Resource.Id.etEmail);
            etPasswordView = view.FindViewById<TextInputEditText>(Resource.Id.etPassword);
            tilEmailView = view.FindViewById<TextInputLayout>(Resource.Id.tilEmail);
            tilPasswordView = view.FindViewById<TextInputLayout>(Resource.Id.tilPassword);
            etPasswordView.EditorAction += OnSignInWithKeyboard;
            btSignInClick = view.FindViewById<Button>(Resource.Id.btSignInClick);
            btSignInClick.Click += OnSignInClick;
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            presenter.AddObserver(this);
        }

        public override void OnActivityCreated(Bundle savedInstanceState) {
            base.OnActivityCreated(savedInstanceState);
            presenter.TryAutoLogin();
        }

        public override void OnDestroyView() {
            base.OnDestroyView();
            this.presenter.RemoveObserver(this);
        }

        private void OnSignInClick(object sender, System.EventArgs e) {
            presenter.Login(etEmailView.Text, etPasswordView.Text);
        }

        private void OnSignInWithKeyboard(object sender, TextView.EditorActionEventArgs e) {
            if ((int)e.ActionId == Resource.Id.login || e.ActionId == Android.Views.InputMethods.ImeAction.Done) {
                presenter.Login(etEmailView.Text, etPasswordView.Text);
            }
        }

        public void UpdateProgress(bool value) {
            if (value) {
                etEmailView.Enabled = etPasswordView.Enabled = btSignInClick.Enabled = false;
                progressBar.Visibility = ViewStates.Visible;
            }
            else {
                etEmailView.Enabled = etPasswordView.Enabled = btSignInClick.Enabled = true;
                progressBar.Visibility = ViewStates.Invisible;
            }
        }

        public void UpdateValidation(ValidationViews id, string message) {
            switch (id) {
                case ValidationViews.clear:
                    tilEmailView.Error = null;
                    tilEmailView.Error = null;
                    break;
                case ValidationViews.login:
                    if (!string.IsNullOrEmpty(message)) {
                        tilEmailView.Error = message;
                        etEmailView.RequestFocus();
                    }
                    else {
                        tilEmailView.Error = null;
                    }
                    break;
                case ValidationViews.password:
                    if (!string.IsNullOrEmpty(message)) {
                        tilPasswordView.Error = message;
                        etPasswordView.RequestFocus();
                    }
                    else {
                        tilPasswordView.Error = null;
                    }
                    break;
            }
        }
    }
}