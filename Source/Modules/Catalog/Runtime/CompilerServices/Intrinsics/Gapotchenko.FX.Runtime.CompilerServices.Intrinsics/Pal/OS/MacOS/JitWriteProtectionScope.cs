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
readonly unsafe struct JitWriteProtectionScope : IDisposable
{
    public static JitWriteProtectionScope Create<T>(Span<T> span)
    {
        void* address = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
        nuint size = checked((nuint)span.Length * (nuint)Unsafe.SizeOf<T>());
        return new(address, size);
    }

    JitWriteProtectionScope(void* address, nuint size)
    {
        m_Address = address;
        m_Size = size;
        NativeMethods.pthread_jit_write_protect_np(0);
    }

    public void Dispose()
    {
        NativeMethods.pthread_jit_write_protect_np(1);
    }

    public void FlushInstructions()
    {
        NativeMethods.sys_icache_invalidate(m_Address, m_Size);
    }

    readonly void* m_Address;
    readonly nuint m_Size;
}
