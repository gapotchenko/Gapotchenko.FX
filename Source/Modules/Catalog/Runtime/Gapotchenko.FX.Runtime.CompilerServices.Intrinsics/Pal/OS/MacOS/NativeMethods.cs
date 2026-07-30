// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
static unsafe class NativeMethods
{
    [Flags]
    public enum MemoryProtection
    {
        None = 0x0,
        Read = 0x1,
        Write = 0x2,
        Execute = 0x4
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VmRegionBasicInfo64
    {
        public MemoryProtection Protection;
        public MemoryProtection MaxProtection;
        public uint Inheritance;
        public int Shared;
        public int Reserved;
        public ulong Offset;
        public int Behavior;
        public ushort UserWiredCount;
    }

    [DllImport("libSystem.B.dylib", ExactSpelling = true)]
    public static extern uint mach_task_self();

    [DllImport("libSystem.B.dylib", ExactSpelling = true)]
    public static extern int mach_vm_region(
        uint targetTask,
        ref ulong address,
        out ulong size,
        int flavor,
        out VmRegionBasicInfo64 info,
        ref uint infoCount,
        out uint objectName);

    [DllImport("libSystem.B.dylib", SetLastError = true, ExactSpelling = true)]
    public static extern int mprotect(void* address, nuint length, MemoryProtection protection);

    [DllImport("libSystem.B.dylib", SetLastError = true, ExactSpelling = true)]
    public static extern void* mmap(void* address, nuint length, MemoryProtection protection, int flags, int fileDescriptor, nint offset);

    [DllImport("libSystem.B.dylib", SetLastError = true, ExactSpelling = true)]
    public static extern int munmap(void* address, nuint length);

    [DllImport("libSystem.B.dylib", ExactSpelling = true)]
    public static extern void pthread_jit_write_protect_np(int enabled);

    [DllImport("libSystem.B.dylib", ExactSpelling = true)]
    public static extern void sys_icache_invalidate(void* start, nuint length);
}
