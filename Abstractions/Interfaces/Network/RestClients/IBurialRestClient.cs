using Abstractions.Models.AppModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network.RestClients
{
    public interface IBurialRestClient : IRestClient
    {
        Task UploadNewBurialAsync(BurialModel burial);
        Task UpdateBurialAsync(BurialModel burial);
        Task<IEnumerable<BurialModel>> DownloadBurialsAsync(int lastSynchronization, string userHash);
    }
}
