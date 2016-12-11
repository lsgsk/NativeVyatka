using System.Threading;

namespace Abstractions.Interfaces.Network.RestClients
{
    public interface IRestClient
    {
        CancellationTokenSource Cancel { get;set; }
    }
}
