using System;
using Abstractions;

namespace Plugins
{    
    public static class CrossImageHelper
    {
        private static Lazy<IImageHelper> TTS = new Lazy<IImageHelper>(Create, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static IImageHelper Current
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

        static IImageHelper Create()
        {
            #if PORTABLE
            return null;
            #else
            return new ImageHelperRealization();
            #endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("CrossImageHelper. This functionality is not implemented in the portable version of this assembly.");
        }
    }
}

