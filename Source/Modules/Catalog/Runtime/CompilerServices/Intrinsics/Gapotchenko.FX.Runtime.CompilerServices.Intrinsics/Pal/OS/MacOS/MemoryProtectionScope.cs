// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
readonly unsafe struct MemoryProtectionScope : IDisposable
{
    public static MemoryProtectionScope Create<T>(Span<T> span, NativeMethods.MemoryProtection protection)
    {
        void* address = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        return new(address, checked((nuint)span.Length * (nuint)Unsafe.SizeOf<T>()), protection);
    }

    MemoryProtectionScope(void* address, nuint size, NativeMethods.MemoryProtection protection)
    {
        if (MemoryMap.TryGetRegion(address) is not { } region)
            throw new InvalidOperationException("Cannot determine the memory region containing the memory scope.");

        nuint start = (nuint)address;
        nuint end = checked(start + size);
        if (end > (nuint)region.End)
            throw new InvalidOperationException("The memory scope crosses a memory-region boundary.");

        nuint pageSize = (nuint)Environment.SystemPageSize;
        nuint pageStart = start / pageSize * pageSize;
        nuint pageEnd = checked((end + pageSize - 1) / pageSize * pageSize);
        m_Address = (void*)pageStart;
        m_Size = pageEnd - pageStart;
        m_OldProtection = region.Protection;

        if (NativeMethods.mprotect(m_Address, m_Size, protection) != 0)
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    public void Dispose()
    {
        _ = NativeMethods.mprotect(m_Address, m_Size, m_OldProtection);
    }

    /// <summary>
    /// Makes the newly written instructions visible to the processor.
    /// </summary>
    public void FlushInstructions()
    {
        NativeMethods.sys_icache_invalidate(m_Address, m_Size);
    }

    readonly void* m_Address;
    readonly nuint m_Size;
    readonly NativeMethods.MemoryProtection m_OldProtection;
}
