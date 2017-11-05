using Abstractions.Models.AppModels;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network
{
    public interface IBurialsNetworkProvider
    {
        Task UploadBurialAsync(BurialModel burial);
        Task SynchronizeBurialsAsync();
    }
}
