﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Math.Combinatorics;

/// <summary>
/// Provides permutation operations.
/// </summary>
public static partial class Permutations
{
    /// <summary>
    /// Returns all possible permutations of elements from a sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements of sequence.</typeparam>
    /// <param name="sequence">The sequence.</param>
    /// <returns>An enumerable that contains all possible permutations of elements from the sequence.</returns>
    public static IResult<T> Of<T>(IEnumerable<T> sequence)
    {
        if (sequence == null)
            throw new ArgumentNullException(nameof(sequence));

        return PermuteAccelerated(sequence);
    }
}
