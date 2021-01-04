using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Properties;
using FluentResults;
using System.Linq;
using System;

#nullable enable
namespace NativeVyatka
{
    public interface IRecordsObserver
    {
        void OnRecordsLoaded(List<BurialModel> burials);
        void OnRecordsFailed(string message);
        Func<BurialModel, bool> FilterPredicate { get; }
    }

    public interface IMainRecordsPresenter : IObservable
    {
        void DisplayBurials();
        void DisplayBurial(BurialModel burial);
        void SyncBurials();
    }

    public partial class MainPresenter : IMainRecordsPresenter
    {
        private static readonly object recordsLocker = new object();
        private IRecordsObserver? recordObserver;
        private Result<List<BurialModel>>? burialsResult;
        private Task? burialsTask;

        public void AddObserver(IRecordsObserver observer) {
            lock (recordsLocker) {
                this.recordObserver = observer;
            }
        }

        public void RemoveObserver(IRecordsObserver observer) {
            lock (recordsLocker) {
                this.recordObserver = null;
            }
        }

        public void DisplayBurials() {
            lock (recordsLocker) {
                if (burialsResult is not null) {
                    Update(burialsResult);
                }
                else if (burialsTask is null) {
                    burialsTask = Task.Run(async () => {
                        Func<Result<List<BurialModel>>> loadsResalts = () => {
                            var burials = bStorage.GetBurials();
                            if (burials.Count == 0) {
                                return Result.Fail(new Error());
                            }
                            else {
                                return Result.Ok(burials);
                            }
                        };
                        try {
                            await permissions.RequestPermissionAsync();
                            await geolocationProvider.StartGpsMonitoring();
                            await burialsNetworkProvider.SynchronizeBurialsAsync();
                            return loadsResalts();
                        }
                        catch (PermissionsException) {
                            if (await dialogs.ConfirmAsync(Resources.Authorization_PermissionsFailedWithSettings, Resources.Dialog_Attention, Resources.Dialog_Settings, Resources.Dialog_Cancel)) {
                                lock (recordsLocker) {
                                    mainObserver?.OpenSettiongs();
                                }
                            }
                            return loadsResalts();
                        }
                        catch (BurialSyncException) {
                            await dialogs.AlertAsync(Resources.MainScreeen_SyncFailed, Resources.Dialog_Attention);
                            return loadsResalts();
                        }

                    }).ContinueWith(result => {
                        lock (recordsLocker) {
                            this.burialsResult = result.Result;
                            this.Update(this.burialsResult);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void Update(Result<List<BurialModel>> result) {
            if (result.IsSuccess) {
                var burials = result.Value;
                if (recordObserver is not null) {
                    burials = burials.Where(recordObserver.FilterPredicate).ToList();
                };
                if (burials.Count == 0) {
                    this.recordObserver?.OnRecordsFailed("Нет ни одной записи записи");
                }
                else {
                    this.recordObserver?.OnRecordsLoaded(burials);
                }
            }
            else {
                this.recordObserver?.OnRecordsFailed("Нет ни одной записи записи");
            }
        }

        public void DisplayBurial(BurialModel burial) {
            router.OpenBurialEditScreen(burial, () => {
                lock (recordsLocker) {
                    this.burialsResult = Result.Ok(bStorage.GetBurials());
                    this.Update(this.burialsResult);
                }
            });
        }

        public void SyncBurials() {
            lock (recordsLocker) {
                if ((burialsTask == null) || (burialsTask?.IsCompleted ?? true)) {
                    burialsResult = null;
                    burialsTask = null;
                    DisplayBurials();
                }
            }
        }
    }
}
#nullable restore