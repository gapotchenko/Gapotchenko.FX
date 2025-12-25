// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Linq;

partial class EnumerableExtensions
{
    /// <summary>
    /// Returns distinct adjacent elements from a sequence by removing consecutive duplicate elements.
    /// Values are compared by using the default equality comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to remove consecutive duplicate elements from.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains distinct adjacent elements from the source sequence,
    /// with only the first occurrence of each consecutive group of equal elements preserved.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> DistinctAdjacent<TSource>(this IEnumerable<TSource> source) =>
        DistinctAdjacent(source, null);

    /// <summary>
    /// Returns distinct adjacent elements from a sequence by removing consecutive duplicate elements.
    /// Values are compared by using a specified <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to remove consecutive duplicate elements from.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains distinct adjacent elements from the source sequence,
    /// with only the first occurrence of each consecutive group of equal elements preserved.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    [OverloadResolutionPriority(1)]
    public static IEnumerable<TSource> DistinctAdjacent<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer) =>
        DistinctAdjacentBy(source, Fn.Identity, comparer);

    /// <summary>
    /// Returns distinct adjacent elements from a sequence by removing consecutive duplicate elements.
    /// Values are compared by using a specified <see cref="IComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to remove consecutive duplicate elements from.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare values.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains distinct adjacent elements from the source sequence,
    /// with only the first occurrence of each consecutive group of equal elements preserved.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> DistinctAdjacent<TSource>(this IEnumerable<TSource> source, IComparer<TSource>? comparer) =>
        DistinctAdjacentBy(source, Fn.Identity, comparer);

    // ------------------------------------------------------------------------

    /// <summary>
    /// Returns distinct adjacent elements from a sequence by removing consecutive duplicate elements
    /// according to a specified key selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of key to distinguish elements by.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to remove consecutive duplicate elements from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains distinct adjacent elements from the source sequence,
    /// with only the first occurrence of each consecutive group of equal elements preserved.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> DistinctAdjacentBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector) =>
        DistinctAdjacentBy(source, keySelector, null);

    /// <summary>
    /// Returns distinct adjacent elements from a sequence by removing consecutive duplicate elements
    /// according to a specified key selector function and
    /// using a specified comparer to compare keys.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of key to distinguish elements by.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to remove consecutive duplicate elements from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains distinct adjacent elements from the source sequence,
    /// with only the first occurrence of each consecutive group of equal elements preserved.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    [OverloadResolutionPriority(1)]
    public static IEnumerable<TSource> DistinctAdjacentBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        comparer ??= EqualityComparer<TKey>.Default;

        return DistinctAdjacentByCore(source, keySelector, comparer.Equals);
    }

    /// <summary>
    /// Returns distinct adjacent elements from a sequence by removing consecutive duplicate elements
    /// according to a specified key selector function and
    /// using a specified comparer to compare keys.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of key to distinguish elements by.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to remove consecutive duplicate elements from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains distinct adjacent elements from the source sequence,
    /// with only the first occurrence of each consecutive group of equal elements preserved.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keySelector"/> is <see langword="null"/>.</exception>
    public static IEnumerable<TSource> DistinctAdjacentBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IComparer<TKey>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        comparer ??= Comparer<TKey>.Default;

        return DistinctAdjacentByCore(source, keySelector, (x, y) => comparer.Compare(x, y) == 0);
    }

    static IEnumerable<TSource> DistinctAdjacentByCore<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, TKey, bool> keyEquals)
    {
        var lastKey = Optional.None<TKey>();

        foreach (var x in source)
        {
            var key = keySelector(x);

            if (lastKey.HasValue && keyEquals(key, lastKey.Value))
                continue;

            yield return x;
            lastKey = key;
        }
    }
}
