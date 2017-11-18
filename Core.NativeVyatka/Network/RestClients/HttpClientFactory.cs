using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Interfaces.Settings;
using System;
using System.Net.Http;

namespace NativeVyatkaCore.Network.RestClients
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClientFactory(ISettingsProvider settings)
        {
            this.settings = settings;
        }

        public HttpClient GetClient()
        {
            return new HttpClient() { BaseAddress = new Uri(settings.ServiceUrl), Timeout = TimeSpan.FromSeconds(15) };
        }

        public HttpClient GetAuthClient()
        {
            var client = new HttpClient() { BaseAddress = new Uri(settings.ServiceUrl), Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.Add("Cookie", $"{settings.SessionName}={settings.SessionId}");
            client.DefaultRequestHeaders.Add("X-CSRF-Token", settings.CsrfToken);
            return client;
        }
        private readonly ISettingsProvider settings;
    }
}
