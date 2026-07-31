// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
static class Unwind
{
    public static unsafe Span<byte> GetMethodInstructions(byte* p)
    {
        if (MemoryMap.TryGetRegion(p) is not { } region ||
            (region.Protection & NativeMethods.MemoryProtection.Execute) == 0)
        {
            return [];
        }

        nuint length = (nuint)(region.End - p);
        if (length > int.MaxValue)
            return [];

        return new(p, (int)length);
    }
}
