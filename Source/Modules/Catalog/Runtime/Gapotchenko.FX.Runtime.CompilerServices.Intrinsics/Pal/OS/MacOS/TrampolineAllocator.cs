// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Utils;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

#if NET
[SupportedOSPlatform("macos")]
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
            NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute,
            NativeMethods.MemoryMapFlags.Private | NativeMethods.MemoryMapFlags.Jit | NativeMethods.MemoryMapFlags.Anonymous,
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

    public static bool TryAllocateNear<T>(void* target, int count, nuint maximumDistance, out Span<T> allocation)
        where T : struct
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        int size = MemoryArithmetics.Align16(checked(count * Unsafe.SizeOf<T>()));
        var blocks = m_Blocks;
        lock (blocks)
        {
            for (int i = 0; i < blocks.Count; ++i)
            {
                var block = blocks[i];
                if (block.AvailableSize >= (nuint)size &&
                    MemoryArithmetics.IsWithinDistance((nint)target, (nint)block.Current, maximumDistance))
                {
                    allocation = new(block.Allocate(size), count);
                    blocks[i] = block;
                    return true;
                }
            }

            if (!TryAllocateNearBlock(target, size, maximumDistance, out var newBlock))
            {
                allocation = [];
                return false;
            }

            allocation = new(newBlock.Allocate(size), count);
            blocks.Add(newBlock);
            return true;
        }
    }

    static readonly List<LinearMemoryBlock> m_Blocks = [];

    static bool TryAllocateNearBlock(void* target, int minimumSize, nuint maximumDistance, out LinearMemoryBlock block)
    {
        nuint pageSize = (nuint)Environment.SystemPageSize;
        nuint blockSize = MemoryArithmetics.AlignUp((nuint)Math.Max(Environment.SystemPageSize, minimumSize), pageSize);
        nuint targetAddress = (nuint)target;
        nuint minimumAddress = targetAddress > maximumDistance ? targetAddress - maximumDistance : 0;
        nuint maximumAddress = targetAddress <= nuint.MaxValue - maximumDistance ? targetAddress + maximumDistance - 1 : nuint.MaxValue;
        nuint limitEnd = maximumAddress == nuint.MaxValue ? maximumAddress : maximumAddress + 1;

        nuint cursor = MemoryArithmetics.AlignUp(minimumAddress, pageSize);
        uint task = NativeMethods.mach_task_self();
        while (cursor <= maximumAddress && blockSize - 1 <= maximumAddress - cursor)
        {
            ulong regionStart = cursor;
            uint infoCount = checked((uint)(Unsafe.SizeOf<NativeMethods.VmRegionBasicInfo64>() / sizeof(int)));
            int result = NativeMethods.mach_vm_region(task, ref regionStart, out ulong regionSize, 9, out _, ref infoCount, out _);
            nuint gapEnd = result == 0 ? checked((nuint)regionStart) : limitEnd;

            if (cursor <= gapEnd && blockSize <= gapEnd - cursor)
            {
                void* p = NativeMethods.mmap(
                    (void*)cursor,
                    blockSize,
                    NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute,
                    NativeMethods.MemoryMapFlags.Private | NativeMethods.MemoryMapFlags.Jit | NativeMethods.MemoryMapFlags.Anonymous,
                    -1,
                    0);
                if (p != (void*)(-1))
                {
                    if (MemoryArithmetics.IsWithinDistance((nint)target, (nint)p, maximumDistance))
                    {
                        block = new((byte*)p, blockSize);
                        return true;
                    }
                    else
                    {
                        _ = NativeMethods.munmap(p, blockSize);
                    }
                }
            }

            if (result != 0)
                break;

            nuint regionEnd = checked((nuint)regionStart + (nuint)regionSize);
            if (regionEnd <= cursor)
                break;
            cursor = MemoryArithmetics.AlignUp(regionEnd, pageSize);
        }

        block = default;
        return false;
    }

    #endregion
}
