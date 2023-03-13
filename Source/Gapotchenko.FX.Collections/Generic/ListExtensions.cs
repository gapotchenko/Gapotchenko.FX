namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// <see cref="List{T}"/> extensions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ListExtensions
{
    /// <summary>
    /// Performs in-place stable sort of the elements in entire <see cref="List{T}"/> using the specified comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="comparer">
    /// The <see cref="IComparer{T}"/> implementation to use when comparing elements,
    /// or <see langword="null"/> to use the default comparer.
    /// </param>
    public static void StableSort<T>(this List<T> list, IComparer<T>? comparer)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        if (list.Count < 2)
            return;

        var sortedList = list.OrderBy(Fn.Identity, comparer).ToList();

        list.Clear();
        list.AddRange(sortedList);
    }

    /// <summary>
    /// Performs in-place stable sort of the elements in entire <see cref="List{T}"/> using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list.</param>
    public static void StableSort<T>(this List<T> list) => StableSort(list, null);

    /// <summary>
    /// Clones the list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list.</param>
    /// <returns>A cloned list.</returns>
    public static List<T> Clone<T>(this List<T> list) => new(list ?? throw new ArgumentNullException(nameof(list)));
}
