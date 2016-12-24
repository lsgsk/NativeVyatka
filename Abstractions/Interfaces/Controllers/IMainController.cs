using Abstractions.Models.AppModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Controllers
{
    public interface IMainController : IMainRecordsController, IMainMapController, IBaseController
    {
        Task CreateNewBurial();
        ProfileModel Profile { get; }
    }

    public interface IMainRecordsController
    {
        List<BurialModel> GetBurials();
        void DisplayBurial(BurialModel burial);
        Task ForceSyncBurials();
    }

    public interface IMainMapController
    {
        List<BurialModel> GetBurials();
        void DisplayBurial(BurialModel burial);
    }
}
