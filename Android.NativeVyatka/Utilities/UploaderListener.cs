using System;
using Abstractions;

namespace NativeVyatkaAndroid
{
    public class UploaderListener : IUploaderListener
    {
        private bool mState;
        private IControllerReceiver mReceiver;

        public UploaderListener()
        {
        }

        public void SetControllerListener(IControllerReceiver receiver)
        {
            mReceiver = receiver;
        }

        public void UploadingStarted(string message)
        {
            mState = true;
            mReceiver?.UploadingStarted();
            mReceiver?.ShowNotification(message);
        }

        public void UploadingFinished(bool uploadResult, string message)
        {
            mState = false;
            mReceiver?.UploadingFinished(uploadResult);
            mReceiver?.ShowNotification(message);
        }

        public bool UploadingState
        {
            get
            {
                return mState;
            }
        }
    }
}

