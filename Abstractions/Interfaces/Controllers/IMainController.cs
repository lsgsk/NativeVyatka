using Abstractions.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Controllers
{
    public interface IMainController : IMainRecordsController, IBaseController
    {
        Task CreateNewBurial();
        event EventHandler<ProfileModel> ProfileReady;
        event EventHandler<List<BurialModel>> BurialsReady;
    }

    public interface IMainRecordsController
    {
        List<BurialModel> GetBurials();
        void DisplayBurial(BurialModel burial);
    }
}
