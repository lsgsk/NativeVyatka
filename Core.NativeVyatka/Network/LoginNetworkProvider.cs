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
        public LoginNetworkProvider(ILoginRestClient restClient, ISessionSettings settingsProvider, IProfileStorage pstorage)
        {
            this.mRestClient = restClient;
            this.mSettingsProvider = settingsProvider;
            this.mpStorage = pstorage;
        }

        public async Task LoginAsync(string login, string password)
        {
            try
            {
                var result = await mRestClient.LoginAsync(login, password, mSettingsProvider.PushToken);
                UpdateSessionAndProfile(result);
            }
            catch(LoginLoadException)
            {
                throw new AuthorizationSyncException();
            }
        }

        public async Task SiginAsync()
        {
            try
            {
                await mRestClient.SiginAsync(mSettingsProvider.PushToken);
            }            
            catch (SigninLoadException)
            {
                throw new AuthorizationSyncException();
            }
        }

        private void UpdateSessionAndProfile(ApiProfile value)
        {
            mSettingsProvider.CsrfToken = value.Token;
            mSettingsProvider.SessionName = value.Session_name;
            mSettingsProvider.SessionId = value.Sessid;
            mpStorage.SaveProfile(new ProfileModel(value.User));
        }

        public void Cancel()
        {
            mRestClient.Cancel.Cancel();
        }        

        private readonly ILoginRestClient mRestClient;
        private readonly ISessionSettings mSettingsProvider;
        private readonly IProfileStorage mpStorage;      
    }
}
