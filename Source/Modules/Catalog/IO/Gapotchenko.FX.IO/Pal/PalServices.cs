// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.IO.Pal;

/// <summary>
/// Provides platform abstraction layer (PAL) services.
/// </summary>
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

    public static IPalAdapter? Adapter => AdapterFactory.Instance;

    public static IPalAdapter RequiredAdapter => Adapter ?? throw new PlatformNotSupportedException();
}
