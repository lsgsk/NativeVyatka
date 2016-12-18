using Abstractions.Exceptions;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Network;
using Abstractions.Models.AppModels;
using System;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Network
{
    public class BurialsDataProvider : IBurialsDataProvider
    {
        public BurialsDataProvider(IBurialStorage storage)
        {
            this.mStorage = storage;
        }

        public Task DownloadBurialsAsync()
        {
            return Task.Delay(500);
        }

        public async Task UploadBurial(BurialModel burial)
        {
            var rd = new Random();
            if(rd.Next() % 3 == 0)
            {
                throw new BurialSyncException();
            }
            else
            {
                await Task.Delay(1000);
                burial.Updated = true;
                mStorage.InsertOrUpdateBurial(burial);
            }
        }

        private readonly IBurialStorage mStorage;
    }
}
