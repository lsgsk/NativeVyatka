using Abstractions.Interfaces.Network;
using System.Threading.Tasks;

namespace NativeVyatkaCore.Network
{
    public class BurialsDataProvider : IBurialsDataProvider
    {
        public Task DownloadBurialsAsync()
        {
            return Task.Delay(500);
        }
    }
}
