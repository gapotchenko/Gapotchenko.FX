// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !HAS_TARGET_PLATFORM || LINUX

namespace Gapotchenko.FX.IO.Pal.Linux;

#if NET && !LINUX
[SupportedOSPlatform("linux")]
#endif
sealed class PalAdapter : Unix.PalAdapter
{
    PalAdapter()
    {
    }

    public static PalAdapter Instance { get; } = new PalAdapter();
}

#endif
