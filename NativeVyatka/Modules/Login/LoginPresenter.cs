using System.Threading.Tasks;
using Core.Properties;
using Acr.UserDialogs;
using System;

#nullable enable
namespace NativeVyatka
{
    public interface ILoginObserver
    {
        void UpdateProgress(bool value);
        void UpdateValidation(ValidationViews id, string? message);
    }

    public interface ILoginPresenter: IDisposable
    {
        void AddObserver(ILoginObserver observer);
        void RemoveObserver(ILoginObserver observer);
        void Login(string email, string password = null);
        void TryAutoLogin();
    }

    public class LoginPresenter : ILoginPresenter
    {
        private readonly ISignInValidator signInValidator;
        private readonly ILoginNetworkProvider loginDataProvider;
        private readonly ISettingsProvider settingsProvider;
        private readonly IRouter router;
        private readonly IUserDialogs dialogs;
        private readonly IPermissionsProvider permissions;
        private readonly IGeolocationService geolocationProvider;
        private ILoginObserver? loginObserver;

        public LoginPresenter(
            ILoginNetworkProvider loginDataProvider,
            ISignInValidator signInValidator,
            ISettingsProvider settingsProvider,
            IRouter router,
            IUserDialogs dialogs,
            IPermissionsProvider permissions,
            IGeolocationService geolocationProvider) {
            this.loginDataProvider = loginDataProvider;
            this.signInValidator = signInValidator;
            this.settingsProvider = settingsProvider;
            this.router = router;
            this.dialogs = dialogs;
            this.permissions = permissions;
            this.geolocationProvider = geolocationProvider;
        }


        public void AddObserver(ILoginObserver observer) {
            this.loginObserver = observer;
        }

        public void RemoveObserver(ILoginObserver observer) {
            this.loginObserver = null;
        }

        public void Dispose() {
            loginDataProvider.Cancel();
        }

        public async void TryAutoLogin() {
            if (string.IsNullOrEmpty(settingsProvider.SessionId) == false) {
                try {
                    loginObserver?.UpdateProgress(true);
                    await StartGpsMonitoringAsync();
                    await loginDataProvider.SiginAsync();       
                }
                catch (AuthorizationSyncException) {
                    await dialogs.AlertAsync(Resources.Authorization_SigninFailed, Resources.Dialog_Attention);
                }
                finally {
                    router.OpenMainScreen();
                    loginObserver?.UpdateProgress(false);
                }
            }
        }

        public async void Login(string email, string password) {
            try {
                loginObserver?.UpdateProgress(true);
                loginObserver?.UpdateValidation(ValidationViews.clear, null);
                await StartGpsMonitoringAsync();
                signInValidator.VerifyEmailAndPassword(email, password);
                await loginDataProvider.LoginAsync(email, password);
                router.OpenMainScreen();
                loginObserver?.UpdateProgress(false);
            }
            catch (NotValidLoginOrPasswordException ex) {
                if (!string.IsNullOrEmpty(ex.EmailMessage)) {
                   loginObserver?.UpdateValidation(ValidationViews.login, ex.EmailMessage);
                }
                if (!string.IsNullOrEmpty(ex.PasswordMessage)) {
                    loginObserver?.UpdateValidation(ValidationViews.password, ex.PasswordMessage);
                }
                loginObserver?.UpdateProgress(false);
            }
            catch (AuthorizationSyncException) {
                await dialogs.AlertAsync(Resources.Authorization_LoginFailed, Resources.Dialog_Attention);
                loginObserver?.UpdateProgress(false);
            }
        }

        private async Task StartGpsMonitoringAsync() {
            try {
                await permissions.RequestPermissionAsync();
                await geolocationProvider.StartGpsMonitoring();
            }
            catch (PermissionsException) {
                await dialogs.AlertAsync(Resources.Authorization_PermissionsFailed, Resources.Dialog_Attention);
            }
        }
    }

    public enum ValidationViews
    {
        clear,
        login,
        password
    }
}
#nullable restore