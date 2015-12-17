using System;
using System.Threading;
using Android.Content;
using Android.Accounts;
using Android.OS;

namespace NativeVyatkaAndroid
{
    public class SyncAdapter : AbstractThreadedSyncAdapter
    {
        public SyncAdapter(Context context) : base(context, true)
        {            
        }

        public override void OnPerformSync(Account account, Bundle extras, string authority, ContentProviderClient provider, SyncResult syncResult)
        {
            Thread.Sleep(5000);
        }
    }
}

