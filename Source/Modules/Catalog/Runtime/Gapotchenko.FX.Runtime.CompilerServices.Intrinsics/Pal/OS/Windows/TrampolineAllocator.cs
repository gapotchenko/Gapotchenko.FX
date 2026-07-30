// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Windows;

/// <summary>
/// Provides functionality for an ever-growing executable memory allocation.
/// </summary>
#if NET
[SupportedOSPlatform("windows")]
#endif
static unsafe class TrampolineAllocator
{
    public static Span<T> Allocate<T>(int count) where T : struct
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        var bytes = AllocateCore(checked(count * Unsafe.SizeOf<T>()));

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

    public static bool TryAllocateNear<T>(void* target, int count, nuint maximumDistance, out Span<T> allocation) where T : struct
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        int size = checked(count * Unsafe.SizeOf<T>());
        lock (m_SyncRoot)
        {
            int allocationSize = Align(size);

            for (int i = 0; i < m_NearBlocks.Count; ++i)
            {
                var block = m_NearBlocks[i];
                if ((nuint)(block.End - block.Current) >= (nuint)allocationSize &&
                    IsWithinDistance(target, block.Current, maximumDistance))
                {
                    allocation = new(block.Allocate(allocationSize), count);
                    m_NearBlocks[i] = block;
                    return true;
                }
            }

            if (!TryAllocateBlockNear(target, allocationSize, maximumDistance, out var newBlock))
            {
                allocation = [];
                return false;
            }

            allocation = new(newBlock.Allocate(allocationSize), count);
            m_NearBlocks.Add(newBlock);
            return true;
        }
    }

    static void AllocateBlock(int minimumSize)
    {
        int blockSize = Math.Max(Environment.SystemPageSize, minimumSize);
        void* p = NativeMethods.VirtualAlloc(
            null,
            (nuint)blockSize,
            NativeMethods.VirtualAllocationType.Reserve | NativeMethods.VirtualAllocationType.Commit,
            NativeMethods.PageProtect.ExecuteRead);

        if (p == null)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        m_Current = (byte*)p;
        m_End = m_Current + blockSize;
    }

    static bool TryAllocateBlockNear(void* target, int minimumSize, nuint maximumDistance, out Block block)
    {
        const nuint AllocationGranularity = 64 * 1024;

        int blockSize = Math.Max(Environment.SystemPageSize, minimumSize);
        nuint targetAddress = (nuint)target;
        nuint minimumAddress = targetAddress > maximumDistance ? targetAddress - maximumDistance : 0;
        nuint maximumAddress = targetAddress <= NuintMaxValue - maximumDistance ? targetAddress + maximumDistance : NuintMaxValue;
        nuint origin = targetAddress & ~(AllocationGranularity - 1);

        for (nuint distance = 0; distance <= maximumDistance; distance += AllocationGranularity)
        {
            if (origin >= distance)
            {
                nuint candidate = origin - distance;
                if (candidate >= minimumAddress && TryAllocateBlockAt(candidate, blockSize, out block))
                    return true;
            }

            if (distance != 0 && origin <= NuintMaxValue - distance)
            {
                nuint candidate = origin + distance;
                if (candidate <= maximumAddress && TryAllocateBlockAt(candidate, blockSize, out block))
                    return true;
            }

            if (maximumDistance - distance < AllocationGranularity)
                break;
        }

        block = default;
        return false;
    }

    static bool TryAllocateBlockAt(nuint address, int blockSize, out Block block)
    {
        void* p = NativeMethods.VirtualAlloc(
            (void*)address,
            (nuint)blockSize,
            NativeMethods.VirtualAllocationType.Reserve | NativeMethods.VirtualAllocationType.Commit,
            NativeMethods.PageProtect.ExecuteRead);

        if (p == null)
        {
            block = default;
            return false;
        }

        block = new() { Current = (byte*)p, End = (byte*)p + blockSize };
        return true;
    }

    static bool IsWithinDistance(void* x, void* y, nuint maximumDistance)
    {
        nuint a = (nuint)x;
        nuint b = (nuint)y;
        return a >= b ? a - b <= maximumDistance : b - a <= maximumDistance;
    }

    static readonly nuint NuintMaxValue = unchecked((nuint)(nint)(-1));

    static readonly Lock m_SyncRoot = new();

    struct Block
    {
        public void* Allocate(int allocationSize)
        {
            byte* p = Current;
            Current += allocationSize;
            return p;
        }

        public byte* Current;
        public byte* End;
    }

    static byte* m_Current;
    static byte* m_End;
    static readonly List<Block> m_NearBlocks = [];

    static int Align(int size)
    {
        const int alignment = 16;
        return checked((size + alignment - 1) & -alignment);
    }
}
