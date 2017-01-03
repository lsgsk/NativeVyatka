using System.Threading.Tasks;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Settings;
using Abstractions.Interfaces.Utilities.Validators;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Plugins;
using Abstractions.Models;
using Abstractions.Exceptions;

namespace NativeVyatkaCore.Controllers
{
    public class LoginController : BaseController, ILoginController
    {
        public LoginController(ILoginNetworkProvider loginDataProvider, IBurialsNetworkProvider burialsDataProvider, ISignInValidator signInValidator, ISessionSettings settingsProvider, ICrossPageNavigator navigator)
        {
            this.mLoginDataProvider = loginDataProvider;
            this.mBurialsDataProvider = burialsDataProvider;
            this.mSignInValidator = signInValidator;
            this.mSettingsProvider = settingsProvider;
            this.mNavigator = navigator;
        }

        public override void Dispose()
        {
            base.Dispose();
            mLoginDataProvider.Cancel();
        }

        public async void TryAutoLogin()
        {
            await AutoLoginIfPossible();
        }

        private async Task AutoLoginIfPossible()
        {
            if (!string.IsNullOrEmpty(mSettingsProvider.SessionId))
            {
                try
                {
                    Progress = true;
                    await mLoginDataProvider.SiginAsync();
                    //тут отправить, всех, кто не синхранизирован
                    mNavigator.GoToPage(PageStates.BulialListPage);
                    Progress = false;
                }
                catch (AuthorizationSyncException)
                {
                    Progress = false;
                    await AlertAsync("Не удалось выполнить вход");
                }
            }
        }

        public async Task Login(string email, string password)
        {
            try
            {
                mSignInValidator.VerifyEmailAndPassword(email, password);
                Progress = true;                
                await mLoginDataProvider.LoginAsync(email, password);
                mNavigator.GoToPage(PageStates.BulialListPage);
                Progress = false;
            }
            catch(NotValidLoginOrPasswordException)
            {
                throw;
            }
            catch(AuthorizationSyncException)
            {
                Progress = false;
                await AlertAsync("Что-то пошло не так");
            }           
        }

        private readonly ISignInValidator mSignInValidator;
        private readonly ILoginNetworkProvider mLoginDataProvider;
        private readonly IBurialsNetworkProvider mBurialsDataProvider;
        private readonly ISessionSettings mSettingsProvider;
        private readonly ICrossPageNavigator mNavigator;
    }
}
