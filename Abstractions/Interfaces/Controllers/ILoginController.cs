using System.Threading.Tasks;

namespace Abstractions.Interfaces.Controllers
{
    public interface ILoginController : IBaseController
    {
        Task TryAutoLogin();
        Task Login(string email, string password);
    }
}
