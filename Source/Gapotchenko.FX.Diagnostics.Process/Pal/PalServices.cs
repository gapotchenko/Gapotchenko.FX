using System;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Diagnostics.Pal
{
    static class PalServices
    {
        static class AdapterFactory
        {
            public static IPalAdapter? Instance { get; } = CreateInstance();

            static IPalAdapter? CreateInstance()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return Windows.PalAdapter.Instance;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return MacOS.PalAdapter.Instance;
                else
                    return null;
            }
        }

        public static IPalAdapter? AdapterOrDefault => AdapterFactory.Instance;

        public static IPalAdapter Adapter => AdapterOrDefault ?? throw new PlatformNotSupportedException();
    }
}
