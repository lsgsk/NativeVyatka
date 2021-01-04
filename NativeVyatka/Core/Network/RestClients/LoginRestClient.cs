using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;

namespace NativeVyatka
{
    public interface ILoginRestClient : IRestClient
    {
        Task<LoginApiProfile> LoginAsync(string login, string password);
        Task<SigninApiProfile> SiginAsync();
        Task<TokenApi> TokenAsync(); 
    }

    public class LoginRestClient : ILoginRestClient
    {
        private readonly IHttpClientFactory factory;
        public CancellationTokenSource Cancel { get; set; } = new CancellationTokenSource();

        public LoginRestClient(IHttpClientFactory factory) {
            this.factory = factory;
        }

        public async Task<LoginApiProfile> LoginAsync(string login, string password) {
            try {
                using var client = factory.GetClient();
                var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("username", login),
                        new KeyValuePair<string, string>("password", password),
                     });
                var response = await client.PostAsync($"/rv_burial/user/login.json", content, Cancel.Token);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return ParceLoginJson(json);
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                throw new LoginException();
            }
        }

        public async Task<SigninApiProfile> SiginAsync() {
            try {
                using var client = factory.GetAuthClient();
                var response = await client.PostAsync("/rv_burial/system/connect.json", null, Cancel.Token);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return ParceSigninJson(json);
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                throw new LoginException();
            }
        }

        public async Task<TokenApi> TokenAsync() {
            try {
                using var client = factory.GetAuthClient();
                var response = await client.PostAsync("/rv_burial/user/token.json", null, Cancel.Token);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return ParceTokenJson(json);
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                throw new LoginException();
            }
        }

        private LoginApiProfile ParceLoginJson(string json) {
            return JsonConvert.DeserializeObject<LoginApiProfile>(json);
        }

        private SigninApiProfile ParceSigninJson(string json) {
            return JsonConvert.DeserializeObject<SigninApiProfile>(json);
        }

        private TokenApi ParceTokenJson(string json) {
            return JsonConvert.DeserializeObject<TokenApi>(json);
        }
    }
}
