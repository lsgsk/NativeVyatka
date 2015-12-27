using System;
using Abstractions;

namespace Plugins
{
    public static class iConsole
    {
        public static void WriteLine(string message)
        {
            CrossConsole.Current.WriteLine(message);
        }
        public static void WrireLine(string tag, string message)
        {
            CrossConsole.Current.WriteLine(tag, message);
        }
        public static void Error(string message)
        {
            CrossConsole.Current.Error(message);
        }
        public static void Error(string tag, string message)
        {
            CrossConsole.Current.Error(message);
        }
        public static void Error(Exception ex)
        {
            CrossConsole.Current.Error(ex);
        }
    }

    public static class CrossConsole
    {
        private static Lazy<IConsole> TTS = new Lazy<IConsole>(Create, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static IConsole Current
        {
            get
            {
                var ret = TTS.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IConsole Create()
        {
            #if PORTABLE
            return null;
            #else
            return new ConsoleRealization();
            #endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the Xam.Plugins.Vibrate NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}

