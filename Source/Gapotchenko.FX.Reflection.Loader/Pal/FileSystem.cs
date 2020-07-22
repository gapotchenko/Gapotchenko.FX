using System;

namespace Gapotchenko.FX.Reflection.Loader.Pal
{
    static class FileSystem
    {
        public static StringComparer PathComparer =>
            IsCaseSensitive ?
                StringComparer.InvariantCulture :
                StringComparer.InvariantCultureIgnoreCase;

        public static bool IsCaseSensitive { get; } = Environment.OSVersion.Platform == PlatformID.Unix;
    }
}
