// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Diagnostics;

namespace Gapotchenko.FX.Runtime.CompilerServices.Utils;

unsafe struct LinearMemoryBlock(byte* current, nuint size)
{
    public byte* Allocate(int size)
    {
        Debug.Assert(size > 0);

        byte* p = Current;
        Current += size;

        Debug.Assert(Current <= End);

        return p;
    }

    public readonly nuint AvailableSize => (nuint)(End - Current);

    public byte* Current { get; private set; } = current;

    public byte* End { get; } = current + size;
}
