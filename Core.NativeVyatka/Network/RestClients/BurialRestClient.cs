using Abstractions.Exceptions;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Interfaces.Utilities;
using Abstractions.Models.AppModels;
using NativeVyatkaCore.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Network.RestClients
{
    public class BurialRestClient :  IBurialRestClient
    {
        public BurialRestClient(IApiBurialConverter converter, IHttpClientFactory factory)
        {
            this.mConverter = converter;
            this.mFactory = factory;
        }

        public async Task UploadBurialAsync(BurialModel burial)
        {
            try
            {
                using (var client = mFactory.GetAuthClient())
                {
                    var json = await mConverter.Serialize(burial);
                    var response = await client.PostAsync("/rv_burial/burial/create.json", new StringContent(json, Encoding.UTF8, "application/json"), Cancel.Token);
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new BurialUploadException();
            }
        }

        public async Task<IEnumerable<BurialModel>> DownloadBurialsAsync()
        {
            try
            {
                using (var client = mFactory.GetAuthClient())
                {
                    var response = await client.PostAsync("/rv_burial/burial/index.json", null, Cancel.Token);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    return mConverter.ParceJson(json);
                }
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
                throw new BurialLoadException();
            }
        }

        public CancellationTokenSource Cancel { get; set; } = new CancellationTokenSource();
        private readonly IApiBurialConverter mConverter;
        private readonly IHttpClientFactory mFactory;
    }
}
