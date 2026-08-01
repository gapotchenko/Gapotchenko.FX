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

    [Flags]
    public enum MemoryMapFlags
    {
        Private = 0x02,
        Anonymous = 0x20,
        FixedNoReplace = 0x100000
    }

    [DllImport("libc", SetLastError = true, ExactSpelling = true)]
    public static extern int mprotect(void* address, nuint length, MemoryProtection protection);

    [DllImport("libc", SetLastError = true, ExactSpelling = true)]
    public static extern void* mmap(
        void* address,
        nuint length,
        MemoryProtection protection,
        MemoryMapFlags flags,
        int fileDescriptor,
        nint offset);

    [DllImport("libc", SetLastError = true, ExactSpelling = true)]
    public static extern int munmap(void* address, nuint length);

    [DllImport("libgcc_s.so.1", EntryPoint = "__clear_cache", ExactSpelling = true)]
    public static extern void ClearInstructionCache(void* begin, void* end);

    [DllImport("libgcc_s.so.1", EntryPoint = "__register_frame", ExactSpelling = true)]
    public static extern void RegisterFrame(void* ehFrame);
}
