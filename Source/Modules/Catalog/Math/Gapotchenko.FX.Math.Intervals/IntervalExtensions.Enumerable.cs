// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals;

partial class IntervalExtensions
{
    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within a specified interval.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        Intersect<TSource, IInterval<TSource>>(source, interval);

    /// <inheritdoc cref="Intersect{TSource}(IEnumerable{TSource}, IInterval{TSource})"/>
    /// <typeparam name="TSource"><inheritdoc/></typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static IEnumerable<TSource> Intersect<TSource, TInterval>(this IEnumerable<TSource> source, TInterval interval)
        where TInterval : IInterval<TSource> =>
        IntersectBy(source, interval, Fn.Identity);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within a specified interval.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> IntersectBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        IInterval<TKey> interval,
        Func<TSource, TKey> keySelector) =>
        IntersectBy<TSource, IInterval<TKey>, TKey>(source, interval, keySelector);

    /// <inheritdoc cref="IntersectBy{TSource, TKey}(IEnumerable{TSource}, IInterval{TKey}, Func{TSource, TKey})"/>
    /// <typeparam name="TSource"><inheritdoc/></typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <typeparam name="TKey"><inheritdoc/></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static IEnumerable<TSource> IntersectBy<TSource, TInterval, TKey>(
        this IEnumerable<TSource> source,
        TInterval interval,
        Func<TSource, TKey> keySelector)
        where TInterval : IInterval<TKey>
    {
        ArgumentNullException.ThrowIfNull(source);
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));
        ArgumentNullException.ThrowIfNull(keySelector);

        if (interval.IsEmpty)
            return [];
        else if (interval.IsInfinite)
            return source;
        else
            return source.Where(x => interval.Contains(keySelector(x)));
    }

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within a specified interval.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        Except<TSource, IInterval<TSource>>(source, interval);

    /// <inheritdoc cref="Except{TSource}(IEnumerable{TSource}, IInterval{TSource})"/>
    /// <typeparam name="TSource"><inheritdoc/></typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static IEnumerable<TSource> Except<TSource, TInterval>(this IEnumerable<TSource> source, TInterval interval)
        where TInterval : IInterval<TSource> =>
        ExceptBy(source, interval, Fn.Identity);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within a specified interval.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        IInterval<TKey> interval,
        Func<TSource, TKey> keySelector) =>
        ExceptBy<TSource, IInterval<TKey>, TKey>(source, interval, keySelector);

    /// <inheritdoc cref="ExceptBy{TSource, TKey}(IEnumerable{TSource}, IInterval{TKey}, Func{TSource, TKey})"/>
    /// <typeparam name="TSource"><inheritdoc/></typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <typeparam name="TKey"><inheritdoc/></typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static IEnumerable<TSource> ExceptBy<TSource, TInterval, TKey>(
        this IEnumerable<TSource> source,
        TInterval interval,
        Func<TSource, TKey> keySelector)
        where TInterval : IInterval<TKey>
    {
        ArgumentNullException.ThrowIfNull(source);
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));
        ArgumentNullException.ThrowIfNull(keySelector);

        if (interval.IsInfinite)
            return [];
        else if (interval.IsEmpty)
            return source;
        else
            return source.Where(x => !interval.Contains(keySelector(x)));
    }
}
