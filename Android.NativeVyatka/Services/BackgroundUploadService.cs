using System;
using Android.App;
using Android.Content;
using NativeVyatkaCore;
using Microsoft.Practices.Unity;
using System.Threading;
using System.Net;

namespace NativeVyatkaAndroid
{
    [Service]
    public class BackgroundUploadService : IntentService
    {
        public static void UploadBurials()
        {
            Intent intent = new Intent(Application.Context, typeof(BackgroundUploadService));
            Application.Context.StartService(intent);
        }

        public BackgroundUploadService() : base("BackgroundUploadService")
        {
            mBurialManager = AppApplication.Container.Resolve<IBurialsManager>();
            mUploaderManager = AppApplication.Container.Resolve<IUploaderManager>();
        }

        protected override async void OnHandleIntent(Intent intent)
        {
            if (intent != null)
            {
                var tokensource = new CancellationTokenSource();
                var items = await mBurialManager.GetUnsendedBurials(tokensource.Token);
                const bool result = true;
                foreach (var item in items)
                {
                    Console.WriteLine(item.ToString());
                    item.IsSended = result;
                }
                await mBurialManager.UpdateSendedBurial(items,tokensource.Token);
                //FIXME Тут отправлем на сервер
                mUploaderManager.NotifiController(result);               
            }
        }

        private readonly IBurialsManager mBurialManager;
        private readonly IUploaderManager mUploaderManager;
    }
}

