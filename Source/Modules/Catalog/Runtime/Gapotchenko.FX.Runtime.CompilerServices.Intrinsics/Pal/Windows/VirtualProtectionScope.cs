using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
readonly unsafe struct VirtualProtectionScope : IDisposable
{
    public VirtualProtectionScope(void* address, int size, NativeMethods.Page protect)
    {
        m_Address = new IntPtr(address);
        m_Size = new IntPtr(size);

        if (!NativeMethods.VirtualProtect(m_Address, m_Size, protect, out m_OldProtect))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    readonly IntPtr m_Address;
    readonly IntPtr m_Size;
    readonly NativeMethods.Page m_OldProtect;

    public void Dispose()
    {
        // Restore the original memory protection at the end of the scope.
        NativeMethods.VirtualProtect(m_Address, m_Size, m_OldProtect, out _);
    }
}
