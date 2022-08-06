namespace Gapotchenko.FX.Math;

/// <summary>
/// Extension methods for <see cref="IInterval{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class IntervalExtensions
{
    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within a specified interval.</returns>
    public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        Intersect<TSource, IInterval<TSource>>(source, interval);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are contained within a specified interval.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
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
    public static IEnumerable<TSource> IntersectBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        IInterval<TKey> interval,
        Func<TSource, TKey> keySelector) =>
        IntersectBy<TSource, IInterval<TKey>, TKey>(source, interval, keySelector);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are contained within a specified interval.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> IntersectBy<TSource, TInterval, TKey>(
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
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within a specified interval.</returns>
    public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> source, IInterval<TSource> interval) =>
        Except<TSource, IInterval<TSource>>(source, interval);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that are not contained within a specified interval.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
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
    public static IEnumerable<TSource> ExceptBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        IInterval<TKey> interval,
        Func<TSource, TKey> keySelector) =>
        ExceptBy<TSource, IInterval<TKey>, TKey>(source, interval, keySelector);

    /// <summary>
    /// Produces an <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are not contained within a specified interval.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="interval">The interval.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence the keys of which are not contained within a specified interval.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> ExceptBy<TSource, TInterval, TKey>(
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

    /// <summary>
    /// Modifies the current set so that it contains only elements that are also in a specified interval.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    /// <param name="set">The set.</param>
    /// <param name="interval">The interval to compare to the current set.</param>
    public static void IntersectWith<T>(this ISet<T> set, IInterval<T> interval) =>
        IntersectWith<T, IInterval<T>>(set, interval);

    /// <summary>
    /// Modifies the current set so that it contains only elements that are also in a specified interval.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in the set.</typeparam>
    /// <typeparam name="TInterval">The type of interval.</typeparam>
    /// <param name="set">The set.</param>
    /// <param name="interval">The interval to compare to the current set.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void IntersectWith<TElement, TInterval>(this ISet<TElement> set, TInterval interval)
        where TInterval : IInterval<TElement>
    {
        if (set == null)
            throw new ArgumentNullException(nameof(set));
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));

        if (interval.IsInfinite)
            return;
        else if (interval.IsEmpty)
            set.Clear();
        else
            set.ExceptWith(set.Except(interval).ToList());
    }

    /// <summary>
    /// Removes all elements contained in the specified interval from the current set.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    /// <param name="set">The set.</param>
    /// <param name="interval">The interval of items to remove from the set.</param>
    public static void ExceptWith<T>(this ISet<T> set, IInterval<T> interval) =>
        ExceptWith<T, IInterval<T>>(set, interval);

    /// <summary>
    /// Removes all elements contained in the specified interval from the current set.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in the set.</typeparam>
    /// <typeparam name="TInterval">The type of interval.</typeparam>
    /// <param name="set">The set.</param>
    /// <param name="interval">The interval of items to remove from the set.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ExceptWith<TElement, TInterval>(this ISet<TElement> set, TInterval interval)
        where TInterval : IInterval<TElement>
    {
        if (set == null)
            throw new ArgumentNullException(nameof(set));
        if (interval == null)
            throw new ArgumentNullException(nameof(interval));

        if (interval.IsInfinite)
            set.Clear();
        else if (interval.IsEmpty)
            return;
        else
            set.ExceptWith(set.Intersect(interval).ToList());
    }
}
