using System.Collections;
using System.Collections.ObjectModel;

namespace Gapotchenko.FX.Linq;

/// <summary>
/// Provides extensions methods for <see cref="IEnumerable{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class EnumerableExtensions
{
    /// <summary>
    /// Returns a <see cref="IReadOnlyCollection{T}"/> view of the source sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/> view of the source sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IReadOnlyCollection<TSource> ReifyCollection<TSource>(this IEnumerable<TSource> source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (source is IReadOnlyCollection<TSource> readOnlyCollection)
            return readOnlyCollection;
        if (source is ICollection<TSource> genericCollection)
            return new ReifiedCollection<TSource>(genericCollection, genericCollection.Count);
        if (source is string s)
            return (IReadOnlyCollection<TSource>)(object)new ReifiedCharList(s);
        if (source is ICollection collection)
            return new ReifiedCollection<TSource>(source, collection.Count);

        if (source.TryGetNonEnumeratedCount(out var count))
            return new ReifiedCollection<TSource>(source, count);

        return EnumerableEx.AsList(source).AsReadOnly();
    }

    /// <summary>
    /// Returns a <see cref="IReadOnlyList{T}"/> view of the source sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A <see cref="IReadOnlyList{T}"/> view of the source sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<TSource> ReifyList<TSource>(this IEnumerable<TSource> source) =>
        source switch
        {
            null => throw new ArgumentNullException(nameof(source)),
            IReadOnlyList<TSource> readOnlyList => readOnlyList,
            IList<TSource> list => new ReadOnlyCollection<TSource>(list),
            string s => (IReadOnlyList<TSource>)(object)new ReifiedCharList(s),
            _ => EnumerableEx.AsList(source).AsReadOnly()
        };
}
