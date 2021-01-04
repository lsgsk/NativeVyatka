using System.Threading;

namespace NativeVyatka
{
    public interface IRestClient
    {
        CancellationTokenSource Cancel { get; set; }
    }
}
