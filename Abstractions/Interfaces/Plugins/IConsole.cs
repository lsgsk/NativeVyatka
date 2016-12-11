using System;

namespace Abstractions
{
    public interface IConsole
    {
        void InitTag(string tag);
        void WriteLine(string message);
        void WriteLine(string tag, string message);
        void Error(string message);
        void Error(string tag, string message);
        void Error(Exception ex);
    }
}

