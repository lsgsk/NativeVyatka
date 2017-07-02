using Abstractions.Models.AppModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network.RestClients
{
    public interface IBurialRestClient : IRestClient
    {
        Task UploadBurialAsync(BurialModel burial);
        Task<IEnumerable<BurialModel>> DownloadBurialsAsync();
    }
}
