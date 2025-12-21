// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Linq;

partial class EnumerableExtensions
{
    /// <summary>
    /// Partitions the elements of a sequence according to a specified key selector function.
    /// </summary>
    /// <remarks>
    /// The order of sequence elements is preserved.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to partition.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> sequence of <see cref="IGrouping{TKey, TElement}"/> objects where
    /// each object contains a collection of elements and a key.
    /// </returns>
    public static IEnumerable<IGrouping<TKey, TSource>> PartitionBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector) =>
        PartitionBy(source, keySelector, null);

    /// <summary>
    /// Partitions the elements of a sequence according to a specified key selector function and
    /// compares the keys by using a specified comparer.
    /// </summary>
    /// <remarks>
    /// The order of sequence elements is preserved.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to partition.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> sequence of <see cref="IGrouping{TKey, TElement}"/> objects where
    /// each object contains a collection of elements and a key.
    /// </returns>
    [OverloadResolutionPriority(1)]
    public static IEnumerable<IGrouping<TKey, TSource>> PartitionBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer) =>
        PartitionBy(source, keySelector, Fn.Identity, comparer);

    /// <summary>
    /// Partitions the elements of a sequence according to a specified key selector function and
    /// compares the keys by using a specified comparer.
    /// </summary>
    /// <remarks>
    /// The order of sequence elements is preserved.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to partition.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> sequence of <see cref="IGrouping{TKey, TElement}"/> objects where
    /// each object contains a collection of elements and a key.
    /// </returns>
    public static IEnumerable<IGrouping<TKey, TSource>> PartitionBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IComparer<TKey>? comparer) =>
        PartitionBy(source, keySelector, Fn.Identity, comparer);

    // ------------------------------------------------------------------------

    /// <summary>
    /// Partitions the elements of a sequence according to a specified key selector function and
    /// projects the elements for each partition by using a specified function.
    /// </summary>
    /// <remarks>
    /// The order of sequence elements is preserved.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref="IGrouping{TKey, TElement}"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to partition.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in an <see cref="IGrouping{TKey, TElement}"/>.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> sequence of <see cref="IGrouping{TKey, TElement}"/> objects where
    /// each object contains a collection of elements and a key.
    /// </returns>
    public static IEnumerable<IGrouping<TKey, TElement>> PartitionBy<TSource, TElement, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector) =>
        PartitionBy(source, keySelector, elementSelector, null);

    /// <summary>
    /// Partitions the elements of a sequence according to a key selector function.
    /// The keys are compared by using a comparer and
    /// each group's elements are projected by using a specified function.
    /// </summary>
    /// <remarks>
    /// The order of sequence elements is preserved.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref="IGrouping{TKey, TElement}"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to partition.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in an <see cref="IGrouping{TKey, TElement}"/>.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> sequence of <see cref="IGrouping{TKey, TElement}"/> objects where
    /// each object contains a collection of elements and a key.
    /// </returns>
    [OverloadResolutionPriority(1)]
    public static IEnumerable<IGrouping<TKey, TElement>> PartitionBy<TSource, TElement, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        IEqualityComparer<TKey>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(elementSelector);

        comparer ??= EqualityComparer<TKey>.Default;

        return PartitionByCore(source, keySelector, elementSelector, comparer.Equals);
    }

    /// <summary>
    /// Partitions the elements of a sequence according to a key selector function.
    /// The keys are compared by using a comparer and
    /// each group's elements are projected by using a specified function.
    /// </summary>
    /// <remarks>
    /// The order of sequence elements is preserved.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TElement">The type of the elements in each <see cref="IGrouping{TKey, TElement}"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to partition.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">A function to map each source element to an element in an <see cref="IGrouping{TKey, TElement}"/>.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> sequence of <see cref="IGrouping{TKey, TElement}"/> objects where
    /// each object contains a collection of elements and a key.
    /// </returns>
    public static IEnumerable<IGrouping<TKey, TElement>> PartitionBy<TSource, TElement, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        IComparer<TKey>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(elementSelector);

        comparer ??= Comparer<TKey>.Default;

        return PartitionByCore(source, keySelector, elementSelector, (x, y) => comparer.Compare(x, y) == 0);
    }

    static IEnumerable<IGrouping<TKey, TElement>> PartitionByCore<TSource, TElement, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        Func<TKey, TKey, bool> keyEquals)
    {
        PartitionedGrouping<TKey, TElement>? partition = null;

        foreach (var x in source)
        {
            var key = keySelector(x);

            if (partition != null)
            {
                key = keySelector(x);

                if (!keyEquals(key, partition.Key))
                {
                    yield return partition;
                    partition = null;
                }
            }

            (partition ??= new(key)).Add(elementSelector(x));
        }

        if (partition != null)
            yield return partition;
    }

    // ------------------------------------------------------------------------

    sealed class PartitionedGrouping<TKey, TElement>(TKey key) : Collection<TElement>, IGrouping<TKey, TElement>
    {
        public TKey Key { get; } = key;
    }
}
