using System.Threading.Tasks;

namespace Abstractions
{
    public interface IControllerReceiver
    {
        Task ShowNotification(string message);
        void UploadingStarted();
        void UploadingFinished(bool uploadResult);
    }

    public interface IUploaderListener
    {
        void SetControllerListener(IControllerReceiver receiver);
        bool UploadingState { get; }
        void UploadingStarted(string message);
        void UploadingFinished(bool uploadResult, string message);
    }
}

