using System.Threading.Tasks;
using FluentResults;
using System.Linq;

#nullable enable
namespace NativeVyatka
{
    public interface IProfileObserver
    {
        void OnProfileChanged(ProfileModel profile);
        void OnProfileFailed(string message);
    }

    public interface IProfilePresenter: IObservable
    {
        void DisplayProfile();
        void Logout();
    }

    public partial class MainPresenter : IProfilePresenter
    {
        private static readonly object profileLocker = new object();
        private readonly IProfileStorage pStorage;
        private IProfileObserver? profileObserver;
        private Result<ProfileModel>? profileResult;
        private Task? profileTask;

        public void AddObserver(IProfileObserver observer) {
            lock (profileLocker) {
                this.profileObserver = observer;
            }
        }

        public void RemoveObserver(IProfileObserver observer) {
            lock (profileLocker) {
                this.profileObserver = null;
            }
        }

        public void DisplayProfile() {
            lock (profileLocker) {
                if (profileResult is not null) {
                    Update(profileResult);
                }
                else if (profileTask is null) {
                    profileTask = Task.Run<Result<ProfileModel>>(() => {
                        try {
                            return Result.Ok(pStorage.GetProfile());
                        }
                        catch {
                            return Result.Fail("Не удалось загрузить профиль, попробуйте перелогиниться");
                        }
                    }).ContinueWith(result => {
                        this.profileResult = result.Result;
                        this.Update(this.profileResult);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void Update(Result<ProfileModel> result) {
            if (result.IsSuccess) {
                this.profileObserver?.OnProfileChanged(result.Value);
            }
            else {
                this.profileObserver?.OnProfileFailed(result.Errors.FirstOrDefault()?.Message ?? "");
            }
        }

        public void Logout() {
            settings.ClearPrefs();
            router.OpenLoginScreen();
        }
    }
}
#nullable restore
