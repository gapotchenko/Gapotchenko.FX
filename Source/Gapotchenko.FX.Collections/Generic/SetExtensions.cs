using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// Generic set extensions.
/// </summary>
public static class SetExtensions
{
    /// <summary>
    /// Indicates whether the specified set is null or empty.
    /// </summary>
    /// <param name="value">The set to test.</param>
    /// <returns><c>true</c> if the <paramref name="value"/> parameter is null or an empty set; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty<T>(
        [NotNullWhen(false)]
#if !TFF_HASHSET_IREADONLYCOLLECTION
        this
#endif
        ISet<T>? value) =>
        value is null || value.Count == 0;

    /// <summary>
    /// Adds the elements of the specified collection to the target set.
    /// </summary>
    /// <typeparam name="T">The type of the elements of collection.</typeparam>
    /// <param name="target">The set where the elements should be added to.</param>
    /// <param name="collection">The collection whose elements should be added.</param>
    /// <returns>
    /// <c>true</c> if at least one element is added to the <paramref name="target">target set</paramref>;
    /// <c>false</c> if all the elements are already present.
    /// </returns>
    public static bool AddRange<T>(this ISet<T> target, IEnumerable<T> collection)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        bool result = false;
        foreach (var i in collection)
            result |= target.Add(i);
        return result;
    }

    /// <summary>
    /// Adds the elements of the specified collection to the target set.
    /// </summary>
    /// <typeparam name="T">The type of the elements of collection.</typeparam>
    /// <param name="target">The set where the elements should be added to.</param>
    /// <param name="collection">The collection whose elements should be added.</param>
    /// <returns>
    /// <c>true</c> if at least one element is added to the <paramref name="target">target set</paramref>;
    /// <c>false</c> if all the elements are already present.
    /// </returns>
    public static bool AddRange<T>(this ISet<T> target, params T[] collection) => AddRange(target, (IEnumerable<T>)collection);
}
