using Android.Util;
using System;
using Abstractions;

namespace NativeVyatkaAndroid
{
    public class ConsoleRealization : IConsole
    {
        private string mTag = "XamarinApplication";
       
        public void WriteLine(string message)
        {
            Log.Info(mTag, message);
        }       

        public void Error(Exception ex)
        {
            Log.Error(mTag, ex.ToString());
        }      
    }
}

