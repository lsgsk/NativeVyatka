using Abstractions;
using System;
using System.Diagnostics;

namespace NativeVyatka.UWP.Utilities
{
    public class ConsoleRealization : IConsole
    {
        public void Error(Exception ex)
        {
            Debug.Fail(ex.ToString());
        }

        public void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
