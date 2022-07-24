using System.ComponentModel;

namespace Gapotchenko.FX.Math.Combinatorics;

/// <summary>
/// Cartesian product LINQ extensions.
/// </summary>
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
            factors.Select(x => (x ?? throw new ArgumentNullException(nameof(factors), "A Cartesian product factor cannot be null."))));
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

        return CartesianProduct.MultiplyAccelerated(new[] { first, second });
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

        return CartesianProduct.MultiplyAccelerated(new[] { first, second, third });
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

        return CartesianProduct.MultiplyAccelerated(new[] { first, second, third, fourth });
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
    /// and applying a user-defined projection to each combination.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
    /// <param name="first">The first input sequence of elements.</param>
    /// <param name="second">The second input sequence of elements.</param>
    /// <param name="resultSelector">The projection function that maps a combination of input sequence elements to an element of a resulting sequence.</param>
    /// <returns>The Cartesian product of two input sequences with user-defined projection applied.</returns>
    public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        Func<TFirst, TSecond, TResult> resultSelector)
    {
        if (first == null)
            throw new ArgumentNullException(nameof(first));
        if (second == null)
            throw new ArgumentNullException(nameof(second));
        if (resultSelector == null)
            throw new ArgumentNullException(nameof(resultSelector));

        return CartesianProduct.Multiply(first, second, resultSelector);
    }

    /// <summary>
    /// Returns a Cartesian product of three sequences by enumerating all possible combinations of sequence elements,
    /// and applying a user-defined projection to each combination.
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
    public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TThird, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        Func<TFirst, TSecond, TThird, TResult> resultSelector)
    {
        if (first == null)
            throw new ArgumentNullException(nameof(first));
        if (second == null)
            throw new ArgumentNullException(nameof(second));
        if (third == null)
            throw new ArgumentNullException(nameof(third));
        if (resultSelector == null)
            throw new ArgumentNullException(nameof(resultSelector));

        return CartesianProduct.Multiply(first, second, third, resultSelector);
    }

    /// <summary>
    /// Returns a Cartesian product of four sequences by enumerating all possible combinations of sequence elements,
    /// and applying a user-defined projection to each combination.
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
    public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TThird, TFourth, TResult>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third,
        IEnumerable<TFourth> fourth,
        Func<TFirst, TSecond, TThird, TFourth, TResult> resultSelector)
    {
        if (first == null)
            throw new ArgumentNullException(nameof(first));
        if (second == null)
            throw new ArgumentNullException(nameof(second));
        if (third == null)
            throw new ArgumentNullException(nameof(third));
        if (fourth == null)
            throw new ArgumentNullException(nameof(fourth));
        if (resultSelector == null)
            throw new ArgumentNullException(nameof(resultSelector));

        return CartesianProduct.Multiply(first, second, third, fourth, resultSelector);
    }
}
