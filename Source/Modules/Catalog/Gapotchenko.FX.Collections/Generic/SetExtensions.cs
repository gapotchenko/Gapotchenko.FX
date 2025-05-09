namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// Generic set extensions.
/// </summary>
public static class SetExtensions
{
#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY
    /// <summary>
    /// Indicates whether the specified set is null or empty.
    /// </summary>
    /// <param name="value">The set to test.</param>
    /// <returns><see langword="true"/> if the <paramref name="value"/> parameter is null or an empty set; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsNullOrEmpty<T>(
        [NotNullWhen(false)]
#if SOURCE_COMPATIBILITY && !TFF_HASHSET_IREADONLYCOLLECTION
        this
#endif
        ISet<T>? value) =>
        value is null || value.Count == 0;
#endif

    /// <summary>
    /// Adds the elements of the specified collection to the target set.
    /// </summary>
    /// <typeparam name="T">The type of the elements of collection.</typeparam>
    /// <param name="target">The set where the elements should be added to.</param>
    /// <param name="collection">The collection whose elements should be added.</param>
    /// <returns>
    /// <see langword="true"/> if at least one element is added to the <paramref name="target">target set</paramref>;
    /// <see langword="false"/> if all the elements are already present.
    /// </returns>
    public static bool AddRange<T>(this ISet<T> target, IEnumerable<T> collection)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(collection);

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
    /// <see langword="true"/> if at least one element is added to the <paramref name="target">target set</paramref>;
    /// <see langword="false"/> if all the elements are already present.
    /// </returns>
    public static bool AddRange<T>(this ISet<T> target, params T[] collection) => AddRange(target, (IEnumerable<T>)collection);
}
