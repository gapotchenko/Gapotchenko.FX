// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !HAS_TARGET_PLATFORM || MACOS

namespace Gapotchenko.FX.IO.Pal.MacOS;

#if NET && !MACOS
[SupportedOSPlatform("macos")]
#endif
sealed class PalAdapter : Unix.PalAdapter
{
    PalAdapter()
    {
    }

    public static PalAdapter Instance { get; } = new PalAdapter();

    public override bool IsCaseSensitive => false; // HFS+ (the macOS file-system) is usually configured to be case insensitive
}

#endif
