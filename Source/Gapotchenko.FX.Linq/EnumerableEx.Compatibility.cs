// Binary API compatibility layer.

namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
    /// <summary>
    /// <para>
    /// Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
    /// <returns>A <see cref="HashSet{T}"/> that contains values of type <typeparamref name="TSource"/> retrieved from the <paramref name="source"/>.</returns>
    [Obsolete("Use IEnumerable<T>.ToHashSet() extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashSet<TSource> ToHashSet<TSource>(IEnumerable<TSource> source) =>
        EnumerableExtensions.ToHashSet(source);

    /// <summary>
    /// <para>
    /// Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/> using the <paramref name="comparer"/> to compare keys.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>A <see cref="HashSet{T}"/> that contains values of type <typeparamref name="TSource"/> retrieved from the <paramref name="source"/>.</returns>
    [Obsolete("Use IEnumerable<T>.ToHashSet(comparer) extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashSet<TSource> ToHashSet<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer) =>
        EnumerableExtensions.ToHashSet(source, comparer);

    /// <summary>
    /// Returns distinct elements from a sequence by using the default equality comparer on the keys extracted by a specified selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence to remove duplicate elements from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
    [Obsolete("Use IEnumerable<T>.DistinctBy(keySelector) extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
        EnumerableExtensions.DistinctBy(source, keySelector);

    /// <summary>
    /// Returns distinct elements from a sequence by using a specified <see cref="IEqualityComparer{T}"/> on the keys extracted by a specified selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence to remove duplicate elements from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
    [Obsolete("Use IEnumerable<T>.DistinctBy(keySelector, comparer) extension method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer) =>
        EnumerableExtensions.DistinctBy(source, keySelector, comparer);
}
