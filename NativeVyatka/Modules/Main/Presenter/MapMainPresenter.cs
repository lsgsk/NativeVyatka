using System.Collections.Generic;

#nullable enable
namespace NativeVyatka
{
    public interface IMapObserver
    {
        void OnRecordsLoaded(List<BurialModel> burials);
    }

    public interface IMainMapPresenter: IObservable
    {
        void DisplayBurialsOnMap();
        void DisplayBurial(BurialModel burial);
    }

    public partial class MainPresenter: IMainMapPresenter
    {
        private IMapObserver? mapObserver;

        public void AddObserver(IMapObserver observer) {
            this.mapObserver = observer;
        }

        public void RemoveObserver(IMapObserver observer) {
            this.mapObserver = null;
        }

        public void DisplayBurialsOnMap() {
            this.mapObserver?.OnRecordsLoaded(bStorage.GetBurials());
        }
    }
}
#nullable restore