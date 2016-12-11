using System.Threading.Tasks;

namespace Abstractions.Interfaces.Network
{
    public interface ILoginDataProvider : IDataProvider
    {
        Task LoginProfileAsync(string login, string password);
        Task SiginProfileAsync();
    }
}
