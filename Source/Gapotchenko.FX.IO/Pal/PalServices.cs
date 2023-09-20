using System.Runtime.InteropServices;

namespace Gapotchenko.FX.IO.Pal;

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
#else
            return null;
#endif
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Windows.PalAdapter.Instance;
            else
                return null;
#endif
        }
    }

    public static IPalAdapter? AdapterOrDefault => AdapterFactory.Instance;

    public static IPalAdapter Adapter => AdapterOrDefault ?? throw new PlatformNotSupportedException();
}
