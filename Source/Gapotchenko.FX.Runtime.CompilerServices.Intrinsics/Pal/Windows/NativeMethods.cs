using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
static class NativeMethods
{
    [Flags]
    public enum Page : uint
    {
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        Guard = 0x100,
        NoCache = 0x200,
        WriteCombine = 0x400
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool VirtualProtect(
        IntPtr lpAddress,
        IntPtr dwSize,
        Page flNewProtect,
        out Page lpflOldProtect);
}
