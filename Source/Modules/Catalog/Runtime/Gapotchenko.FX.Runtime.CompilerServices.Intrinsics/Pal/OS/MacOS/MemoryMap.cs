// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

using System.Runtime.CompilerServices;

#if NET
[SupportedOSPlatform("macos")]
#endif
static unsafe class MemoryMap
{
    public static Region? TryGetRegion(void* address)
    {
        const int VM_REGION_BASIC_INFO_64 = 9;

        ulong addressValue;
        try
        {
            addressValue = checked((ulong)address);
        }
        catch (OverflowException)
        {
            return null;
        }

        ulong start = addressValue;
        uint infoCount = checked((uint)(Unsafe.SizeOf<NativeMethods.VmRegionBasicInfo64>() / sizeof(int)));
        int result = NativeMethods.mach_vm_region(NativeMethods.mach_task_self(), ref start, out ulong size, VM_REGION_BASIC_INFO_64, out var info, ref infoCount, out _);
        if (result == 0 && start <= addressValue && size <= ulong.MaxValue - start)
        {
            ulong end = start + size;
            if (addressValue < end)
            {
                try
                {
                    return new Region(
                        (byte*)checked((nuint)start),
                        (byte*)checked((nuint)end),
                        info.Protection);
                }
                catch (OverflowException)
                {
                    return null;
                }
            }
        }

        return null;
    }

    public readonly struct Region(byte* start, byte* end, NativeMethods.MemoryProtection protection)
    {
        public byte* Start { get; } = start;
        public byte* End { get; } = end;
        public NativeMethods.MemoryProtection Protection { get; } = protection;
    }
}
