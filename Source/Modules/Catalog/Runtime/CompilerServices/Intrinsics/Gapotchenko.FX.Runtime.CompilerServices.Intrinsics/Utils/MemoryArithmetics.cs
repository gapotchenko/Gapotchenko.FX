// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices.Utils;

static class MemoryArithmetics
{
    public static int Align16(int size)
    {
        const int alignment = 16;
        return checked((size + alignment - 1) & -alignment);
    }

    public static int Align2(int value) => checked((value + 1) & ~1);

    public static int Align4(int value) => checked((value + 3) & ~3);

    public static int AlignUp(int value, int alignment)
    {
        return checked((value + alignment - 1) / alignment * alignment);
    }

    public static nuint AlignUp(nuint value, nuint alignment)
    {
        return checked((value + alignment - 1) & ~(alignment - 1));
    }

    public static nuint AlignDown(nuint value, nuint alignment)
    {
        return value & ~(alignment - 1);
    }

    public static bool IsWithinDistance(nint x, nint y, nuint maximumDistance)
    {
        nuint a = (nuint)x;
        nuint b = (nuint)y;
        return a >= b ? a - b <= maximumDistance : b - a <= maximumDistance;
    }
}
