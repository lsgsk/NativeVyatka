using Abstractions.Models.AppModels;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network
{
    public interface IBurialsDataProvider
    {
        Task DownloadBurialsAsync();
        Task UploadBurial(BurialModel burial);
    }
}
