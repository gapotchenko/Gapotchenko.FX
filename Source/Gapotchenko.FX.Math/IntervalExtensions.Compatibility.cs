namespace Gapotchenko.FX.Math;

partial class IntervalExtensions
{
    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.</returns>
    [Obsolete("Use Intersect method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithinInterval<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        Intersect(source, interval);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.</returns>
    [Obsolete("Use Intersect method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithinInterval<TSource, TInterval>(this IEnumerable<TSource> source, TInterval interval)
        where TInterval : IInterval<TSource> =>
        Intersect(source, interval);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.</returns>
    [Obsolete("Use Except method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithoutInterval<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        Except(source, interval);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.</returns>
    [Obsolete("Use Except method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithoutInterval<TSource, TInterval>(this IEnumerable<TSource> source, TInterval interval)
        where TInterval : IInterval<TSource> =>
        Except(source, interval);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within the interval.</returns>
    [Obsolete("Use IntersectBy method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithinIntervalBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        IInterval<TKey> interval,
        Func<TSource, TKey> keySelector) =>
        IntersectBy(source, interval, keySelector);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within the interval.</returns>
    [Obsolete("Use IntersectBy method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithinIntervalBy<TSource, TInterval, TKey>(
        this IEnumerable<TSource> source,
        TInterval interval,
        Func<TSource, TKey> keySelector)
        where TInterval : IInterval<TKey> =>
        IntersectBy(source, interval, keySelector);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.</returns>
    [Obsolete("Use ExceptBy method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithoutIntervalBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        IInterval<TKey> interval,
        Func<TSource, TKey> keySelector) =>
        ExceptBy(source, interval, keySelector);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are not contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are not contained within the interval.</returns>
    [Obsolete("Use ExceptBy method instead.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithoutIntervalBy<TSource, TInterval, TKey>(
        this IEnumerable<TSource> source,
        TInterval interval,
        Func<TSource, TKey> keySelector)
        where TInterval : IInterval<TKey> =>
        ExceptBy(source, interval, keySelector);
}
