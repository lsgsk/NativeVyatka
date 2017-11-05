using Abstractions.Exceptions;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Network;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Interfaces.Settings;
using Abstractions.Models.AppModels;
using NativeVyatkaCore.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Network
{
    public class BurialsNetworkProvider : IBurialsNetworkProvider
    {
        public BurialsNetworkProvider(IBurialRestClient restClient, IBurialStorage storage, ISettingsProvider settings)
        {
            this.restClient = restClient;
            this.storage = storage;
            this.settings = settings;
        }

        public async Task UploadBurialAsync(BurialModel burial)
        {
            try
            {
                if (burial != null && burial != BurialModel.Null)
                {
                    if(burial.Uploaded)
                    {
                        await restClient.UpdateBurialAsync(burial);
                    }
                    else
                    {
                        await restClient.UploadNewBurialAsync(burial);
                        burial.Uploaded = true;
                    }                    
                    burial.Updated = true;
                    storage.InsertOrUpdateBurial(burial);
                }
            }
            catch (BurialUploadException)
            {
                throw new BurialSyncException();
            }
        }

        public async Task SynchronizeBurialsAsync()
        {
            try
            {
                var burials = storage.GetNotSyncBurials();
                foreach (var burial in burials ?? Enumerable.Empty<BurialModel>())
                {
                    try
                    {
                        await restClient.UploadNewBurialAsync(burial);
                        burial.Updated = true;
                        storage.InsertOrUpdateBurial(burial);
                    }
                    catch(BurialUploadException)
                    {
                    }
                }

                foreach (var burial in await restClient.DownloadBurialsAsync(0/*settings.LastSynchronization*/) ?? Enumerable.Empty<BurialModel>())
                {
                    if (burial.Status == BurialModel.BurialStatus.ToRemove)
                    {
                        storage.DeleteBurial(burial.CloudId);
                    }
                    else
                    {
                        storage.InsertOrUpdateBurial(burial);
                    }
                }
                settings.LastSynchronization = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
            catch (BurialUploadException)
            {
                throw new BurialSyncException();
            }
            catch(Exception ex)
            {
                iConsole.Error(ex);
                throw new BurialSyncException();
            }
        }
        private readonly IBurialRestClient restClient;
        private readonly IBurialStorage storage;
        private readonly ISettingsProvider settings;
    }
}
