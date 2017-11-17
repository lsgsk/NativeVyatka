using Abstractions.Interfaces.Network;
using System.Threading.Tasks;
using Abstractions.Interfaces.Settings;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Models.AppModels;
using Abstractions.Models.Network.ServiceEntities;
using Abstractions.Interfaces.Utilities;

namespace NativeVyatkaCore.Network
{
    public class LoginNetworkProvider : ILoginNetworkProvider
    {
        public LoginNetworkProvider(ILoginRestClient restClient, ISettingsProvider settingsProvider, IProfileStorage pstorage, IMd5HashGenerator hashGenerator )
        {
            this.restClient = restClient;
            this.settings = settingsProvider;
            this.pStorage = pstorage;
            this.hashGenerator = hashGenerator;
        }

        public async Task LoginAsync(string login, string password)
        {
            try
            {
                var user = await restClient.LoginAsync(login, password);
                UpdateSessionAndProfile(login, user);
            }
            catch(LoginLoadException)
            {
                pStorage.ClearProfile();
                throw new AuthorizationSyncException();
            }
        }

        public async Task SiginAsync()
        {
            try
            {
                var user = await restClient.SiginAsync();
                UpdateSession(user);
            }            
            catch (SigninLoadException)
            {
                throw new AuthorizationSyncException();
            }
        }

        private void UpdateSessionAndProfile(string login, LoginApiProfile value)
        {
            settings.UserHash = hashGenerator.GenerateHash(login);
            settings.CsrfToken = value.token;
            settings.SessionName = value.session_name;
            settings.SessionId = value.sessid;
            pStorage.SaveProfile(new ProfileModel(value.user));
        }

        private void UpdateSession(SigninApiProfile value)
        {
            settings.SessionName = value.session_name;
            settings.SessionId = value.sessid;
        }

        public void Cancel()
        {
            restClient.Cancel.Cancel();
        }        

        private readonly ILoginRestClient restClient;
        private readonly ISettingsProvider settings;
        private readonly IProfileStorage pStorage;
        private readonly IMd5HashGenerator hashGenerator;
    }
}
