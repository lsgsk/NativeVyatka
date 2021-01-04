using System;

namespace NativeVyatka
{
    public interface IConsole
    {
        void WriteLine(string message);
        void Error(Exception ex);
    }

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
