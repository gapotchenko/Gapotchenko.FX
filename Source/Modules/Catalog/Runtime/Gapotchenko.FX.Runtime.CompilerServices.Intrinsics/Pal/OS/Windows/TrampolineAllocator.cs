// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

#if NET
[SupportedOSPlatform("windows")]
#endif
static unsafe class TrampolineAllocator
{
    public static Span<T> Allocate<T>(int count) where T : struct
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        var bytes = AllocateCore(count * Unsafe.SizeOf<T>());

        return new(
            Unsafe.AsPointer(ref MemoryMarshal.GetReference(bytes)),
            count);
    }

    public static Span<byte> Allocate(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        return AllocateCore(size);
    }

    static Span<byte> AllocateCore(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        lock (m_SyncRoot)
        {
            int allocationSize = Align(size);

            if ((nuint)(m_End - m_Current) < (nuint)allocationSize)
                AllocateBlock(allocationSize);

            byte* p = m_Current;
            m_Current += allocationSize;

            return new Span<byte>(p, size);
        }
    }

    static void AllocateBlock(int minimumSize)
    {
        int blockSize = Math.Max(Environment.SystemPageSize, minimumSize);
        void* p = NativeMethods.VirtualAlloc(
            null,
            (nuint)blockSize,
            NativeMethods.VirtualAllocationType.Reserve | NativeMethods.VirtualAllocationType.Commit,
            NativeMethods.PageProtect.ExecuteReadWrite);

        if (p == null)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        m_Current = (byte*)p;
        m_End = m_Current + blockSize;
    }

    static int Align(int size) => checked((size + Alignment - 1) & -Alignment);

    const int Alignment = 16;

    static readonly Lock m_SyncRoot = new();
    static byte* m_Current;
    static byte* m_End;
}
