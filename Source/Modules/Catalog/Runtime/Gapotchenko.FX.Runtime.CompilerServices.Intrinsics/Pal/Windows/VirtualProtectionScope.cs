// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
readonly unsafe struct VirtualProtectionScope : IDisposable
{
    public static VirtualProtectionScope Create<T>(Span<T> span, NativeMethods.PageProtect protect)
    {
        return new(
            Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)),
            span.Length,
            protect);
    }

    public VirtualProtectionScope(void* address, int size, NativeMethods.PageProtect protect)
    {
        m_Address = address;
        m_Size = size;

        if (!NativeMethods.VirtualProtect(m_Address, (nuint)m_Size, protect, out m_OldProtect))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    public void Dispose()
    {
        // Restore the original memory protection at the end of the scope.
        NativeMethods.VirtualProtect(m_Address, (nuint)m_Size, m_OldProtect, out _);
    }

    public int Size => m_Size;

    public Span<T> GetSpan<T>() where T : struct
    {
        return new Span<T>(m_Address, m_Size);
    }

    /// <summary>
    /// Makes the newly written instructions visible to the processor.
    /// </summary>
    public void FlushInstructions()
    {
        if (!NativeMethods.FlushInstructionCache(new IntPtr(-1), m_Address, (nuint)m_Size))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    readonly void* m_Address;
    readonly int m_Size;
    readonly NativeMethods.PageProtect m_OldProtect;
}
