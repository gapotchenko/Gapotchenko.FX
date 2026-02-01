// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections.Generic;

partial class ListExtensions
{
    /// <summary>
    /// Searches the entire sorted <see cref="IReadOnlyList{T}"/> for an element using the specified key selector and comparer.
    /// </summary>
    /// <returns>
    /// <para>
    /// The index of a matching element in the specified <paramref name="list"/>, if element is found; otherwise, a negative number.
    /// </para>
    /// <para>
    /// If element is not found and <paramref name="value"/> is less than at least one element key in <paramref name="list"/>,
    /// the negative number returned is the bitwise complement of the index of the first element whose key is larger than <paramref name="value"/>.
    /// </para>
    /// <para>
    /// If element is not found and <paramref name="value"/> is greater than all element keys in <paramref name="list"/>,
    /// the negative number returned is the bitwise complement of (the index of the last element plus 1).
    /// </para>
    /// <para>
    /// If this method is called with a non-sorted <paramref name="list"/>,
    /// the return value can be incorrect and a negative number could be returned,
    /// even if matching element is present in <paramref name="list"/>.
    /// </para>
    /// </returns>
    /// <param name="list">The list of elements.</param>
    /// <param name="value">The element value to find.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="comparer">The comparer, or <see langword="null"/> to use the default <typeparamref name="TValue"/> comparer.</param>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    /// <typeparam name="TElement">The type of list elements.</typeparam>
    /// <typeparam name="TValue">The type of a value to find.</typeparam>
    public static int BinarySearchBy<TElement, TValue>(
        this IReadOnlyList<TElement> list,
        TValue value,
        Func<TElement, TValue> keySelector,
        IComparer<TValue>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(keySelector);

        return BinarySearchByCore(list, 0, list.Count, value, keySelector, comparer);
    }

    /// <summary>
    /// Searches the entire sorted <see cref="IList{T}"/> for an element using the specified key selector and comparer.
    /// </summary>
    /// <inheritdoc cref="BinarySearchBy{TElement, TValue}(IReadOnlyList{TElement}, TValue, Func{TElement, TValue}, IComparer{TValue}?)"/>
    [OverloadResolutionPriority(-1)]
    public static int BinarySearchBy<TElement, TValue>(
        this IList<TElement> list,
        TValue value,
        Func<TElement, TValue> keySelector,
        IComparer<TValue>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(keySelector);

        return BinarySearchByCore(list, 0, list.Count, value, keySelector, comparer);
    }

    /// <summary>
    /// Searches a range of elements in the sorted <see cref="IReadOnlyList{T}"/> for an element using the specified key selector and comparer.
    /// </summary>
    /// <inheritdoc cref="BinarySearchByCore{TElement, TValue}(IReadOnlyList{TElement}, int, int, TValue, Func{TElement, TValue}, IComparer{TValue}?)"/>
    /// <param name="list"><inheritdoc/></param>
    /// <param name="range">The range to search.</param>
    /// <param name="value"><inheritdoc/>The range to search</param>
    /// <param name="keySelector"><inheritdoc/></param>
    /// <param name="comparer"><inheritdoc/></param>
    public static int BinarySearchBy<TElement, TValue>(
        this IReadOnlyList<TElement> list,
        Range range,
        TValue value,
        Func<TElement, TValue> keySelector,
        IComparer<TValue>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(keySelector);

        var (offset, count) = range.GetOffsetAndLength(list.Count);
        return BinarySearchByCore(list, offset, offset + count, value, keySelector, comparer);
    }

    /// <summary>
    /// Searches a range of elements in the sorted <see cref="IList{T}"/> for an element using the specified key selector and comparer.
    /// </summary>
    /// <inheritdoc cref="BinarySearchBy{TElement, TValue}(IReadOnlyList{TElement}, Range, TValue, Func{TElement, TValue}, IComparer{TValue}?)"/>
    [OverloadResolutionPriority(-1)]
    public static int BinarySearchBy<TElement, TValue>(
        this IList<TElement> list,
        Range range,
        TValue value,
        Func<TElement, TValue> keySelector,
        IComparer<TValue>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(keySelector);

        var (offset, count) = range.GetOffsetAndLength(list.Count);
        return BinarySearchByCore(list, offset, offset + count, value, keySelector, comparer);
    }

    // ------------------------------------------------------------------------

    static int BinarySearchByCore<TElement, TValue>(
        IReadOnlyList<TElement> list,
        int start, // the inclusive start index
        int end, // the exclusive end index
        TValue value,
        Func<TElement, TValue> keySelector,
        IComparer<TValue>? comparer)
    {
        comparer ??= Comparer<TValue>.Default;

        int lo = start;
        int hi = end - 1;

        while (lo <= hi)
        {
            int i = GetMedian(lo, hi);

            var key = keySelector(list[i]);
            int c = comparer.Compare(key, value);

            if (c == 0)
                return i;
            else if (c < 0)
                lo = i + 1;
            else
                hi = i - 1;
        }

        // Not found; lo is the insertion index
        return ~lo;
    }

    static int BinarySearchByCore<TElement, TValue>(
        IList<TElement> list,
        int start, // the inclusive start index
        int end, // the exclusive end index
        TValue value,
        Func<TElement, TValue> keySelector,
        IComparer<TValue>? comparer)
    {
        comparer ??= Comparer<TValue>.Default;

        int lo = start;
        int hi = end - 1;

        while (lo <= hi)
        {
            int i = GetMedian(lo, hi);

            var key = keySelector(list[i]);
            int c = comparer.Compare(key, value);

            if (c == 0)
                return i;
            else if (c < 0)
                lo = i + 1;
            else
                hi = i - 1;
        }

        // Not found; lo is the insertion index
        return ~lo;
    }

    static int GetMedian(int lo, int hi) =>
        // Avoid overflow: same technique used in BCL implementations
        lo + ((hi - lo) >> 1);
}
