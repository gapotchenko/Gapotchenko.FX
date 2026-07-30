// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Runtime.CompilerServices.Pal.OS.MacOS;

#if NET
[SupportedOSPlatform("macos")]
#endif
static unsafe class TrampolineAllocator
{
    public static bool TryAllocateNear<T>(void* target, int count, nuint maximumDistance, out Span<T> allocation)
        where T : struct
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        int size = Align(checked(count * Unsafe.SizeOf<T>()));
        var blocks = m_Blocks;
        lock (blocks)
        {
            for (int i = 0; i < blocks.Count; ++i)
            {
                var block = blocks[i];
                if (block.AvailableSize >= (nuint)size && IsWithinDistance(target, block.Current, maximumDistance))
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
        nuint blockSize = AlignUp((nuint)Math.Max(Environment.SystemPageSize, minimumSize), pageSize);
        nuint targetAddress = (nuint)target;
        nuint minimumAddress = targetAddress > maximumDistance ? targetAddress - maximumDistance : 0;
        nuint maximumAddress = targetAddress <= nuint.MaxValue - maximumDistance ? targetAddress + maximumDistance - 1 : nuint.MaxValue;
        nuint limitEnd = maximumAddress == nuint.MaxValue ? maximumAddress : maximumAddress + 1;

        nuint cursor = AlignUp(minimumAddress, pageSize);
        uint task = NativeMethods.mach_task_self();
        while (cursor <= maximumAddress && blockSize - 1 <= maximumAddress - cursor)
        {
            ulong regionStart = cursor;
            uint infoCount = checked((uint)(Unsafe.SizeOf<NativeMethods.VmRegionBasicInfo64>() / sizeof(int)));
            int result = NativeMethods.mach_vm_region(task, ref regionStart, out ulong regionSize, 9, out _, ref infoCount, out _);
            nuint gapEnd = result == 0 ? checked((nuint)regionStart) : limitEnd;

            if (cursor <= gapEnd && blockSize <= gapEnd - cursor)
            {
                const int MapPrivate = 0x0002;
                const int MapJit = 0x0800;
                const int MapAnonymous = 0x1000;
                var protection = NativeMethods.MemoryProtection.Read | NativeMethods.MemoryProtection.Write | NativeMethods.MemoryProtection.Execute;
                void* p = NativeMethods.mmap((void*)cursor, blockSize, protection, MapPrivate | MapJit | MapAnonymous, -1, 0);
                if (p != (void*)(-1))
                {
                    if (IsWithinDistance(target, p, maximumDistance))
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
            cursor = AlignUp(regionEnd, pageSize);
        }

        block = default;
        return false;
    }

    static bool IsWithinDistance(void* x, void* y, nuint maximumDistance)
    {
        nuint a = (nuint)x;
        nuint b = (nuint)y;
        return a >= b ? a - b <= maximumDistance : b - a <= maximumDistance;
    }

    static int Align(int size)
    {
        const int alignment = 16;
        return checked((size + alignment - 1) & -alignment);
    }

    static nuint AlignUp(nuint value, nuint alignment)
    {
        return checked((value + alignment - 1) & ~(alignment - 1));
    }

}
