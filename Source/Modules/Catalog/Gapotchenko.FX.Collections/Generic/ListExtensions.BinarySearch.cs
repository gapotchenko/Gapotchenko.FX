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
    /// Searches a sorted <see cref="IReadOnlyList{T}"/> for a specific element using the specified value selector and comparer.
    /// </summary>
    /// <returns>
    /// The index of a matching element, or a negative number equal to the bitwise complement of the insertion index if no element is found.
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

        comparer ??= Comparer<TValue>.Default;

        int lo = 0;
        int hi = list.Count - 1;

        while (lo <= hi)
        {
            // Avoid overflow: same technique used in BCL implementations
            int mid = lo + ((hi - lo) >> 1);

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

    /// <summary>
    /// Searches a sorted <see cref="IList{T}"/> for a specific element using the specified value selector and comparer.
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

        comparer ??= Comparer<TValue>.Default;

        int lo = 0;
        int hi = list.Count - 1;

        while (lo <= hi)
        {
            // Avoid overflow: same technique used in BCL implementations
            int mid = lo + ((hi - lo) >> 1);

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
}
