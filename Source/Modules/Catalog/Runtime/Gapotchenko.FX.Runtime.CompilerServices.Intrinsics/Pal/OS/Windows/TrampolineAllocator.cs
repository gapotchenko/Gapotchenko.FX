// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Runtime.CompilerServices.Utils;
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
        nuint blockSize = (nuint)Math.Max(Environment.SystemPageSize, minimumSize);
        void* p = NativeMethods.VirtualAlloc(
            null,
            blockSize,
            NativeMethods.VirtualAllocationType.Reserve | NativeMethods.VirtualAllocationType.Commit,
            NativeMethods.PageProtect.ExecuteRead);

        if (p == null)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        return new LinearMemoryBlock((byte*)p, blockSize);
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

        int size = MemoryArithmetics.Align16(checked(count * Unsafe.SizeOf<T>()));

        var nearBlocks = m_NearBlocks;
        lock (nearBlocks)
        {
            for (int i = 0; i < nearBlocks.Count; ++i)
            {
                var block = nearBlocks[i];
                if (block.AvailableSize >= (nuint)size &&
                    MemoryArithmetics.IsWithinDistance((nint)target, (nint)block.Current, maximumDistance))
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

    static readonly List<LinearMemoryBlock> m_NearBlocks = [];

    static bool TryAllocateNearBlock(void* target, int minimumSize, nuint maximumDistance, out LinearMemoryBlock block)
    {
        const nuint AllocationGranularity = 64 * 1024;

        nuint blockSize = (nuint)MemoryArithmetics.AlignUp(Math.Max(Environment.SystemPageSize, minimumSize), Environment.SystemPageSize);
        nuint targetAddress = (nuint)target;
        nuint minimumAddress = targetAddress > maximumDistance ? targetAddress - maximumDistance : 0;
        nuint maximumAddress = targetAddress <= nuint.MaxValue - maximumDistance ? targetAddress + maximumDistance - 1 : nuint.MaxValue;

        minimumAddress = MemoryArithmetics.AlignUp(minimumAddress, AllocationGranularity);
        maximumAddress = maximumAddress == nuint.MaxValue ? maximumAddress : MemoryArithmetics.AlignDown(maximumAddress + 1, AllocationGranularity) - 1;
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
            blockSize,
            NativeMethods.VirtualAllocationType.Reserve | NativeMethods.VirtualAllocationType.Commit,
            NativeMethods.PageProtect.ExecuteRead,
            &extendedParameter,
            1);

        if (p == null)
        {
            block = default;
            return false;
        }

        block = new LinearMemoryBlock((byte*)p, blockSize);
        return true;
    }

    #endregion
}
