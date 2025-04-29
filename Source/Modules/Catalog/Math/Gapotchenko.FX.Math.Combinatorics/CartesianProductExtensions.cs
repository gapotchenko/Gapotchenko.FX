// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Math.Combinatorics;

/// <summary>
/// Provides LINQ extension methods for Cartesian product.
/// </summary>
/// <seealso cref="CartesianProduct"/>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class CartesianProductExtensions
{
    /// <summary>
    /// Returns a Cartesian product of sequences by enumerating all possible combinations of sequence elements.
    /// </summary>
    /// <typeparam name="T">Type of sequence elements.</typeparam>
    /// <param name="factors">The input sequences.</param>
    /// <returns>The Cartesian product of input sequences.</returns>
    public static CartesianProduct.IResult<T> CrossJoin<T>(this IEnumerable<IEnumerable<T>> factors)
    {
        if (factors == null)
            throw new ArgumentNullException(nameof(factors));

        return CartesianProduct.MultiplyAccelerated(
            factors.Select(x => x ?? throw new ArgumentException("A Cartesian product factor cannot be null.", nameof(factors))));
    }

    /// <summary>
    /// Returns a Cartesian product of two sequences by enumerating all possible combinations of sequence elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="first">The first input sequence of elements.</param>
    /// <param name="second">The second input sequence of elements.</param>
    /// <returns>The Cartesian product of two input sequences.</returns>
    public static CartesianProduct.IResult<TSource> CrossJoin<TSource>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second)
    {
        if (first == null)
            throw new ArgumentNullException(nameof(first));
        if (second == null)
            throw new ArgumentNullException(nameof(second));

        return CartesianProduct.MultiplyAccelerated([first, second]);
    }

    /// <summary>
    /// Returns a Cartesian product of three sequences by enumerating all possible combinations of sequence elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="first">The first input sequence of elements.</param>
    /// <param name="second">The second input sequence of elements.</param>
    /// <param name="third">The third input sequence of elements.</param>
    /// <returns>The Cartesian product of three input sequences.</returns>
    public static CartesianProduct.IResult<TSource> CrossJoin<TSource>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        IEnumerable<TSource> third)
    {
        if (first == null)
            throw new ArgumentNullException(nameof(first));
        if (second == null)
            throw new ArgumentNullException(nameof(second));
        if (third == null)
            throw new ArgumentNullException(nameof(third));

        return CartesianProduct.MultiplyAccelerated([first, second, third]);
    }

    /// <summary>
    /// Returns a Cartesian product of four sequences by enumerating all possible combinations of sequence elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="first">The first input sequence of elements.</param>
    /// <param name="second">The second input sequence of elements.</param>
    /// <param name="third">The third input sequence of elements.</param>
    /// <param name="fourth">The fourth input sequence of elements.</param>
    /// <returns>The Cartesian product of four input sequences.</returns>
    public static CartesianProduct.IResult<TSource> CrossJoin<TSource>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        IEnumerable<TSource> third,
        IEnumerable<TSource> fourth)
    {
        if (first == null)
            throw new ArgumentNullException(nameof(first));
        if (second == null)
            throw new ArgumentNullException(nameof(second));
        if (third == null)
            throw new ArgumentNullException(nameof(third));
        if (fourth == null)
            throw new ArgumentNullException(nameof(fourth));

        return CartesianProduct.MultiplyAccelerated([first, second, third, fourth]);
    }

    /// <summary>
    /// Returns a Cartesian product of sequences by enumerating all possible combinations of sequence elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="first">The first input sequence of elements.</param>
    /// <param name="second">The second input sequence of elements.</param>
    /// <param name="third">The third input sequence of elements.</param>
    /// <param name="fourth">The fourth input sequence of elements.</param>
    /// <param name="rest">The rest of input sequences of elements.</param>
    /// <returns>The Cartesian product of input sequences.</returns>
    public static CartesianProduct.IResult<TSource> CrossJoin<TSource>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        IEnumerable<TSource> third,
        IEnumerable<TSource> fourth,
        params IEnumerable<TSource>[] rest)
    {
        if (first == null)
            throw new ArgumentNullException(nameof(first));
        if (second == null)
            throw new ArgumentNullException(nameof(second));
        if (third == null)
            throw new ArgumentNullException(nameof(third));
        if (fourth == null)
            throw new ArgumentNullException(nameof(fourth));
        if (rest == null)
            throw new ArgumentNullException(nameof(rest));

        return CartesianProduct.MultiplyAccelerated(new[] { first, second, third, fourth }.Concat(rest));
    }

    /// <summary>
    /// Returns a Cartesian product of two sequences by enumerating all possible combinations of sequence elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="first">The first input sequence of elements.</param>
    /// <param name="second">The second input sequence of elements.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input sequence elements to an element of a resulting sequence.</param>
    /// <returns>The Cartesian product of two input sequences with user-defined projection applied.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="first"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="second"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        Func<TFirst, TSecond, TResult> resultSelector) =>
        CartesianProduct.Multiply(
            first ?? throw new ArgumentNullException(nameof(first)),
            second ?? throw new ArgumentNullException(nameof(second)),
            resultSelector ?? throw new ArgumentNullException(nameof(resultSelector)));

    /// <summary>
    /// Returns a Cartesian product of three sequences by enumerating all possible combinations of sequence elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TThird">The type of the elements of the third input sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="first">The first input sequence of elements.</param>
    /// <param name="second">The second input sequence of elements.</param>
    /// <param name="third">The third input sequence of elements.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input sequence elements to an element of a resulting sequence.</param>
    /// <returns>The Cartesian product of two input sequences with user-defined projection applied.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="first"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="second"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="third"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TThird, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        Func<TFirst, TSecond, TThird, TResult> resultSelector) =>
        CartesianProduct.Multiply(
            first ?? throw new ArgumentNullException(nameof(first)),
            second ?? throw new ArgumentNullException(nameof(second)),
            third ?? throw new ArgumentNullException(nameof(third)),
            resultSelector ?? throw new ArgumentNullException(nameof(resultSelector)));

    /// <summary>
    /// Returns a Cartesian product of four sequences by enumerating all possible combinations of sequence elements,
    /// and applying a user-defined projection to each combination to produce a sequence of results.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TThird">The type of the elements of the third input sequence.</typeparam>
    /// <typeparam name="TFourth">The type of the elements of the fourth input sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="first">The first input sequence of elements.</param>
    /// <param name="second">The second input sequence of elements.</param>
    /// <param name="third">The third input sequence of elements.</param>
    /// <param name="fourth">The fourth input sequence of elements.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input sequence elements to an element of a resulting sequence.</param>
    /// <returns>The Cartesian product of two input sequences with user-defined projection applied.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="first"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="second"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="third"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fourth"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="resultSelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TThird, TFourth, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        IEnumerable<TFourth> fourth,
        Func<TFirst, TSecond, TThird, TFourth, TResult> resultSelector) =>
        CartesianProduct.Multiply(
            first ?? throw new ArgumentNullException(nameof(first)),
            second ?? throw new ArgumentNullException(nameof(second)),
            third ?? throw new ArgumentNullException(nameof(third)),
            fourth ?? throw new ArgumentNullException(nameof(fourth)),
            resultSelector ?? throw new ArgumentNullException(nameof(resultSelector)));
}
