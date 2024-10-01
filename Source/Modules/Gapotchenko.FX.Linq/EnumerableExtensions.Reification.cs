using System.Collections;
using System.Collections.ObjectModel;

namespace Gapotchenko.FX.Linq;

partial class EnumerableExtensions
{
    /// <summary>
    /// Attempts to get a <see cref="IReadOnlyCollection{T}"/> view of the source sequence
    /// without forcing an enumeration.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="collection">
    /// A <see cref="IReadOnlyCollection{T}"/> view of the source sequence,
    /// or <see langword="null"/> if the view cannot be reified without enumeration.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IReadOnlyCollection{T}"/> view of the source sequence can be reified without enumeration;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static bool TryReifyNonEnumeratedCollection<TSource>(
        this IEnumerable<TSource> source,
        [NotNullWhen(true)] out IReadOnlyCollection<TSource>? collection) =>
        (collection = TryReifyNonEnumeratedCollection(source)) is not null;

    /// <summary>
    /// Attempts to get a <see cref="IReadOnlyCollection{T}"/> view of the source sequence
    /// without forcing an enumeration.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>
    /// A <see cref="IReadOnlyCollection{T}"/> view of the source sequence,
    /// or <see langword="null"/> if the <see cref="IReadOnlyCollection{T}"/> cannot be reified without enumeration.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IReadOnlyCollection<TSource>? TryReifyNonEnumeratedCollection<TSource>(this IEnumerable<TSource> source) =>
        source switch
        {
            null => throw new ArgumentNullException(nameof(source)),
            IReadOnlyCollection<TSource> readOnlyCollection => readOnlyCollection,
            ICollection<TSource> genericCollection => new ReifiedCollection<TSource>(genericCollection, genericCollection.Count),
            string s => (IReadOnlyCollection<TSource>)(object)new ReifiedCharList(s),
            ICollection collection => new ReifiedCollection<TSource>(source, collection.Count),
            _ when source.TryGetNonEnumeratedCount(out var count) => new ReifiedCollection<TSource>(source, count),
            _ => null
        };

    /// <summary>
    /// Gets a <see cref="IReadOnlyCollection{T}"/> view of the sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A <see cref="IReadOnlyCollection{T}"/> view of the source sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IReadOnlyCollection<TSource> ReifyCollection<TSource>(this IEnumerable<TSource> source) =>
        TryReifyNonEnumeratedCollection(source) ??
        EnumerableEx.AsList(source).AsReadOnly();

    /// <summary>
    /// Attempts to get a <see cref="IReadOnlyList{T}"/> view of the source sequence
    /// without forcing an enumeration.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="list">
    /// A <see cref="IReadOnlyList{T}"/> view of the source sequence,
    /// or <see langword="null"/> if the view cannot be reified without enumeration.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IReadOnlyList{T}"/> view of the source sequence can be reified without enumeration;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static bool TryReifyNonEnumeratedList<TSource>(
        this IEnumerable<TSource> source,
        [NotNullWhen(true)] out IReadOnlyCollection<TSource>? list) =>
        (list = TryReifyNonEnumeratedList(source)) is not null;

    /// <summary>
    /// Attempts to get a <see cref="IReadOnlyList{T}"/> view of the source sequence
    /// without forcing an enumeration.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>
    /// A <see cref="IReadOnlyList{T}"/> view of the source sequence,
    /// or <see langword="null"/> if the <see cref="IReadOnlyList{T}"/> cannot be reified without enumeration.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<TSource>? TryReifyNonEnumeratedList<TSource>(this IEnumerable<TSource> source) =>
        source switch
        {
            null => throw new ArgumentNullException(nameof(source)),
            IReadOnlyList<TSource> readOnlyList => readOnlyList,
            IList<TSource> list => new ReadOnlyCollection<TSource>(list),
            string s => (IReadOnlyList<TSource>)(object)new ReifiedCharList(s),
            _ => null
        };

    /// <summary>
    /// Gets a <see cref="IReadOnlyList{T}"/> view of the source sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A <see cref="IReadOnlyList{T}"/> view of the source sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<TSource> ReifyList<TSource>(this IEnumerable<TSource> source) =>
        TryReifyNonEnumeratedList(source) ??
        EnumerableEx.AsList(source).AsReadOnly();
}
