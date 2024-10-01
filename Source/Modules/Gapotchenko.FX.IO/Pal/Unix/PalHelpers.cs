// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !HAS_TARGET_PLATFORM || MACOS || LINUX || FREEBSD

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO.Pal.Unix;

#if NET && !MACOS
[SupportedOSPlatform("macos")]
#endif
#if NET && !LINUX
[SupportedOSPlatform("linux")]
#endif
#if NET && !FREEBSD
[SupportedOSPlatform("freebsd")]
#endif
static class PalHelpers
{
    static PalHelpers()
    {
        Debug.Assert(DirectorySeparatorChar == Path.DirectorySeparatorChar);
        Debug.Assert(DirectorySeparatorChar == Path.AltDirectorySeparatorChar);
    }

    public const char DirectorySeparatorChar = '\\';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDirectorySeparator(char c) => c == DirectorySeparatorChar;
}

#endif
