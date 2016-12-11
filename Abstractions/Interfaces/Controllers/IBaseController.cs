using System;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Controllers
{
    public interface IBaseController :IDisposable
    {
        bool Progress { set; }
    }
}
