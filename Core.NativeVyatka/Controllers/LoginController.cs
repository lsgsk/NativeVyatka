using System.Threading.Tasks;
using Abstractions.Interfaces.Controllers;
using Abstractions.Interfaces.Settings;
using Abstractions.Interfaces.Utilities.Validators;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Plugins;
using Abstractions.Models;
using Abstractions.Exceptions;
using NativeVyatkaCore.Properties;
using Acr.UserDialogs;
using Plugin.Media.Abstractions;

namespace NativeVyatkaCore.Controllers
{
    public class LoginController : BaseController, ILoginController
    {
        public LoginController(ILoginNetworkProvider loginDataProvider, ISignInValidator signInValidator, ISettingsProvider settingsProvider, ICrossPageNavigator navigator, IUserDialogs dialogs, IMedia media) : base(dialogs, media)
        {
            this.mLoginDataProvider = loginDataProvider;
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
            if (!string.IsNullOrEmpty(mSettingsProvider.SessionId))
            {
                try
                {
                    Progress = true;
                    await mLoginDataProvider.SiginAsync();
                    //TODO тут отправить, всех, кто не синхранизирован
                    mNavigator.GoToPage(PageStates.BulialListPage);
                    Progress = false;
                }
                catch (AuthorizationSyncException)
                {
                    Progress = false;
                    await AlertAsync(Resources.Authorization_SigninFailed, Resources.Dialog_Attention);
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
                await AlertAsync(Resources.Authorization_LoginFailed, Resources.Dialog_Attention);
            }           
        }

        private readonly ISignInValidator mSignInValidator;
        private readonly ILoginNetworkProvider mLoginDataProvider;
        private readonly ISettingsProvider mSettingsProvider;
        private readonly ICrossPageNavigator mNavigator;
    }
}
