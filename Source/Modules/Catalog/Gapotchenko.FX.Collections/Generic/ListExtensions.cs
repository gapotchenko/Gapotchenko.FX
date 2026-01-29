// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// Provides extension methods for <see cref="IReadOnlyList{T}"/>, <see cref="IList{T}"/>, and <see cref="List{T}"/> types.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class ListExtensions
{
    /// <summary>
    /// Sorts the elements in the entire <see cref="List{T}"/>
    /// using the default comparer
    /// and a stable sorting algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list.</param>
    public static void StableSort<T>(this List<T> list) => StableSort(list, null);

    /// <summary>
    /// Sorts the elements in the entire <see cref="List{T}"/>
    /// using the specified comparer
    /// and a stable sorting algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The <see cref="List{T}"/>.</param>
    /// <param name="comparer">
    /// The <see cref="IComparer{T}"/> implementation to use when comparing elements,
    /// or <see langword="null"/> to use the default comparer.
    /// </param>
    public static void StableSort<T>(this List<T> list, IComparer<T>? comparer)
    {
        ArgumentNullException.ThrowIfNull(list);

        if (list.Count < 2)
            return;

        var sortedList = list.OrderBy(Fn.Identity, comparer).ToList();

        list.Clear();
        list.AddRange(sortedList);
    }

    /// <summary>
    /// Clones the <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <see cref="List{T}"/>.</typeparam>
    /// <param name="list">The <see cref="List{T}"/>.</param>
    /// <returns>A cloned <see cref="List{T}"/>.</returns>
    public static List<T> Clone<T>(this List<T> list) => [.. list ?? throw new ArgumentNullException(nameof(list))];
}
