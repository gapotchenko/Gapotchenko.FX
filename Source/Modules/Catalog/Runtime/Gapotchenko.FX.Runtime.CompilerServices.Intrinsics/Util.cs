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

    /// <summary>
    /// Gets the count of instructions guaranteed to be patchable
    /// in a method with the specified prologues.
    /// </summary>
    /// <typeparam name="T">The type of instructions in the CPU instruction stream.</typeparam>
    /// <param name="data">The method instructions.</param>
    /// <param name="prologues">The set of prologues.</param>
    /// <returns>
    /// The count of instructions guaranteed to be patchable,
    /// or <c>-1</c> when the method prologue is not recognized.
    /// </returns>
    public static int GetPatchablePrologueSize<T>(
        ReadOnlySpan<T> data,
        (T[] Instructions, int Size /* count of patchable instructions */)[] prologues)
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
