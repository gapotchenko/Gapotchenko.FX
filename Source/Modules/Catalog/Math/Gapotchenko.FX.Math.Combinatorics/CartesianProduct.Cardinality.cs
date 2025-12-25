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
    /// <param name="lengths">The lengths of multiplied factors.</param>
    /// <returns>A Cartesian product cardinality.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lengths"/> is <see langword="null"/>.</exception>
    /// <exception cref="OverflowException">The result is too big to fit into <see cref="int"/>.</exception>
    public static int Cardinality(params IEnumerable<int> lengths)
    {
        ArgumentNullException.ThrowIfNull(lengths);

        int cardinality = 1;
        bool hasFactor = false;

        foreach (int length in lengths)
        {
            if (length == 0)
                return 0;

            cardinality = checked(cardinality * length);
            hasFactor = true;
        }

        if (hasFactor)
            return cardinality;
        else
            return 0;
    }

    /// <inheritdoc cref="Cardinality(IEnumerable{long})"/>
    public static long Cardinality(params long[] lengths) => Cardinality((IEnumerable<long>)lengths);

    /// <summary>
    /// <inheritdoc cref="Cardinality(IEnumerable{int})"/>
    /// </summary>
    /// <param name="lengths"><inheritdoc cref="Cardinality(IEnumerable{int})"/></param>
    /// <returns><inheritdoc cref="Cardinality(IEnumerable{int})"/></returns>
    /// <exception cref="ArgumentNullException"><paramref name="lengths"/> is <see langword="null"/>.</exception>
    /// <exception cref="OverflowException">The result is too big to fit into <see cref="long"/>.</exception>
    public static long Cardinality(IEnumerable<long> lengths)
    {
        ArgumentNullException.ThrowIfNull(lengths);

        long cardinality = 1;
        bool hasFactor = false;

        foreach (long length in lengths)
        {
            if (length == 0)
                return 0;

            cardinality = checked(cardinality * length);
            hasFactor = true;
        }

        if (hasFactor)
            return cardinality;
        else
            return 0;
    }
}
