// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Math.Combinatorics;

/// <summary>
/// Provides Cartesian product operations.
/// </summary>
public static partial class CartesianProduct
{
    /// <summary>
    /// Returns a Cartesian product of specified sequences.
    /// </summary>
    /// <typeparam name="T">Type of sequence elements.</typeparam>
    /// <param name="sequences">The sequences.</param>
    /// <returns>The Cartesian product of the specified sequences.</returns>
    public static IResult<T> Of<T>(IEnumerable<IEnumerable<T>?> sequences)
    {
        if (sequences == null)
            throw new ArgumentNullException(nameof(sequences));

        return MultiplyAccelerated(sequences.Where(x => x != null)!);
    }

    /// <summary>
    /// Returns a Cartesian product of specified factors.
    /// </summary>
    /// <typeparam name="T">Type of factor items.</typeparam>
    /// <param name="first">The first factor.</param>
    /// <param name="second">The second factor.</param>
    /// <param name="rest">The rest of factors.</param>
    /// <returns>The Cartesian product of the specified factors.</returns>
    public static IResult<T> Of<T>(IEnumerable<T>? first, IEnumerable<T>? second, params IEnumerable<T>?[] rest)
    {
        if (rest == null)
            throw new ArgumentNullException(nameof(rest));

        return Of(new[] { first, second }.Concat(rest));
    }

    /// <summary>
    /// Returns a Cartesian product of two sequences by enumerating all possible combinations of sequence elements,
    /// and applying a user-defined projection to each combination.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="first">The first input sequence of elements.</param>
    /// <param name="second">The second input sequence of elements.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input sequence elements to an element of a resulting sequence.</param>
    /// <returns>The Cartesian product of two input sequences with user-defined projection applied.</returns>
    public static IEnumerable<TResult> Of<TFirst, TSecond, TResult>(
        IEnumerable<TFirst>? first,
        IEnumerable<TSecond>? second,
        Func<TFirst, TSecond, TResult> resultSelector)
    {
        if (resultSelector == null)
            throw new ArgumentNullException(nameof(resultSelector));

        if (first == null ||
            second == null)
        {
            return [];
        }

        return Multiply(first, second, resultSelector);
    }
}
