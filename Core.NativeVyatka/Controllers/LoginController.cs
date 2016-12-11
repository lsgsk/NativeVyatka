using System.Threading.Tasks;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Settings;
using Abstractions.Interfaces.Utilities.Validators;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Plugins;
using Abstractions.Models;
using Abstractions.Exceptions;
using System;

namespace NativeVyatkaCore.Controllers
{
    public class LoginController : BaseController, ILoginController
    {
        public LoginController(ILoginDataProvider loginDataProvider, IBurialsDataProvider burialsDataProvider, ISignInValidator signInValidator, ISettingsProvider settingsProvider, ICrossPageNavigator navigator)
        {
            this.mLoginDataProvider = loginDataProvider;
            this.mBurialsDataProvider = burialsDataProvider;
            this.mSignInValidator = signInValidator;
            this.mSettingsProvider = settingsProvider;
            this.mNavigator = navigator;
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
                    await mLoginDataProvider.SiginProfileAsync();
                    await mBurialsDataProvider.DownloadBurialsAsync();
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
                Progress = true;
                mSignInValidator.VerifyEmailAndPassword(email, password);
                await mLoginDataProvider.LoginProfileAsync(email, password);
                await mBurialsDataProvider.DownloadBurialsAsync();
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
        private readonly ILoginDataProvider mLoginDataProvider;
        private readonly IBurialsDataProvider mBurialsDataProvider;
        private readonly ISettingsProvider mSettingsProvider;
        private readonly ICrossPageNavigator mNavigator;
    }
}
