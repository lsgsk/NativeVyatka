using System.Threading.Tasks;

namespace NativeVyatka
{
    public interface ILoginNetworkProvider : INetworkProvider
    {
        Task LoginAsync(string login, string password);
        Task SiginAsync();
    }

    public class LoginNetworkProvider : ILoginNetworkProvider
    {
        private readonly ILoginRestClient restClient;
        private readonly ISettingsProvider settings;
        private readonly IProfileStorage pStorage;
        private readonly IMd5HashGenerator hashGenerator;

        public LoginNetworkProvider(ILoginRestClient restClient, ISettingsProvider settingsProvider, IProfileStorage pstorage, IMd5HashGenerator hashGenerator) {
            this.restClient = restClient;
            this.settings = settingsProvider;
            this.pStorage = pstorage;
            this.hashGenerator = hashGenerator;
        }

        public async Task LoginAsync(string login, string password) {
            try {
                var user = await restClient.LoginAsync(login, password);
                await UpdateSessionAndProfile(login, password, user);
            }
            catch (LoginException) {
                throw new AuthorizationSyncException();
            }
        }

        public async Task SiginAsync() {
            try {
                var profile = await restClient.SiginAsync();
                ValidateProfile(profile);
                var token = await restClient.TokenAsync();
                UpdateSession(profile, token);
            }
            catch (SessionException) {
                await LoginAsync(await settings.GetLoginAsync(), await settings.GetPasswordAsync());
            }
            catch (LoginException) {
                await LoginAsync(await settings.GetLoginAsync(), await settings.GetPasswordAsync());
            }
        }

        private async Task UpdateSessionAndProfile(string login, string password, LoginApiProfile value) {
            await settings.SetLoginAsync(login);
            await settings.SetPasswordAsync(password);
            settings.UserHash = hashGenerator.GenerateHash(login);
            settings.CsrfToken = value.token;
            settings.SessionName = value.session_name;
            settings.SessionId = value.sessid;
            pStorage.SaveProfile(new ProfileModel(value.user));
        }

        private void UpdateSession(SigninApiProfile profile, TokenApi token) {
            settings.SessionName = profile.session_name;
            settings.SessionId = profile.sessid;
            settings.CsrfToken = token.Token;
        }

        private void ValidateProfile(SigninApiProfile value) {
            if (value?.user?.roles?.Name == "anonymous user") {
                throw new SessionException();
            }
        }

        public void Cancel() {
            restClient.Cancel.Cancel();
        }
    }
}