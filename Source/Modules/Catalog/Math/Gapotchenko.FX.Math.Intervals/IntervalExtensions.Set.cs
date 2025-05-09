// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals;

partial class IntervalExtensions
{
    /// <summary>
    /// Modifies the current set so that it contains only elements that are also in a specified interval.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    /// <param name="set">The set.</param>
    /// <param name="interval">The interval to compare to the current set.</param>
    /// <exception cref="ArgumentNullException"><paramref name="set"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is <see langword="null"/>.</exception>
    public static void IntersectWith<T>(this ISet<T> set, IInterval<T> interval) =>
        IntersectWith<T, IInterval<T>>(set, interval);

    /// <inheritdoc cref="IntersectWith{T}(ISet{T}, IInterval{T})"/>
    /// <typeparam name="TElement">The type of elements in the set.</typeparam>
    /// <typeparam name="TInterval">The type of interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static void IntersectWith<TElement, TInterval>(this ISet<TElement> set, TInterval interval)
        where TInterval : IInterval<TElement>
    {
        ArgumentNullException.ThrowIfNull(set);
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
    /// <exception cref="ArgumentNullException"><paramref name="set"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is <see langword="null"/>.</exception>
    public static void ExceptWith<T>(this ISet<T> set, IInterval<T> interval) =>
        ExceptWith<T, IInterval<T>>(set, interval);

    /// <inheritdoc cref="ExceptWith{T}(ISet{T}, IInterval{T})"/>
    /// <typeparam name="TElement">The type of elements in the set.</typeparam>
    /// <typeparam name="TInterval">The type of interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static void ExceptWith<TElement, TInterval>(this ISet<TElement> set, TInterval interval)
        where TInterval : IInterval<TElement>
    {
        ArgumentNullException.ThrowIfNull(set);
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
