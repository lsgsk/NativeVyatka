using Android.Util;
using System;

namespace NativeVyatka
{
    public class ConsoleRealization : IConsole
    {
        private string mTag = "XamarinApplication";

        public void WriteLine(string message) {
            Log.Info(mTag, message);
        }

        public void Error(Exception ex) {
            Log.Error(mTag, ex.ToString());
        }
    }
}

