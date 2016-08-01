using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Accounts;
using Abstractions;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;

namespace NativeVyatkaAndroid
{
    public abstract class BaseAppCompatActivity : AppCompatActivity, IControllerReceiver
    {
        protected override void OnResume()
        {
            base.OnResume();
            mUploaderNotifier.SetControllerListener(this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            mUploaderNotifier.SetControllerListener(null);
        }

        public static void OnRefresh(Account account)
        {
            var extras = new Bundle();
            extras.PutBoolean(ContentResolver.SyncExtrasManual, true);
            extras.PutBoolean(ContentResolver.SyncExtrasExpedited, true); 
            ContentResolver.RequestSync(account, SyncConstants.CONTENT_AUTHORITY, extras);
        }

        public void ShowSnack(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                using (var handler = new Handler(Application.MainLooper))
                {
                    handler.Post(() => Snackbar.Make(Window.DecorView, message, Snackbar.LengthShort).Show()); //FindViewById(Resource.Id.content_frame)
                }
            }
        }

        public virtual void UploadingStarted()
        {
            
        }

        public virtual void UploadingFinished(bool uploadResult)
        {            
        }

        public async Task ShowNotification(string message)
        {            
            ShowSnack(message);
            await Task.Delay(0);
        }      

        private IUploaderListener mUploaderNotifier
        {
            get
            {
                return MainApplication.Container.Resolve<IUploaderListener>();
            }
        }
    }
}

