using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gapotchenko.FX.Math;

/// <summary>
/// Extension methods for <see cref="IInterval{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IntervalExtensions
{
    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.</returns>
    public static IEnumerable<TSource> WithinInterval<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        WithinInterval<TSource, IInterval<TSource>>(source, interval);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithinInterval<TSource, TInterval>(this IEnumerable<TSource> source, TInterval interval)
        where TInterval : IInterval<TSource> =>
        WithinIntervalBy(source, interval, Fn.Identity);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.</returns>
    public static IEnumerable<TSource> WithoutInterval<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        WithoutInterval<TSource, IInterval<TSource>>(source, interval);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithoutInterval<TSource, TInterval>(this IEnumerable<TSource> source, TInterval interval)
        where TInterval : IInterval<TSource> =>
        WithoutIntervalBy(source, interval, Fn.Identity);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within the interval.</returns>
    public static IEnumerable<TSource> WithinIntervalBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        IInterval<TKey> interval,
        Func<TSource, TKey> keySelector) =>
        WithinIntervalBy<TSource, IInterval<TKey>, TKey>(source, interval, keySelector);

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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithinIntervalBy<TSource, TInterval, TKey>(
        this IEnumerable<TSource> source,
        TInterval interval,
        Func<TSource, TKey> keySelector)
        where TInterval : IInterval<TKey>
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        if (interval.IsEmpty)
            return Enumerable.Empty<TSource>();
        else if (interval.IsInfinite)
            return source;
        else
            return source.Where(x => interval.Contains(keySelector(x)));
    }

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.</returns>
    public static IEnumerable<TSource> WithoutIntervalBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        IInterval<TKey> interval,
        Func<TSource, TKey> keySelector) =>
        WithoutIntervalBy<TSource, IInterval<TKey>, TKey>(source, interval, keySelector);

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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithoutIntervalBy<TSource, TInterval, TKey>(
        this IEnumerable<TSource> source,
        TInterval interval,
        Func<TSource, TKey> keySelector)
        where TInterval : IInterval<TKey>
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        if (interval.IsInfinite)
            return Enumerable.Empty<TSource>();
        else if (interval.IsEmpty)
            return source;
        else
            return source.Where(x => !interval.Contains(keySelector(x)));
    }
}
