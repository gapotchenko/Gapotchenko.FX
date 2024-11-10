// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Math.Combinatorics;

partial class Permutations
{
    /// <summary>
    /// Returns a total number of permutations for a multiset of the specified length.
    /// </summary>
    /// <param name="length">The length of a multiset.</param>
    /// <returns>A total number of permutations.</returns>
    /// <exception cref="OverflowException">The result is too big to fit into <see cref="int"/>.</exception>
    public static int Cardinality(int length) => MathEx.Factorial(length);

    /// <summary>
    /// <inheritdoc cref="Cardinality(int)"/>
    /// </summary>
    /// <param name="length"><inheritdoc cref="Cardinality(int)"/></param>
    /// <returns><inheritdoc cref="Cardinality(int)"/></returns>
    /// <exception cref="OverflowException">The result is too big to fit into <see cref="long"/>.</exception>
    public static long Cardinality(long length) => MathEx.Factorial(length);
}
