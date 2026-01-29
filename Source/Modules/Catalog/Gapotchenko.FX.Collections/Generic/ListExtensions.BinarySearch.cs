// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections.Generic;

partial class ListExtensions
{
    /// <summary>
    /// Searches the entire sorted <see cref="IReadOnlyList{T}"/> for an element using the specified value selector and comparer.
    /// </summary>
    /// <returns>
    /// The zero-based index of a matching element, or a negative number equal to the bitwise complement of the insertion index if no element is found.
    /// </returns>
    /// <param name="list">The list of elements.</param>
    /// <param name="value">The element value to find.</param>
    /// <param name="valueSelector">The value selector which selects a value from a list element.</param>
    /// <param name="comparer">The comparer, or <see langword="null"/> to use the default <typeparamref name="TValue"/> comparer.</param>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="valueSelector"/> is <see langword="null"/>.</exception>
    /// <typeparam name="TElement">The type of list elements.</typeparam>
    /// <typeparam name="TValue">The type of a value to find.</typeparam>
    public static int BinarySearchBy<TElement, TValue>(
        this IReadOnlyList<TElement> list,
        TValue value,
        Func<TElement, TValue> valueSelector,
        IComparer<TValue>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(valueSelector);

        return BinarySearchByCore(list, 0, list.Count, value, valueSelector, comparer);
    }

    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element using the specified value selector and comparer.
    /// </summary>
    /// <inheritdoc cref="BinarySearchBy{TElement, TValue}(IReadOnlyList{TElement}, TValue, Func{TElement, TValue}, IComparer{TValue}?)"/>
    [OverloadResolutionPriority(-1)]
    public static int BinarySearchBy<TElement, TValue>(
        this IList<TElement> list,
        TValue value,
        Func<TElement, TValue> valueSelector,
        IComparer<TValue>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(valueSelector);

        return BinarySearchByCore(list, 0, list.Count, value, valueSelector, comparer);
    }

    /// <summary>
    /// Searches a range of elements in the sorted <see cref="IReadOnlyList{T}"/> for an element using the specified value selector and comparer.
    /// </summary>
    /// <inheritdoc cref="BinarySearchByCore{TElement, TValue}(IReadOnlyList{TElement}, int, int, TValue, Func{TElement, TValue}, IComparer{TValue}?)"/>
    /// <param name="list"><inheritdoc/></param>
    /// <param name="range">The range to search.</param>
    /// <param name="value"><inheritdoc/>The range to search</param>
    /// <param name="valueSelector"><inheritdoc/></param>
    /// <param name="comparer"><inheritdoc/></param>
    public static int BinarySearchBy<TElement, TValue>(
        this IReadOnlyList<TElement> list,
        Range range,
        TValue value,
        Func<TElement, TValue> valueSelector,
        IComparer<TValue>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(valueSelector);

        var (offset, count) = range.GetOffsetAndLength(list.Count);
        return BinarySearchByCore(list, offset, offset + count, value, valueSelector, comparer);
    }

    /// <summary>
    /// Searches a range of elements in the sorted <see cref="IList{T}"/> for an element using the specified value selector and comparer.
    /// </summary>
    /// <inheritdoc cref="BinarySearchBy{TElement, TValue}(IReadOnlyList{TElement}, Range, TValue, Func{TElement, TValue}, IComparer{TValue}?)"/>
    [OverloadResolutionPriority(-1)]
    public static int BinarySearchBy<TElement, TValue>(
        this IList<TElement> list,
        Range range,
        TValue value,
        Func<TElement, TValue> valueSelector,
        IComparer<TValue>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(valueSelector);

        var (offset, count) = range.GetOffsetAndLength(list.Count);
        return BinarySearchByCore(list, offset, offset + count, value, valueSelector, comparer);
    }

    // ------------------------------------------------------------------------

    static int BinarySearchByCore<TElement, TValue>(
        IReadOnlyList<TElement> list,
        int start, // the inclusive start index
        int end, // the exclusive end index
        TValue value,
        Func<TElement, TValue> valueSelector,
        IComparer<TValue>? comparer)
    {
        comparer ??= Comparer<TValue>.Default;

        int lo = start;
        int hi = end - 1;

        while (lo <= hi)
        {
            int mid = GetMid(lo, hi);

            var midValue = valueSelector(list[mid]);

            int cmp = comparer.Compare(midValue, value);
            if (cmp == 0)
                return mid;
            else if (cmp < 0)
                lo = mid + 1;
            else
                hi = mid - 1;
        }

        // Not found; lo is the insertion index
        return ~lo;
    }

    static int BinarySearchByCore<TElement, TValue>(
        IList<TElement> list,
        int start, // the inclusive start index
        int end, // the exclusive end index
        TValue value,
        Func<TElement, TValue> valueSelector,
        IComparer<TValue>? comparer)
    {
        comparer ??= Comparer<TValue>.Default;

        int lo = start;
        int hi = end - 1;

        while (lo <= hi)
        {
            int mid = GetMid(lo, hi);

            var midValue = valueSelector(list[mid]);

            int cmp = comparer.Compare(midValue, value);
            if (cmp == 0)
                return mid;
            else if (cmp < 0)
                lo = mid + 1;
            else
                hi = mid - 1;
        }

        // Not found; lo is the insertion index
        return ~lo;
    }

    static int GetMid(int lo, int hi) =>
        // Avoid overflow: same technique used in BCL implementations
        lo + ((hi - lo) >> 1);
}
