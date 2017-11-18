using Abstractions.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions.Interfaces.Controllers
{
    public interface IMainController : IMainRecordsController, IMainMapController, IBaseController
    {
        void Logout();
        Task CreateNewBurial();
        ProfileModel Profile { get; }
        event EventHandler<GpsState> GpsEnableChanged;
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

    public class GpsState : EventArgs
    {
        public GpsState(int satetiles, double accuracy)
        {
            this.Satetiles = satetiles;
            this.Accuracy = accuracy;
        }
        public int Satetiles { get; }
        public double Accuracy { get; }
    }
}
