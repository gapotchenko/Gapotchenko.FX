// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Globalization;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
static unsafe class MemoryMap
{
    public static bool TryGetRegion(void* address, out Region region)
    {
        ulong value = (ulong)(nuint)address;

        foreach (string line in File.ReadLines("/proc/self/maps"))
        {
            int rangeSeparator = line.IndexOf('-');
            int rangeEnd = line.IndexOf(' ');
            if (rangeSeparator < 0 || rangeEnd < 0 || rangeSeparator >= rangeEnd)
                continue;

            if (!ulong.TryParse(line[..rangeSeparator], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong start) ||
                !ulong.TryParse(line[(rangeSeparator + 1)..rangeEnd], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong end))
            {
                continue;
            }

            if (value < start || value >= end)
                continue;

            int permissionsStart = rangeEnd + 1;
            if (line.Length < permissionsStart + 4)
                break;

            var protection = NativeMethods.MemoryProtection.None;
            if (line[permissionsStart] == 'r')
                protection |= NativeMethods.MemoryProtection.Read;
            if (line[permissionsStart + 1] == 'w')
                protection |= NativeMethods.MemoryProtection.Write;
            if (line[permissionsStart + 2] == 'x')
                protection |= NativeMethods.MemoryProtection.Execute;

            region = new((byte*)(nuint)start, (byte*)(nuint)end, protection);
            return true;
        }

        region = default;
        return false;
    }

    public readonly struct Region(byte* start, byte* end, NativeMethods.MemoryProtection protection)
    {
        public byte* Start { get; } = start;
        public byte* End { get; } = end;
        public NativeMethods.MemoryProtection Protection { get; } = protection;
    }
}
