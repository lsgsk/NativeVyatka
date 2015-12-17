using System;
using Android.App;
using Android.OS;
using Android.Content;

namespace NativeVyatkaAndroid
{
    #if DEBUG
    [Service(Exported = true)]
    #else
    [Service(Exported = true, Process = ":sync")]
    #endif
    [IntentFilter(new[] { "android.content.SyncAdapter" })]
    [MetaData("android.content.SyncAdapter", Resource = "@xml/syncadapter")]
    public class SyncService : Service
    {
        private static SyncAdapter sSyncAdapter;

        public override void OnCreate()
        {
            base.OnCreate();
            if (sSyncAdapter == null)
            {
                sSyncAdapter = new SyncAdapter(ApplicationContext);
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return sSyncAdapter.SyncAdapterBinder;
        }
    }
}

