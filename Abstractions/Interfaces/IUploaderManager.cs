namespace Abstractions
{
    public interface IUploaderManager
    {
        bool  UploadingState { get; }
        void UploadingStarted();
        void UploadingFinished(bool uploadResult, string message);
    }
}

