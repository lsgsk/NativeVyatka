using Android.Util;
using System;
using Abstractions;

namespace Plugins
{
    public class ConsoleRealization : IConsole
    {
        private string mTag = "XamarinApplication";

        public void InitTag(string tag)
        {
            mTag = tag;
        }
        public void WriteLine(string message)
        {
            Log.Info(mTag, message);
        }

        public void WriteLine(string tag, string message)
        {
            Log.Info(tag, message);
        }

        public void Error(Exception ex)
        {
            Log.Error(mTag, ex.ToString());
        }

        public void Error(string tag, string message)
        {
            Log.Error(tag, message);
        }

        public void Error(string message)
        {
            Log.Error(mTag, message);
        }
    }
}

