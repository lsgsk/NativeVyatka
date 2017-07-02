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
            this.mSettings = settings;
        }

        public HttpClient GetClient()
        {
            return new HttpClient() { BaseAddress = new Uri(mSettings.ServiceUrl) };
        }

        public HttpClient GetAuthClient()
        {
            var client = new HttpClient() { BaseAddress = new Uri(mSettings.ServiceUrl) };
            client.DefaultRequestHeaders.Add("Cookie", $"{mSettings.SessionName}={mSettings.SessionId}");
            client.DefaultRequestHeaders.Add("X-CSRF-Token", mSettings.CsrfToken);
            return client;
        }
        private readonly ISettingsProvider mSettings;
    }
}
