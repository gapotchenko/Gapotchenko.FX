// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
#endif
static unsafe class NativeMethods
{
    [Flags]
    public enum MemoryProtection
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }

    [DllImport("libc", SetLastError = true, ExactSpelling = true)]
    public static extern int mprotect(void* address, nuint length, MemoryProtection protection);
}
