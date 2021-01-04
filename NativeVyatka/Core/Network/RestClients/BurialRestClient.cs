using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NativeVyatka
{
    public interface IBurialRestClient : IRestClient
    {
        Task UploadNewBurialAsync(BurialModel burial);
        Task UpdateBurialAsync(BurialModel burial);
        Task<IEnumerable<BurialModel>> DownloadBurialsAsync(int lastSynchronization, string userHash);
    }

    public class BurialRestClient : IBurialRestClient
    {
        public CancellationTokenSource Cancel { get; set; } = new CancellationTokenSource();
        private readonly IApiBurialConverter converter;
        private readonly IHttpClientFactory factory;

        public BurialRestClient(IApiBurialConverter converter, IHttpClientFactory factory) {
            this.converter = converter;
            this.factory = factory;
        }

        public async Task UploadNewBurialAsync(BurialModel burial) {
            try {
                using var client = factory.GetAuthClient();
                var json = await converter.Serialize(burial);
                var response = await client.PostAsync("/rv_burial/burial/create.json", new StringContent(json, Encoding.UTF8, "application/json"), Cancel.Token);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                throw new BurialUploadException();
            }
        }

        public async Task UpdateBurialAsync(BurialModel burial) {
            try {
                using var client = factory.GetAuthClient();
                var json = await converter.Serialize(burial);
                var response = await client.PostAsync("/rv_burial/burial/update.json", new StringContent(json, Encoding.UTF8, "application/json"), Cancel.Token);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                throw new BurialUploadException();
            }
        }

        public async Task<IEnumerable<BurialModel>> DownloadBurialsAsync(int lastSynchronization, string userHash) {
            try {
                using var client = factory.GetAuthClient();
                var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("LastSynchronization", lastSynchronization.ToString()) });
                HttpResponseMessage response = await client.PostAsync("/rv_burial/burial/index.json", content, Cancel.Token);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                iConsole.WriteLine(string.Format("DownloadBurialsAsync: {0}", json));
                return converter.ParceJson(json, userHash);
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                throw new BurialLoadException();
            }
        }
    }
}
