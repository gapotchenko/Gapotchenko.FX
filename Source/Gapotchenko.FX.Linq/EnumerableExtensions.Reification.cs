using System.Collections;
using System.Collections.ObjectModel;

namespace Gapotchenko.FX.Linq;

partial class EnumerableExtensions
{
    /// <summary>
    /// Gets a <see cref="IReadOnlyCollection{T}"/> view of the source sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/> view of the source sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IReadOnlyCollection<TSource> ReifyCollection<TSource>(this IEnumerable<TSource> source) =>
        source switch
        {
            null => throw new ArgumentNullException(nameof(source)),
            IReadOnlyCollection<TSource> readOnlyCollection => readOnlyCollection,
            ICollection<TSource> genericCollection => new ReifiedCollection<TSource>(genericCollection, genericCollection.Count),
            string s => (IReadOnlyCollection<TSource>)(object)new ReifiedCharList(s),
            ICollection collection => new ReifiedCollection<TSource>(source, collection.Count),
            _ when source.TryGetNonEnumeratedCount(out var count) => new ReifiedCollection<TSource>(source, count),
            _ => EnumerableEx.AsList(source).AsReadOnly()
        };

    /// <summary>
    /// Gets a <see cref="IReadOnlyList{T}"/> view of the source sequence.
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
