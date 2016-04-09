using System;
using System.Threading.Tasks;
using ServiceStack;
using Abstractions;
using System.Collections.Generic;
using Plugins;

namespace NativeVyatkaCore
{
    internal sealed class RestServiceManager
    {
        public RestServiceManager(string serviceUrl)
        {
            this.mServiceClient = new JsonServiceClient(serviceUrl) { Timeout = TimeSpan.FromSeconds(15) };
        }

        public async Task<bool> UploadNewBurials(IEnumerable<ApiBurialEntity> collection)
        {          
            try
            {
                var request = new ApiBurialEntityCollectionRequest(){ Items = collection };
                return await mServiceClient.PostAsync(request);
            }
            catch (Exception ex)
            {
                iConsole.Error(ex);
            }
            return false;
        }

        private readonly JsonServiceClient mServiceClient;
    }
}

