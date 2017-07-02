using Abstractions.Interfaces.Network;
using System.Threading.Tasks;
using Abstractions.Interfaces.Settings;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Models.AppModels;
using Abstractions.Models.Network.ServiceEntities;

namespace NativeVyatkaCore.Network
{
    public class LoginNetworkProvider : ILoginNetworkProvider
    {
        public LoginNetworkProvider(ILoginRestClient restClient, ISettingsProvider settingsProvider, IProfileStorage pstorage)
        {
            this.mRestClient = restClient;
            this.mSettingsProvider = settingsProvider;
            this.pStorage = pstorage;
        }

        public async Task LoginAsync(string login, string password)
        {
            try
            {
                var user = await mRestClient.LoginAsync(login, password);
                UpdateSessionAndProfile(user);
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
                var user = await mRestClient.SiginAsync();
                UpdateSessionAndProfile(user);
            }            
            catch (SigninLoadException)
            {
                pStorage.ClearProfile();
                throw new AuthorizationSyncException();
            }
        }

        private void UpdateSessionAndProfile(ApiProfile value)
        {
            mSettingsProvider.CsrfToken = value.token;
            mSettingsProvider.SessionName = value.session_name;
            mSettingsProvider.SessionId = value.sessid;
            pStorage.SaveProfile(new ProfileModel(value.user));
        }

        public void Cancel()
        {
            mRestClient.Cancel.Cancel();
        }        

        private readonly ILoginRestClient mRestClient;
        private readonly ISettingsProvider mSettingsProvider;
        private readonly IProfileStorage pStorage;      
    }
}
