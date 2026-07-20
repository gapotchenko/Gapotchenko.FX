using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
readonly unsafe struct VirtualProtectionScope : IDisposable
{
    public VirtualProtectionScope(void* address, int size, NativeMethods.Page protect)
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

    public Span<T> GetSpan<T>() where T : struct
    {
        return new Span<T>(m_Address, m_Size);
    }

    readonly void* m_Address;
    readonly int m_Size;
    readonly NativeMethods.Page m_OldProtect;
}
