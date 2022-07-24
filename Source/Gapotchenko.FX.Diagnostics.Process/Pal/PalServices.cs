using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Diagnostics.Pal;

static class PalServices
{
    static class AdapterFactory
    {
        public static IPalAdapter? Instance { get; } = CreateInstance();

        static IPalAdapter? CreateInstance()
        {
#if HAS_TARGET_PLATFORM
#if WINDOWS
            return Windows.PalAdapter.Instance;
#elif MACOS
            return MacOS.PalAdapter.Instance;
#elif LINUX
            return Linux.PalAdapter.Instance;
#else
            return null;
#endif
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Windows.PalAdapter.Instance;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return MacOS.PalAdapter.Instance;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return Linux.PalAdapter.Instance;
            else
                return null;
#endif
        }
    }

    public static IPalAdapter? AdapterOrDefault => AdapterFactory.Instance;

    public static IPalAdapter Adapter => AdapterOrDefault ?? throw new PlatformNotSupportedException();
}
