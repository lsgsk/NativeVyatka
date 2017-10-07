using Abstractions.Models.Network.ServiceEntities;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network.RestClients
{
    public interface ILoginRestClient : IRestClient
    {
        Task<LoginApiProfile> LoginAsync(string login, string password);
        Task<SigninApiProfile> SiginAsync();
    }
}
