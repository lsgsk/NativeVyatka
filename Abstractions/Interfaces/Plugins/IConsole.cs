using System;

namespace Abstractions
{
    public interface IConsole
    {
        void WriteLine(string message);
        void Error(Exception ex);
    }
}

