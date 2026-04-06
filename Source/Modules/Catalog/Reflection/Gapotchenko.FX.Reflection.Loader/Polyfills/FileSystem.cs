using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Reflection.Loader.Polyfills;

static class FileSystem
{
    public static StringComparer PathComparer =>
        IsCaseSensitive ?
            StringComparer.InvariantCulture :
            StringComparer.InvariantCultureIgnoreCase;

    public static bool IsCaseSensitive { get; } = IsCaseSensitiveCore();

    static bool IsCaseSensitiveCore()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return false;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
#if NETCOREAPP3_0_OR_GREATER
            || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)
#endif
            )
        {
            return true;
        }
        else
        {
            // A graceful fallback.
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return true;
            else
                return false; // a safer default
        }
    }
}
