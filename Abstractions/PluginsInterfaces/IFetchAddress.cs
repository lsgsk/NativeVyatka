using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface IFetchAddress
    {
        Task<string> GetAdress(double latitude, double longitude, CancellationToken? token = null);
    }
}

