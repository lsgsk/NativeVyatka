using Abstractions;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Database.Tables;
using Abstractions.Interfaces.Network;
using Abstractions.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Network
{
    public class BurialsNetworkProvider : IBurialsNetworkProvider
    {
        public BurialsNetworkProvider(IBurialStorage storage, IBurialImageGuide guide)
        {
            this.mStorage = storage;
            this.mGuide = guide;
        }

        public Task DownloadBurialsAsync()
        {
            return Task.Delay(500);
        }

        public async Task UploadBurial(BurialModel burial)
        {
            var rd = new Random();
            if (rd.Next() % 3 == 0)
            {
                throw new BurialSyncException();
            }
            else
            {
                await Task.Delay(1000);
                var request = new BurialCollection()
                {
                    Colllection = new List<ApiBurial>() { await burial.ToApiBurial(mGuide) }
                };
                burial.Updated = true;
                mStorage.InsertOrUpdateBurial(burial);
            }
        }

        private readonly IBurialStorage mStorage;
        private readonly IBurialImageGuide mGuide;
    }
}
