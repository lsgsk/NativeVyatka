using System;
using System.Net.Http;

namespace NativeVyatka
{
    public interface IHttpClientFactory
    {
        HttpClient GetClient();
        HttpClient GetAuthClient();
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly ISettingsProvider settings;

        public HttpClientFactory(ISettingsProvider settings) {
            this.settings = settings;
        }

        public HttpClient GetClient() {
            return new HttpClient() { BaseAddress = new Uri(settings.ServiceUrl), Timeout = TimeSpan.FromSeconds(15) };
        }

        public HttpClient GetAuthClient() {
            var client = new HttpClient() { BaseAddress = new Uri(settings.ServiceUrl), Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.Add("Cookie", $"{settings.SessionName}={settings.SessionId}");
            client.DefaultRequestHeaders.Add("X-CSRF-Token", settings.CsrfToken);
            return client;
        }
    }
}
