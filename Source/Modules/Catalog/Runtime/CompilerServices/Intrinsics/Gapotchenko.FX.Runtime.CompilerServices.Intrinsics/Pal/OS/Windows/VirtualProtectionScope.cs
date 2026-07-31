// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
readonly unsafe struct VirtualProtectionScope : IDisposable
{
    public static VirtualProtectionScope Create<T>(Span<T> span, NativeMethods.PageProtect protect)
    {
        void* address = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        nuint size = checked((nuint)span.Length * (nuint)Unsafe.SizeOf<T>());
        return new(address, size, protect);
    }

    VirtualProtectionScope(void* address, nuint size, NativeMethods.PageProtect protect)
    {
        m_Address = address;
        m_Size = size;

        if (!NativeMethods.VirtualProtect(m_Address, m_Size, protect, out m_OldProtect))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    public void Dispose()
    {
        // Restore the original memory protection at the end of the scope.
        _ = NativeMethods.VirtualProtect(m_Address, m_Size, m_OldProtect, out _);
    }

    /// <summary>
    /// Makes the newly written instructions visible to the processor.
    /// </summary>
    public void FlushInstructions()
    {
        if (!NativeMethods.FlushInstructionCache(new IntPtr(-1), m_Address, m_Size))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    readonly void* m_Address;
    readonly nuint m_Size;
    readonly NativeMethods.PageProtect m_OldProtect;
}
