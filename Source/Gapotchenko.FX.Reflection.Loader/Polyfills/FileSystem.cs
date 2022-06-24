using System;

namespace Gapotchenko.FX.Reflection.Loader.Polyfills;

static class FileSystem
{
    public static StringComparer PathComparer =>
        IsCaseSensitive ?
            StringComparer.InvariantCulture :
            StringComparer.InvariantCultureIgnoreCase;

    public static bool IsCaseSensitive { get; } = Environment.OSVersion.Platform == PlatformID.Unix;
}
