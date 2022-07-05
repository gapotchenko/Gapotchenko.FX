using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gapotchenko.FX.Math;

/// <summary>
/// Interval extensions for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class IntervalEnumerableExtensions
{
    /// <summary>
    /// Filters a sequence of values based on an interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.</returns>
    public static IEnumerable<TSource> WithinInterval<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        WithinInterval<TSource, IInterval<TSource>>(source, interval);

    /// <summary>
    /// Filters a sequence of values based on an interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within the interval.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithinInterval<TSource, TInterval>(this IEnumerable<TSource> source, TInterval interval)
        where TInterval : IInterval<TSource>
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));

        if (interval.IsEmpty)
            return Enumerable.Empty<TSource>();
        else if (interval.IsInfinite)
            return source;
        else
            return source.Where(x => interval.Contains(x));
    }

    /// <summary>
    /// Filters a sequence of values based on an interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.</returns>
    public static IEnumerable<TSource> WithoutInterval<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        WithoutInterval<TSource, IInterval<TSource>>(source, interval);

    /// <summary>
    /// Filters a sequence of values based on an interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within the interval.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> WithoutInterval<TSource, TInterval>(this IEnumerable<TSource> source, TInterval interval)
        where TInterval : IInterval<TSource>
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));

        if (interval.IsInfinite)
            return Enumerable.Empty<TSource>();
        else if (interval.IsEmpty)
            return source;
        else
            return source.Where(x => !interval.Contains(x));
    }
}
