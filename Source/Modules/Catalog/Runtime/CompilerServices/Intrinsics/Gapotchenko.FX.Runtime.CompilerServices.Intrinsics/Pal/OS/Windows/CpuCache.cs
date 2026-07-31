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
static unsafe class CpuCache
{
    /// <summary>
    /// Makes the newly written instructions visible to the processor.
    /// </summary>
    public static void FlushInstructions<T>(ReadOnlySpan<T> span)
    {
        void* address = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        nuint size = checked((nuint)span.Length * (nuint)Unsafe.SizeOf<T>());

        if (!NativeMethods.FlushInstructionCache(new IntPtr(-1), address, size))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }
}
