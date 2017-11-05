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
        public LoginRestClient(ISettingsProvider settings)
        {
            this.mSettings = settings;
        }

        public async Task<LoginApiProfile> LoginAsync(string login, string password)
        {
            try
            {
                using (var client = new HttpClient() { BaseAddress = new Uri(mSettings.ServiceUrl) })
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
                throw new LoginLoadException();
            }
        }

        public async Task<SigninApiProfile> SiginAsync()
        {
            try
            {
                using (var client = new HttpClient() { BaseAddress = new Uri(mSettings.ServiceUrl) })
                {
                    client.DefaultRequestHeaders.Add("Cookie", $"{mSettings.SessionName}={mSettings.SessionId}");
                    client.DefaultRequestHeaders.Add("X-CSRF-Token", mSettings.CsrfToken);
                    var response = await client.PostAsync("/rv_burial/system/connect.json", null, Cancel.Token);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    return ParceSigninJson(json);
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new SigninLoadException();
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

        private readonly ISettingsProvider mSettings;
        public CancellationTokenSource Cancel { get; set; } = new CancellationTokenSource();
    }
}
