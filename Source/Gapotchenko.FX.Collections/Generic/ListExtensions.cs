using Gapotchenko.FX.Collections.Utils;

namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// <see cref="List{T}"/> extensions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ListExtensions
{
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
        ExceptionHelpers.ThrowIfArgumentIsNull(list);

        if (list.Count < 2)
            return;

        var sortedList = list.OrderBy(Fn.Identity, comparer).ToList();

        list.Clear();
        list.AddRange(sortedList);
    }

    /// <summary>
    /// Sorts the elements in the entire <see cref="List{T}"/>
    /// using the default comparer
    /// and a stable sorting algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list.</param>
    public static void StableSort<T>(this List<T> list) => StableSort(list, null);

    /// <summary>
    /// Clones the <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <see cref="List{T}"/>.</typeparam>
    /// <param name="list">The <see cref="List{T}"/>.</param>
    /// <returns>A cloned <see cref="List{T}"/>.</returns>
    public static List<T> Clone<T>(this List<T> list) => new(list ?? throw new ArgumentNullException(nameof(list)));
}
