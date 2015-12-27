using System;
using Abstractions;

namespace Plugins
{
    public static class CrossFetchAddress
    {
        private static Lazy<IFetchAddress> TTS = new Lazy<IFetchAddress>(Create, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static IFetchAddress Current
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

        static IFetchAddress Create()
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

