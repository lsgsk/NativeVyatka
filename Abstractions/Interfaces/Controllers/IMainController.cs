using Abstractions.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Controllers
{
    public interface IMainController : IMainRecordsController, IBaseController
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
}
