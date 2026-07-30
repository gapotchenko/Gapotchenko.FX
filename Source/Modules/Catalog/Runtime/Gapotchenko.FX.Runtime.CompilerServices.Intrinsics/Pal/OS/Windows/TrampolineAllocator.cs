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
    #region Global Allocation

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
        int allocationSize = Align(size);
        ref var globalBlock = ref m_GlobalBlock;

        lock (m_GlobalLock)
        {
            if ((nuint)(globalBlock.End - globalBlock.Current) < (nuint)allocationSize)
                globalBlock = AllocateGlobalBlock(allocationSize);

            byte* p = globalBlock.Allocate(allocationSize);

            return new Span<byte>(p, size);
        }
    }

    static Block AllocateGlobalBlock(int minimumSize)
    {
        int blockSize = Math.Max(Environment.SystemPageSize, minimumSize);
        void* p = NativeMethods.VirtualAlloc(
            null,
            (nuint)blockSize,
            NativeMethods.VirtualAllocationType.Reserve | NativeMethods.VirtualAllocationType.Commit,
            NativeMethods.PageProtect.ExecuteRead);

        if (p == null)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        return new Block((byte*)p, blockSize);
    }

    static readonly Lock m_GlobalLock = new();
    static Block m_GlobalBlock;

    #endregion

    #region Proximal Allocation

    public static bool TryAllocateNear<T>(
        void* target,
        int count,
        nuint maximumDistance,
        out Span<T> allocation)
        where T : struct
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        int size = Align(checked(count * Unsafe.SizeOf<T>()));

        var nearBlocks = m_NearBlocks;
        lock (nearBlocks)
        {
            for (int i = 0; i < nearBlocks.Count; ++i)
            {
                var block = nearBlocks[i];
                if ((nuint)(block.End - block.Current) >= (nuint)size &&
                    IsWithinDistance(target, block.Current, maximumDistance))
                {
                    allocation = new(block.Allocate(size), count);
                    nearBlocks[i] = block;
                    return true;
                }
            }

            if (!TryAllocateNearBlock(target, size, maximumDistance, out var newBlock))
            {
                allocation = [];
                return false;
            }

            allocation = new(newBlock.Allocate(size), count);
            nearBlocks.Add(newBlock);
            return true;
        }
    }

    static readonly List<Block> m_NearBlocks = [];

    static bool IsWithinDistance(void* x, void* y, nuint maximumDistance)
    {
        nuint a = (nuint)x;
        nuint b = (nuint)y;
        return a >= b ? a - b <= maximumDistance : b - a <= maximumDistance;
    }

    static bool TryAllocateNearBlock(void* target, int minimumSize, nuint maximumDistance, out Block block)
    {
        const nuint AllocationGranularity = 64 * 1024;

        int blockSize = AlignUp(Math.Max(Environment.SystemPageSize, minimumSize), Environment.SystemPageSize);
        nuint targetAddress = (nuint)target;
        nuint minimumAddress = targetAddress > maximumDistance ? targetAddress - maximumDistance : 0;
        nuint maximumAddress = targetAddress <= nuint.MaxValue - maximumDistance ? targetAddress + maximumDistance - 1 : nuint.MaxValue;

        minimumAddress = AlignUp(minimumAddress, AllocationGranularity);
        maximumAddress = maximumAddress == nuint.MaxValue ? maximumAddress : ((maximumAddress + 1) & ~(AllocationGranularity - 1)) - 1;
        if (minimumAddress > maximumAddress)
        {
            block = default;
            return false;
        }

        var addressRequirements = new NativeMethods.MemoryAddressRequirements
        {
            LowestStartingAddress = (void*)minimumAddress,
            HighestEndingAddress = (void*)maximumAddress
        };

        var extendedParameter = new NativeMethods.MemoryExtendedParameter
        {
            Type = NativeMethods.MemoryExtendedParameterType.AddressRequirements,
            Pointer = &addressRequirements
        };

        void* p = NativeMethods.VirtualAlloc2(
            IntPtr.Zero,
            null,
            (nuint)blockSize,
            NativeMethods.VirtualAllocationType.Reserve | NativeMethods.VirtualAllocationType.Commit,
            NativeMethods.PageProtect.ExecuteRead,
            &extendedParameter,
            1);

        if (p == null)
        {
            block = default;
            return false;
        }

        block = new Block((byte*)p, blockSize);
        return true;
    }

    #endregion

    struct Block(byte* current, int size)
    {
        public byte* Allocate(int allocationSize)
        {
            byte* p = Current;
            Current += allocationSize;
            return p;
        }

        public byte* Current { get; private set; } = current;

        public byte* End { get; } = current + size;
    }

    static int Align(int size)
    {
        const int alignment = 16;
        return checked((size + alignment - 1) & -alignment);
    }

    static int AlignUp(int value, int alignment)
    {
        return checked((value + alignment - 1) / alignment * alignment);
    }

    static nuint AlignUp(nuint value, nuint alignment)
    {
        return checked((value + alignment - 1) & ~(alignment - 1));
    }
}
