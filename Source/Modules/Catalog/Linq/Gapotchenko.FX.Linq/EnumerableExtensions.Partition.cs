// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Linq;

partial class EnumerableExtensions
{
    /// <summary>
    /// Partition a sequence into groups by the given key preserving the order of elements.
    /// </summary>
    /// <typeparam name="TElement">The type of sequence element.</typeparam>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <returns>The sequence of groups.</returns>
    public static IEnumerable<IGrouping<TKey, TElement>> PartitionBy<TElement, TKey>(
        this IEnumerable<TElement> source,
        Func<TElement, TKey> keySelector) =>
        PartitionBy(source, keySelector, null);

    /// <summary>
    /// Partition a sequence into groups by the given key preserving the order of elements
    /// and using a specified equality comparer to compare keys.
    /// </summary>
    /// <typeparam name="TElement">The type of sequence element.</typeparam>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>The sequence of groups.</returns>
    [OverloadResolutionPriority(1)]
    public static IEnumerable<IGrouping<TKey, TElement>> PartitionBy<TElement, TKey>(
        this IEnumerable<TElement> source,
        Func<TElement, TKey> keySelector,
        IEqualityComparer<TKey>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        comparer ??= EqualityComparer<TKey>.Default;

        return PartitionByCore(source, keySelector, comparer.Equals);
    }

    /// <summary>
    /// Partition a sequence into groups by the given key preserving the order of elements
    /// and using a specified comparer to compare keys.
    /// </summary>
    /// <typeparam name="TElement">The type of sequence element.</typeparam>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>The sequence of groups.</returns>
    public static IEnumerable<IGrouping<TKey, TElement>> PartitionBy<TElement, TKey>(
        this IEnumerable<TElement> source,
        Func<TElement, TKey> keySelector,
        IComparer<TKey>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        comparer ??= Comparer<TKey>.Default;

        return PartitionByCore(source, keySelector, (x, y) => comparer.Compare(x, y) == 0);
    }

    static IEnumerable<IGrouping<TKey, TElement>> PartitionByCore<TElement, TKey>(
        this IEnumerable<TElement> source,
        Func<TElement, TKey> keySelector,
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

            (partition ??= new(key)).Add(x);
        }

        if (partition != null)
            yield return partition;
    }

    sealed class PartitionedGrouping<TKey, TElement>(TKey key) : Collection<TElement>, IGrouping<TKey, TElement>
    {
        public TKey Key { get; } = key;
    }
}
