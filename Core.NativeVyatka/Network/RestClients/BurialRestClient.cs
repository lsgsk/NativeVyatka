using Abstractions;
using Abstractions.Exceptions;
using Abstractions.Interfaces.Network.RestClients;
using Abstractions.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Network.RestClients
{
    public class BurialRestClient :  IBurialRestClient
    {
        public BurialRestClient(IBurialImageGuide guide)
        {
            this.mGuide = guide;
        }

        public async Task UploadBurialsAsync(IEnumerable<BurialModel> burials)
        {
            var rd = new Random();
            if (rd.Next() % 3 == 0)
            {
                throw new BurialUploadException();
            }
            else
            {
                await Task.Delay(1000);
                var request = new BurialCollection()
                {
                    Colllection = new List<ApiBurial>() //{ await burial.ToApiBurial(mGuide) }
                };
            }
        }
        public CancellationTokenSource Cancel { get; set; } = new CancellationTokenSource();
        private readonly IBurialImageGuide mGuide;
    }
}
