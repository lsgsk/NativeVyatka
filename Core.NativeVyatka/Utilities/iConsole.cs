using Abstractions;
using System;

namespace NativeVyatkaCore.Utilities
{
    public static class iConsole
    {
        public static void Init(IConsole console)
        {
            mConsole = console;
        }

        public static void WriteLine(string message)
        {
            mConsole?.WriteLine(message);
        }
               
        public static void Error(Exception ex)
        {
            mConsole?.Error(ex);
        }

        private static IConsole mConsole;
    }
}
