using System;
using Android.App;
using Android.Content;
using System.Threading;
using NativeVyatkaCore.Utilities;

namespace NativeVyatkaAndroid
{
    [Service]
    public class BackgroundUploadService : IntentService
    {
        public static void UploadBurials(int? id = null)
        {
            var intent = new Intent(Application.Context, typeof(BackgroundUploadService));
            if (id != null && id.HasValue)
            {
                intent.PutExtra(ID, id.Value);
            }
            Application.Context.StartService(intent);
        }

        public BackgroundUploadService() : base("BackgroundUploadService")
        {
        }

        protected override async void OnHandleIntent(Intent intent)
        {
            bool result = false;
            string message = "Отправка записей начата";
            iConsole.WriteLine("UploadService start");         
            if (!Uploading && intent != null)
            {                
                try
                {
                    Uploading = true;
                    //mUploaderNotifier.UploadingStarted(message);
                    var tokensource = new CancellationTokenSource();
                    var id = intent.GetIntExtra(ID, -1);
                    /*var items = id == -1 ? await mBurialManager.GetUnsendedBurials(tokensource.Token) : new List<BurialEntity>(1) { await mBurialManager.GetBurial(id, tokensource.Token) };
                    if (await mRestManager.UploadNewBurials(items))
                    {
                        //---items.All(x => x.IsSended = true);
                        await mBurialManager.UpdateSendedBurial(items, tokensource.Token);
                    }*/
                    message = "Отправка записей успешна закончена";
                    result = true;
                }
                catch (Exception ex)
                {
                    iConsole.Error(ex);
                    message = "Ошибка при отправке записей";
                }
                finally
                {
                    //mUploaderNotifier.UploadingFinished(result, message);
                    Uploading = false; 
                }
            }           
        }

        private static bool Uploading = false;
        private const string ID = "id";
    }
}

