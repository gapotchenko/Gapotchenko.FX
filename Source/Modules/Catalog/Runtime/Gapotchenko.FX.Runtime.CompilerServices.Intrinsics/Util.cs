// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Runtime.CompilerServices;

static class Util
{
    public static bool HasPrologue<T>(ReadOnlySpan<T> data, T[][] prologues)
        where T : IEquatable<T>?
    {
        foreach (var prologue in prologues)
        {
            if (data.StartsWith(prologue))
                return true;
        }
        return false;
    }

    public static int GetGuaranteedInstructionCount<T>(ReadOnlySpan<T> data, (T[] Instructions, int Size)[] prologues)
        where T : IEquatable<T>?
    {
        foreach (var prologue in prologues)
        {
            if (data.StartsWith(prologue.Instructions))
                return prologue.Size;
        }
        return -1;
    }
}
