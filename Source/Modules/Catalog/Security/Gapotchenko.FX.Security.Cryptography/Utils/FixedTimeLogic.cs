// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Security.Cryptography.Utils;

static class FixedTimeLogic
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int IsZero(int value) => (value - 1 >> 31) & 1;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int Select(int condition, int whenTrue, int whenFalse)
    {
        int mask = -condition;
        return whenFalse ^ ((whenFalse ^ whenTrue) & mask);
    }
}
