// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Utils;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.Linux;

#if NET
[SupportedOSPlatform("linux")]
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
        int allocationSize = MemoryArithmetics.Align16(size);
        ref var globalBlock = ref m_GlobalBlock;

        lock (m_GlobalLock)
        {
            if (globalBlock.AvailableSize < (nuint)allocationSize)
                globalBlock = AllocateGlobalBlock(allocationSize);

            byte* p = globalBlock.Allocate(allocationSize);

            return new Span<byte>(p, size);
        }
    }

    static LinearMemoryBlock AllocateGlobalBlock(int minimumSize)
    {
        nuint pageSize = (nuint)Environment.SystemPageSize;
        nuint blockSize = MemoryArithmetics.AlignUp((nuint)Math.Max(Environment.SystemPageSize, minimumSize), pageSize);
        void* p = NativeMethods.mmap(
            null,
            blockSize,
            NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Execute,
            NativeMethods.MemoryMapFlags.Private | NativeMethods.MemoryMapFlags.Anonymous,
            -1,
            0);
        if (p == (void*)(-1))
            throw new InvalidOperationException("Cannot allocate executable memory for an intrinsic trampoline.");

        return new((byte*)p, blockSize);
    }

    static readonly Lock m_GlobalLock = new();
    static LinearMemoryBlock m_GlobalBlock;

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

        int allocationSize = MemoryArithmetics.Align16(checked(count * Unsafe.SizeOf<T>()));
        var blocks = m_NearBlocks;
        lock (blocks)
        {
            for (int i = 0; i < blocks.Count; ++i)
            {
                var block = blocks[i];
                if (block.AvailableSize >= (nuint)allocationSize &&
                    MemoryArithmetics.IsWithinDistance((nint)target, (nint)block.Current, maximumDistance))
                {
                    allocation = new(block.Allocate(allocationSize), count);
                    blocks[i] = block;
                    return true;
                }
            }

            if (!TryAllocateNearBlock(target, allocationSize, maximumDistance, out var newBlock))
            {
                allocation = [];
                return false;
            }

            allocation = new(newBlock.Allocate(allocationSize), count);
            blocks.Add(newBlock);
            return true;
        }
    }

    static bool TryAllocateNearBlock(void* target, int minimumSize, nuint maximumDistance, out LinearMemoryBlock block)
    {
        nuint pageSize = (nuint)Environment.SystemPageSize;
        nuint blockSize = MemoryArithmetics.AlignUp((nuint)Math.Max(Environment.SystemPageSize, minimumSize), pageSize);
        nuint targetAddress = (nuint)target;
        nuint minimumAddress = targetAddress > maximumDistance ? targetAddress - maximumDistance : 0;
        nuint maximumAddress = targetAddress <= nuint.MaxValue - maximumDistance ? targetAddress + maximumDistance : nuint.MaxValue;
        nuint limitEnd = maximumAddress == nuint.MaxValue ? maximumAddress : maximumAddress + 1;
        nuint cursor = MemoryArithmetics.AlignUp(minimumAddress, pageSize);

        foreach (var region in MemoryMap.GetRegions())
        {
            nuint regionStart = (nuint)region.Start;
            nuint regionEnd = (nuint)region.End;
            if (regionEnd <= cursor)
                continue;
            if (regionStart > maximumAddress)
                break;

            nuint gapEnd = regionStart < limitEnd ? regionStart : limitEnd;
            if (cursor <= gapEnd && blockSize <= gapEnd - cursor && TryMapBlock(cursor, blockSize, out block))
                return true;

            cursor = MemoryArithmetics.AlignUp(cursor > regionEnd ? cursor : regionEnd, pageSize);
            if (cursor > maximumAddress)
                break;
        }

        if (cursor <= maximumAddress && blockSize - 1 <= maximumAddress - cursor && TryMapBlock(cursor, blockSize, out block))
            return true;

        block = default;
        return false;

        static bool TryMapBlock(nuint address, nuint size, out LinearMemoryBlock block)
        {
            void* p = NativeMethods.mmap(
                (void*)address,
                size,
                NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Execute,
                NativeMethods.MemoryMapFlags.Private | NativeMethods.MemoryMapFlags.Anonymous | NativeMethods.MemoryMapFlags.FixedNoReplace,
                -1,
                0);
            if (p == (void*)address)
            {
                block = new((byte*)p, size);
                return true;
            }
            if (p != (void*)(-1))
                _ = NativeMethods.munmap(p, size);

            // Kernels predating MAP_FIXED_NOREPLACE may reject or ignore that flag.
            // A plain hint is safe when its exact result is verified.
            p = NativeMethods.mmap(
                (void*)address,
                size,
                NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Execute,
                NativeMethods.MemoryMapFlags.Private | NativeMethods.MemoryMapFlags.Anonymous,
                -1,
                0);

            if (p == (void*)address)
            {
                block = new((byte*)p, size);
                return true;
            }

            if (p != (void*)(-1))
                _ = NativeMethods.munmap(p, size);

            block = default;
            return false;
        }
    }

    static readonly List<LinearMemoryBlock> m_NearBlocks = [];

    #endregion
}
