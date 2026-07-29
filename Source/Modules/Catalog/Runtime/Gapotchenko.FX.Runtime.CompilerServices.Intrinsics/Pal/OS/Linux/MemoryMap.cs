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
    public static Region? TryGetRegion(void* address)
    {
        nuint value = (nuint)address;

        foreach (string line in File.ReadLines("/proc/self/maps"))
        {
            int rangeSeparator = line.IndexOf('-');
            int rangeEnd = line.IndexOf(' ');
            if (rangeSeparator < 0 || rangeEnd < 0 || rangeSeparator >= rangeEnd)
                continue;

            var s = line.AsSpan();
            if (!TryParseAddress(s[..rangeSeparator], out nuint start) ||
                !TryParseAddress(s[(rangeSeparator + 1)..rangeEnd], out nuint end))
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

            return new Region((byte*)start, (byte*)end, protection);
        }

        return null;
    }

    static bool TryParseAddress(ReadOnlySpan<char> s, out nuint result)
    {
        const NumberStyles style = NumberStyles.HexNumber;
        var provider = CultureInfo.InvariantCulture;
#if NET
        return nuint.TryParse(s, style, provider, out result);
#else
        if (ulong.TryParse(s.ToString(), style, provider, out ulong value))
        {
            try
            {
                result = checked((nuint)value);
            }
            catch (OverflowException)
            {
                result = default;
                return false;
            }
            return true;
        }
        else
        {
            result = default;
            return false;
        }
#endif
    }

    public readonly struct Region(byte* start, byte* end, NativeMethods.MemoryProtection protection)
    {
        public byte* Start { get; } = start;
        public byte* End { get; } = end;
        public NativeMethods.MemoryProtection Protection { get; } = protection;
    }
}
