using System.Threading.Tasks;

namespace Abstractions.Interfaces.Controllers
{
    public interface ILoginController : IBaseController
    {
        void TryAutoLogin();
        Task Login(string email, string password);
    }
}
