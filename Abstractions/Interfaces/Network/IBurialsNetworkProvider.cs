using Abstractions.Models.AppModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network
{
    public interface IBurialsNetworkProvider
    {
        Task UploadBurialAsync(BurialModel burial);
        Task SynchronizeBurialsAsync(IEnumerable<BurialModel> burials);
    }
}
