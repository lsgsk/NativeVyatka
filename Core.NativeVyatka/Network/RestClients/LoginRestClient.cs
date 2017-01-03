using Abstractions.Exceptions;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Models.Network.ServiceEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Abstractions.Interfaces.Settings;
using NativeVyatkaCore.Utilities;

namespace NativeVyatkaCore.Network.RestClients
{
    public class LoginRestClient : ILoginRestClient
    {
        public LoginRestClient(ISessionSettings setings)
        {
            this.mSettings = setings;
        }

        public async Task<ApiProfile> LoginAsync(string login, string password, string pushtoken)
        {
            try
            {
                using (var httpClient = new HttpClient(){ BaseAddress = new Uri(mSettings.ServiceUrl)})
                {
                    var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("username", login),
                            new KeyValuePair<string, string>("password", password),
                            new KeyValuePair<string, string>("pushtoken", pushtoken)
                        });
                    var response = await httpClient.PostAsync($"{mSettings.ServiceUrl}/rv_burial/user/login.json", content, Cancel.Token);
                    response.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<ApiProfile>(await response.Content.ReadAsStringAsync());                    
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new LoginLoadException();
            }
        }

        public async Task SiginAsync(string pushToken)
        {
            try
            {
                var baseAddress = new Uri(mSettings.ServiceUrl);
                using (var clientHandler = new HttpClientHandler())
                {
                    clientHandler.CookieContainer.Add(baseAddress, new Cookie(mSettings.SessionName, mSettings.SessionId));
                    using (var client = new HttpClient(clientHandler) { BaseAddress = baseAddress })
                    {
                        client.DefaultRequestHeaders.Add("X-CSRF-Token", mSettings.CsrfToken);
                        var response = await client.PostAsync("/rv_burial/syst/connect.json", null, Cancel.Token);
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new SigninLoadException();
            }
        }
        private readonly ISessionSettings mSettings;
        public CancellationTokenSource Cancel { get; set; } = new CancellationTokenSource();
    }
}
