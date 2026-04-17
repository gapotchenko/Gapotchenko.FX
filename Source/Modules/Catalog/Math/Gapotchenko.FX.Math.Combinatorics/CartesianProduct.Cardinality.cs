// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Math.Combinatorics;

partial class CartesianProduct
{
    /// <inheritdoc cref="Cardinality(IEnumerable{int})"/>
    public static int Cardinality(params int[] lengths) => Cardinality((IEnumerable<int>)lengths);

    /// <summary>
    /// Returns a Cartesian product cardinality for the specified lengths of multiplied factors.
    /// </summary>
    /// <remarks>
    /// The Cartesian product cardinality of zero factors is 1.
    /// </remarks>
    /// <param name="lengths">The lengths of multiplied factors.</param>
    /// <returns>A Cartesian product cardinality.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lengths"/> is <see langword="null"/>.</exception>
    /// <exception cref="OverflowException">The result is too big to fit into <see cref="int"/>.</exception>
    public static int Cardinality(params IEnumerable<int> lengths)
    {
        ArgumentNullException.ThrowIfNull(lengths);

        int cardinality = 1;

        foreach (int length in lengths)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(length, nameof(lengths));
            cardinality = checked(cardinality * length);
        }

        return cardinality;
    }

    /// <inheritdoc cref="Cardinality(IEnumerable{long})"/>
    public static long Cardinality(params long[] lengths) => Cardinality((IEnumerable<long>)lengths);

    /// <summary>
    /// <inheritdoc cref="Cardinality(IEnumerable{int})"/>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="Cardinality(IEnumerable{int})"/>
    /// </remarks>
    /// <param name="lengths"><inheritdoc cref="Cardinality(IEnumerable{int})"/></param>
    /// <returns><inheritdoc cref="Cardinality(IEnumerable{int})"/></returns>
    /// <exception cref="ArgumentNullException"><paramref name="lengths"/> is <see langword="null"/>.</exception>
    /// <exception cref="OverflowException">The result is too big to fit into <see cref="long"/>.</exception>
    public static long Cardinality(IEnumerable<long> lengths)
    {
        ArgumentNullException.ThrowIfNull(lengths);

        long cardinality = 1;

        foreach (long length in lengths)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(length, nameof(lengths));
            cardinality = checked(cardinality * length);
        }

        return cardinality;
    }
}
