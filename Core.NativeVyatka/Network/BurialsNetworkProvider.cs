﻿using Abstractions.Exceptions;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Models.AppModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Network
{
    public class BurialsNetworkProvider : IBurialsNetworkProvider
    {
        public BurialsNetworkProvider(IBurialRestClient restClient, IBurialStorage storage)
        {
            this.mRestClient = restClient;
            this.mStorage = storage;
        }

        public async Task UploadBurialsAsync(IEnumerable<BurialModel> burials)
        {
            try
            {
                foreach (var burial in burials ?? Enumerable.Empty<BurialModel>())
                {
                    await mRestClient.UploadBurialAsync(burial);
                    burial.Updated = true;
                    mStorage.InsertOrUpdateBurial(burial);
                }
            }
            catch (BurialUploadException)
            {
                throw new BurialSyncException();
            }
        }
        private readonly IBurialRestClient mRestClient;
        private readonly IBurialStorage mStorage;
    }
}
