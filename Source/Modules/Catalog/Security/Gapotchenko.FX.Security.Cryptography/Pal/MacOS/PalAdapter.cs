// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if !HAS_TARGET_PLATFORM || MACOS

namespace Gapotchenko.FX.Security.Cryptography.Pal.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
sealed class PalAdapter : IPalAdapter
{
    public static PalAdapter Instance { get; } = new();

    public bool? QueryFipsPolicy()
    {
        // macOS does not provide a system-wide FIPS policy setting.
        return null;
    }
}

#endif
