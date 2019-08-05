using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Reflection.Pal
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
