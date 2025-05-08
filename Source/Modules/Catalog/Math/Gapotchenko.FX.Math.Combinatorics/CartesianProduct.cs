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
    /// Returns a Cartesian product of the specified factors.
    /// </summary>
    /// <typeparam name="T">Type of factor items.</typeparam>
    /// <param name="factor1">The first factor.</param>
    /// <param name="factor2">The second factor.</param>
    /// <param name="rest">The rest of factors.</param>
    /// <returns>An <see cref="IResultCollection{T}"/> instance representing the sequence of Cartesian product results.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="factor1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor2"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="rest"/> is <see langword="null"/>.</exception>
    public static IResultCollection<T> Of<T>(IEnumerable<T> factor1, IEnumerable<T> factor2, params IEnumerable<T>[] rest)
    {
        if (factor1 == null)
            throw new ArgumentNullException(nameof(factor1));
        if (factor2 == null)
            throw new ArgumentNullException(nameof(factor2));
        if (rest == null)
            throw new ArgumentNullException(nameof(rest));

        return Of(new[] { factor1, factor2 }.Concat(rest));
    }

    /// <summary>
    /// Returns a Cartesian product of the specified factors.
    /// </summary>
    /// <typeparam name="T">Type of factor items.</typeparam>
    /// <param name="factors">The factors.</param>
    /// <returns>An <see cref="IResultCollection{T}"/> instance representing the sequence of Cartesian product results.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="factors"/> is <see langword="null"/>.</exception>
    public static IResultCollection<T> Of<T>(IEnumerable<IEnumerable<T>> factors)
    {
        if (factors == null)
            throw new ArgumentNullException(nameof(factors));

        factors = factors.Select(x => x ?? throw new ArgumentException("A Cartesian product factor cannot be null.", nameof(factors)));

        return MultiplyAccelerated(factors);
    }

    /// <summary>
    /// Returns a Cartesian product of two factors by enumerating all possible combinations of factor elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="T1">The type of the elements of the first factor.</typeparam>
    /// <typeparam name="T2">The type of the elements of the second factor.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="factor1">The first factor.</param>
    /// <param name="factor2">The second factor.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input factor elements to an element of a resulting sequence.</param>
    /// <returns>
    /// An <see cref="IResultCollection{T}"/> instance representing the sequence of Cartesian product results
    /// of two factors with user-defined projection applied.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="factor1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor2"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> Of<T1, T2, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        Func<T1, T2, TResult> resultSelector)
    {
        if (factor1 == null)
            throw new ArgumentNullException(nameof(factor1));
        if (factor2 == null)
            throw new ArgumentNullException(nameof(factor2));
        if (resultSelector == null)
            throw new ArgumentNullException(nameof(resultSelector));

        return Multiply(factor1, factor2, resultSelector);
    }

    /// <summary>
    /// Returns a Cartesian product of three factors by enumerating all possible combinations of factor elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="T1">The type of the elements of the first factor.</typeparam>
    /// <typeparam name="T2">The type of the elements of the second factor.</typeparam>
    /// <typeparam name="T3">The type of the elements of the third factor.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="factor1">The first factor.</param>
    /// <param name="factor2">The second factor.</param>
    /// <param name="factor3">The third factor.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input factor elements to an element of a resulting sequence.</param>
    /// <returns>
    /// An <see cref="IResultCollection{T}"/> instance representing the sequence of Cartesian product results
    /// of three factors with user-defined projection applied.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="factor1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor2"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor3"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> Of<T1, T2, T3, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        Func<T1, T2, T3, TResult> resultSelector) =>
        Multiply(
            factor1 ?? throw new ArgumentNullException(nameof(factor1)),
            factor2 ?? throw new ArgumentNullException(nameof(factor2)),
            factor3 ?? throw new ArgumentNullException(nameof(factor3)),
            resultSelector ?? throw new ArgumentNullException(nameof(resultSelector)));

    /// <summary>
    /// Returns a Cartesian product of four factors by enumerating all possible combinations of factor elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="T1">The type of the elements of the first factor.</typeparam>
    /// <typeparam name="T2">The type of the elements of the second factor.</typeparam>
    /// <typeparam name="T3">The type of the elements of the third factor.</typeparam>
    /// <typeparam name="T4">The type of the elements of the fourth factor.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="factor1">The first factor.</param>
    /// <param name="factor2">The second factor.</param>
    /// <param name="factor3">The third factor.</param>
    /// <param name="factor4">The fourth factor.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input factor elements to an element of a resulting sequence.</param>
    /// <returns>
    /// An <see cref="IResultCollection{T}"/> instance representing the sequence of Cartesian product results
    /// of four factors with user-defined projection applied.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="factor1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor2"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor3"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor4"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> Of<T1, T2, T3, T4, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        Func<T1, T2, T3, T4, TResult> resultSelector) =>
        Multiply(
            factor1 ?? throw new ArgumentNullException(nameof(factor1)),
            factor2 ?? throw new ArgumentNullException(nameof(factor2)),
            factor3 ?? throw new ArgumentNullException(nameof(factor3)),
            factor4 ?? throw new ArgumentNullException(nameof(factor4)),
            resultSelector ?? throw new ArgumentNullException(nameof(resultSelector)));

    /// <summary>
    /// Returns a Cartesian product of five factors by enumerating all possible combinations of factor elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="T1">The type of the elements of the first factor.</typeparam>
    /// <typeparam name="T2">The type of the elements of the second factor.</typeparam>
    /// <typeparam name="T3">The type of the elements of the third factor.</typeparam>
    /// <typeparam name="T4">The type of the elements of the fourth factor.</typeparam>
    /// <typeparam name="T5">The type of the elements of the fifth factor.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="factor1">The first factor.</param>
    /// <param name="factor2">The second factor.</param>
    /// <param name="factor3">The third factor.</param>
    /// <param name="factor4">The fourth factor.</param>
    /// <param name="factor5">The fifth factor.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input factor elements to an element of a resulting sequence.</param>
    /// <returns>
    /// An <see cref="IResultCollection{T}"/> instance representing the sequence of Cartesian product results
    /// of five factors with user-defined projection applied.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="factor1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor2"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor3"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor4"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor5"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> Of<T1, T2, T3, T4, T5, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        IEnumerable<T5> factor5,
        Func<T1, T2, T3, T4, T5, TResult> resultSelector) =>
        Multiply(
            factor1 ?? throw new ArgumentNullException(nameof(factor1)),
            factor2 ?? throw new ArgumentNullException(nameof(factor2)),
            factor3 ?? throw new ArgumentNullException(nameof(factor3)),
            factor4 ?? throw new ArgumentNullException(nameof(factor4)),
            factor5 ?? throw new ArgumentNullException(nameof(factor5)),
            resultSelector ?? throw new ArgumentNullException(nameof(resultSelector)));

    /// <summary>
    /// Returns a Cartesian product of six factors by enumerating all possible combinations of factor elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="T1">The type of the elements of the first factor.</typeparam>
    /// <typeparam name="T2">The type of the elements of the second factor.</typeparam>
    /// <typeparam name="T3">The type of the elements of the third factor.</typeparam>
    /// <typeparam name="T4">The type of the elements of the fourth factor.</typeparam>
    /// <typeparam name="T5">The type of the elements of the fifth factor.</typeparam>
    /// <typeparam name="T6">The type of the elements of the sixth factor.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="factor1">The first factor.</param>
    /// <param name="factor2">The second factor.</param>
    /// <param name="factor3">The third factor.</param>
    /// <param name="factor4">The fourth factor.</param>
    /// <param name="factor5">The fifth factor.</param>
    /// <param name="factor6">The sixth factor.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input factor elements to an element of a resulting sequence.</param>
    /// <returns>
    /// An <see cref="IResultCollection{T}"/> instance representing the sequence of Cartesian product results
    /// of six factors with user-defined projection applied.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="factor1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor2"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor3"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor4"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor5"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor6"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> Of<T1, T2, T3, T4, T5, T6, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        IEnumerable<T5> factor5,
        IEnumerable<T6> factor6,
        Func<T1, T2, T3, T4, T5, T6, TResult> resultSelector) =>
        Multiply(
            factor1 ?? throw new ArgumentNullException(nameof(factor1)),
            factor2 ?? throw new ArgumentNullException(nameof(factor2)),
            factor3 ?? throw new ArgumentNullException(nameof(factor3)),
            factor4 ?? throw new ArgumentNullException(nameof(factor4)),
            factor5 ?? throw new ArgumentNullException(nameof(factor5)),
            factor6 ?? throw new ArgumentNullException(nameof(factor6)),
            resultSelector ?? throw new ArgumentNullException(nameof(resultSelector)));

    /// <summary>
    /// Returns a Cartesian product of seven factors by enumerating all possible combinations of factor elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="T1">The type of the elements of the first factor.</typeparam>
    /// <typeparam name="T2">The type of the elements of the second factor.</typeparam>
    /// <typeparam name="T3">The type of the elements of the third factor.</typeparam>
    /// <typeparam name="T4">The type of the elements of the fourth factor.</typeparam>
    /// <typeparam name="T5">The type of the elements of the fifth factor.</typeparam>
    /// <typeparam name="T6">The type of the elements of the sixth factor.</typeparam>
    /// <typeparam name="T7">The type of the elements of the seventh factor.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="factor1">The first factor.</param>
    /// <param name="factor2">The second factor.</param>
    /// <param name="factor3">The third factor.</param>
    /// <param name="factor4">The fourth factor.</param>
    /// <param name="factor5">The fifth factor.</param>
    /// <param name="factor6">The sixth factor.</param>
    /// <param name="factor7">The seventh factor.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input factor elements to an element of a resulting sequence.</param>
    /// <returns>
    /// An <see cref="IResultCollection{T}"/> instance representing the sequence of Cartesian product results
    /// of seven factors with user-defined projection applied.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="factor1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor2"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor3"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor4"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor5"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor6"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor7"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> Of<T1, T2, T3, T4, T5, T6, T7, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        IEnumerable<T5> factor5,
        IEnumerable<T6> factor6,
        IEnumerable<T7> factor7,
        Func<T1, T2, T3, T4, T5, T6, T7, TResult> resultSelector) =>
        Multiply(
            factor1 ?? throw new ArgumentNullException(nameof(factor1)),
            factor2 ?? throw new ArgumentNullException(nameof(factor2)),
            factor3 ?? throw new ArgumentNullException(nameof(factor3)),
            factor4 ?? throw new ArgumentNullException(nameof(factor4)),
            factor5 ?? throw new ArgumentNullException(nameof(factor5)),
            factor6 ?? throw new ArgumentNullException(nameof(factor6)),
            factor7 ?? throw new ArgumentNullException(nameof(factor7)),
            resultSelector ?? throw new ArgumentNullException(nameof(resultSelector)));

    /// <summary>
    /// Returns a Cartesian product of eight factors by enumerating all possible combinations of factor elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="T1">The type of the elements of the first factor.</typeparam>
    /// <typeparam name="T2">The type of the elements of the second factor.</typeparam>
    /// <typeparam name="T3">The type of the elements of the third factor.</typeparam>
    /// <typeparam name="T4">The type of the elements of the fourth factor.</typeparam>
    /// <typeparam name="T5">The type of the elements of the fifth factor.</typeparam>
    /// <typeparam name="T6">The type of the elements of the sixth factor.</typeparam>
    /// <typeparam name="T7">The type of the elements of the seventh factor.</typeparam>
    /// <typeparam name="T8">The type of the elements of the eighth factor.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="factor1">The first factor.</param>
    /// <param name="factor2">The second factor.</param>
    /// <param name="factor3">The third factor.</param>
    /// <param name="factor4">The fourth factor.</param>
    /// <param name="factor5">The fifth factor.</param>
    /// <param name="factor6">The sixth factor.</param>
    /// <param name="factor7">The seventh factor.</param>
    /// <param name="factor8">The eighth factor.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input factor elements to an element of a resulting sequence.</param>
    /// <returns>
    /// An <see cref="IResultCollection{T}"/> instance representing the sequence of Cartesian product results
    /// of eight factors with user-defined projection applied.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="factor1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor2"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor3"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor4"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor5"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor6"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor7"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="factor8"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> Of<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        IEnumerable<T1> factor1,
        IEnumerable<T2> factor2,
        IEnumerable<T3> factor3,
        IEnumerable<T4> factor4,
        IEnumerable<T5> factor5,
        IEnumerable<T6> factor6,
        IEnumerable<T7> factor7,
        IEnumerable<T8> factor8,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resultSelector) =>
        Multiply(
            factor1 ?? throw new ArgumentNullException(nameof(factor1)),
            factor2 ?? throw new ArgumentNullException(nameof(factor2)),
            factor3 ?? throw new ArgumentNullException(nameof(factor3)),
            factor4 ?? throw new ArgumentNullException(nameof(factor4)),
            factor5 ?? throw new ArgumentNullException(nameof(factor5)),
            factor6 ?? throw new ArgumentNullException(nameof(factor6)),
            factor7 ?? throw new ArgumentNullException(nameof(factor7)),
            factor8 ?? throw new ArgumentNullException(nameof(factor8)),
            resultSelector ?? throw new ArgumentNullException(nameof(resultSelector)));
}
