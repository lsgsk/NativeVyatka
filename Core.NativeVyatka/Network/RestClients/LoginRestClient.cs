using Abstractions.Exceptions;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Models.Network.ServiceEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Abstractions.Interfaces.Settings;
using NativeVyatkaCore.Utilities;

namespace NativeVyatkaCore.Network.RestClients
{
    public class LoginRestClient : ILoginRestClient
    {
        public LoginRestClient(ISettingsProvider settings)
        {
            this.mSettings = settings;
        }

        public async Task<ApiProfile> LoginAsync(string login, string password)
        {
            try
            {
                using (var httpClient = new HttpClient() { BaseAddress = new Uri(mSettings.ServiceUrl) })
                {
                    var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("username", login),
                            new KeyValuePair<string, string>("password", password),
                        });
                    var response = await httpClient.PostAsync($"/rv_burial/user/login.json", content, Cancel.Token);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    return ParceJson(json);
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new LoginLoadException();
            }
        }

        public async Task<ApiProfile> SiginAsync()
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
                    return ParceJson(json);                    
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new SigninLoadException();
            }
        }

        private ApiProfile ParceJson(string json)
        {
            return JsonConvert.DeserializeObject<ApiProfile>(json);
        }

        private readonly ISettingsProvider mSettings;
        public CancellationTokenSource Cancel { get; set; } = new CancellationTokenSource();
    }
}
