using System.Net.Http;
using System.Threading;

namespace Abstractions.Interfaces.Network.RestClients
{
    public interface IHttpClientFactory
    {
        HttpClient GetClient();
        HttpClient GetAuthClient();
    }

    public interface IRestClient
    {
        CancellationTokenSource Cancel { get;set; }
    }
}
