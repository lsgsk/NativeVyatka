using Abstractions;
using System;

namespace NativeVyatkaIOS.Utilities
{
    public class ConsoleRealization : IConsole
    {
        public void Error(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
