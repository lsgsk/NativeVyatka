using Abstractions.Exceptions;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Models.Network.ServiceEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Abstractions.Interfaces.Settings;
using NativeVyatkaCore.Utilities;
using System.Net.Http;


namespace NativeVyatkaCore.Network.RestClients
{
    public class LoginRestClient : ILoginRestClient
    {
        public LoginRestClient(ISettingsProvider settings, IHttpClientFactory factory)
        {
            this.settings = settings;
            this.factory = factory;
        }

        public async Task<LoginApiProfile> LoginAsync(string login, string password)
        {
            try
            {
                using (var client = factory.GetClient())
                {
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
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new LoginException();
            }
        }

        public async Task<SigninApiProfile> SiginAsync()
        {
            try
            {
                using (var client = factory.GetAuthClient())
                {
                    var response = await client.PostAsync("/rv_burial/system/connect.json", null, Cancel.Token);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    return ParceSigninJson(json);
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new LoginException();
            }
        }

        private LoginApiProfile ParceLoginJson(string json)
        {
            return JsonConvert.DeserializeObject<LoginApiProfile>(json);
        }

        private SigninApiProfile ParceSigninJson(string json)
        {
            return JsonConvert.DeserializeObject<SigninApiProfile>(json);
        }

        private readonly ISettingsProvider settings;
        private readonly IHttpClientFactory factory;
        public CancellationTokenSource Cancel { get; set; } = new CancellationTokenSource();
    }
}
