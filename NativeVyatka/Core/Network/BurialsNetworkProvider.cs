using System;
using System.Linq;
using System.Threading.Tasks;

namespace NativeVyatka
{
    public interface IBurialsNetworkProvider
    {
        Task UploadBurialAsync(BurialModel burial);
        Task RemoveBurialAsync(BurialModel burial);
        Task SynchronizeBurialsAsync();
    }

    public class BurialsNetworkProvider : IBurialsNetworkProvider
    {
        private readonly IBurialRestClient restClient;
        private readonly IBurialStorage storage;
        private readonly ISettingsProvider settings;
        private readonly IBurialImageGuide guide;

        public BurialsNetworkProvider(IBurialRestClient restClient, IBurialStorage storage, ISettingsProvider settings, IBurialImageGuide guide) {
            this.restClient = restClient;
            this.storage = storage;
            this.settings = settings;
            this.guide = guide;
        }

        public async Task UploadBurialAsync(BurialModel burial) {
            try {
                if (burial != null && burial != BurialModel.Null) {
                    burial.Updated = false;
                    storage.InsertOrUpdateBurial(burial);
                    if (burial.Uploaded) {
                        await restClient.UpdateBurialAsync(burial);
                    }
                    else {
                        await restClient.UploadNewBurialAsync(burial);
                        burial.Uploaded = true;
                        await TryDeletePicture(burial.PicturePath);
                    }
                    burial.Updated = true;
                    storage.InsertOrUpdateBurial(burial);
                }
            }
            catch (BurialUploadException) {
                throw new BurialSyncException();
            }
        }

        public async Task SynchronizeBurialsAsync() {
            try {
                bool exception = false;
                var burials = storage.GetNotSyncBurials();
                foreach (var burial in burials ?? Enumerable.Empty<BurialModel>()) {
                    try {
                        await UploadBurialAsync(burial);
                    }
                    catch (BurialUploadException) {
                        exception = true;
                    }
                }
                foreach (var burial in await restClient.DownloadBurialsAsync(settings.LastSynchronization, settings.UserHash) ?? Enumerable.Empty<BurialModel>()) {
                    if (burial.Status == BurialModel.BurialStatus.ToRemove) {
                        var existing = storage.GetBurial(burial.CloudId);
                        if (existing != BurialModel.Null) {
                            await TryDeletePicture(existing.PicturePath);
                            storage.DeleteBurial(existing.CloudId);
                        }
                    }
                    else {
                        storage.InsertOrUpdateBurial(burial);
                    }
                }
                settings.LastSynchronization = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                if (exception) {
                    throw new BurialUploadException();
                }
            }
            catch (BurialUploadException) {
                throw new BurialSyncException();
            }
            catch (Exception ex) {
                iConsole.Error(ex);
                throw new BurialSyncException();
            }
        }

        public async Task RemoveBurialAsync(BurialModel burial) {
            try {
                if (burial.Uploaded) {
                    await Task.Delay(1000);
                    throw new BurialSyncException();
                }
                else {
                    await TryDeletePicture(burial.PicturePath);
                }
                storage.DeleteBurial(burial.CloudId);
            }
            catch (BurialRemoveException) {
                throw new BurialSyncException();
            }
        }

        private async Task TryDeletePicture(string picturePath) {
            try {
                await guide.DeleteFromFileSystemAsync(picturePath);
            }
            catch (FileGuideException) {
            }
        }
    }
}
