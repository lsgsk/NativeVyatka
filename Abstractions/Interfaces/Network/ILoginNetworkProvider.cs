using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network
{
    public interface ILoginNetworkProvider : INetworkProvider
    {
        Task LoginAsync(string login, string password);
        Task SiginAsync();
    }
}
