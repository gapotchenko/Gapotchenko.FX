// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Kirill Rode
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;
using System.Collections;

namespace Gapotchenko.FX.Math.Graphs;

/// <summary>
/// Provides topological sort extension methods.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class TopologicalSortExtensions
{
    /// <summary>
    /// <para>
    /// Sorts the elements of a sequence in topological order according to a key and specified dependency function.
    /// </para>
    /// <para>
    /// The sort is stable.
    /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence of values to order.</param>
    /// <param name="keySelector">The function to extract a key from an element.</param>
    /// <param name="dependencyFunction">
    /// <para>
    /// The dependency function that defines dependencies between the elements of a sequence.
    /// </para>
    /// <para>
    /// Given a pair of element keys as <c>arg1</c> and <c>arg2</c>, returns a Boolean value indicating whether <c>arg2</c> should appear before <c>arg1</c> in the resulting order.
    /// </para>
    /// </param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted in topological order according to a key and specified dependency function.</returns>
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologicallyBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, TKey, bool> dependencyFunction,
        IEqualityComparer<TKey>? comparer = null)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));
        if (dependencyFunction == null)
            throw new ArgumentNullException(nameof(dependencyFunction));

        return new TopologicallyOrderedEnumerable<TSource, TKey>(
            source,
            keySelector,
            comparer,
            vertices => new Graph<TKey>(
                vertices,
                new GraphIncidenceFunction<TKey>(dependencyFunction),
                comparer,
                GraphIncidenceOptions.ReflexiveReduction | GraphIncidenceOptions.Connected));
    }

    /// <summary>
    /// <para>
    /// Sorts the elements of a sequence in topological order according to a key and specified dependency function.
    /// </para>
    /// <para>
    /// The sort is stable.
    /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence of values to order.</param>
    /// <param name="keySelector">The function to extract a key from an element.</param>
    /// <param name="dependencyFunction">
    /// The dependency function that defines dependencies between the elements of a sequence.
    /// Given an element key, returns a set of element keys which must appear before it in the resulting order.
    /// </param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted in topological order according to a key and specified dependency function.</returns>
    public static ITopologicallyOrderedEnumerable<TSource> OrderTopologicallyBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TKey, IEnumerable<TKey>?> dependencyFunction,
        IEqualityComparer<TKey>? comparer = null)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));
        if (dependencyFunction == null)
            throw new ArgumentNullException(nameof(dependencyFunction));

        return new TopologicallyOrderedEnumerable<TSource, TKey>(
            source,
            keySelector,
            comparer,
            vertices =>
            {
                var graph = new Graph<TKey>(comparer);
                var edges = graph.Edges;

                foreach (var vertex in vertices)
                {
                    var adjacentVertices = dependencyFunction(vertex);
                    if (adjacentVertices != null)
                    {
                        foreach (var adjacentVertex in adjacentVertices)
                            edges.Add(vertex, adjacentVertex);
                    }
                }

                return graph;
            });
    }

    abstract class TopologicallyOrderedEnumerable
    {
    }

    abstract class TopologicallyOrderedEnumerable<TSource>(IEnumerable<TSource> source) : TopologicallyOrderedEnumerable
    {
        public IEnumerable<TSource> Source { get; } = source;

        public abstract IEnumerable<TSource> Order(IEnumerable<TSource> source);
    }

    sealed class TopologicallyOrderedEnumerable<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
        Func<IEnumerable<TKey>, Graph<TKey>> graphFactory) :
        TopologicallyOrderedEnumerable<TSource>(source), ITopologicallyOrderedEnumerable<TSource>
    {
        public IEnumerator<TSource> GetEnumerator() => Order(Source).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override IEnumerable<TSource> Order(IEnumerable<TSource> source) => DoOrderTopologicallyBy(source, keySelector, comparer, graphFactory, Reversed);

        public ITopologicallyOrderedEnumerable<TSource> Reverse() =>
            new TopologicallyOrderedEnumerable<TSource, TKey>(Source, keySelector, comparer, graphFactory)
            {
                Reversed = !Reversed
            };

        bool Reversed { get; init; }

        IOrderedEnumerable<TSource> IOrderedEnumerable<TSource>.CreateOrderedEnumerable<TSubsequentKey>(
            Func<TSource, TSubsequentKey> keySelector,
            IComparer<TSubsequentKey>? comparer,
            bool descending) =>
            new NestedTopologicallyOrderedEnumerable<TSource, TSubsequentKey>(this, keySelector, comparer, descending);
    }

    static IEnumerable<TSource> DoOrderTopologicallyBy<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
        Func<IEnumerable<TKey>, Graph<TKey>> graphFactory,
        bool reversed)
    {
        var list = EnumerableEx.AsList(source);
        int n = list.Count;

        if (n < 2)
            return list;

        var g = graphFactory(list.Select(keySelector).Distinct(comparer));

        if (list == source)
            list = list.Clone();

        // Algorithm: Oleksiy Gapotchenko, 2014. Property of public domain.
        // https://blog.gapotchenko.com/stable-topological-sort

        // Selection sort algorithm compensates the lack of partial order support in comparer by
        // doing a full scan through the whole range of candidate elements.
        for (int i = 0; i < n - 1; ++i)
        {
            int jMin = i;
            for (int j = i + 1; j < n; ++j)
            {
                var (a, b) = reversed ? (j, jMin) : (jMin, j);
                if (Compare(g, keySelector(list[a]), keySelector(list[b])))
                    jMin = j;
            }

            if (jMin != i)
            {
                // Stable sort variant.
                var t = list[jMin];
                list.RemoveAt(jMin);
                list.Insert(i, t);
            }
        }

        return list;

        // Note that this comparer does not support partial orders
        // and thus cannot be used with most other sorting algorithms,
        // unless they do a full scan as selection sort does.
        static bool Compare(IReadOnlyGraph<TKey> g, TKey x, TKey y)
        {
            // If x depends on y and there is no circular dependency then topological order should prevail.
            // Otherwise, the positional order provided by the underlying sorting algorithm prevails.
            return g.HasPath(x, y) && !g.HasPath(y, x);
        }
    }

    abstract class NestedTopologicallyOrderedEnumerable<TSource>(TopologicallyOrderedEnumerable parent) : TopologicallyOrderedEnumerable
    {
        public TopologicallyOrderedEnumerable Parent { get; } = parent;

        public abstract IOrderedEnumerable<TSource> Order(IEnumerable<TSource> source);
    }

    sealed class NestedTopologicallyOrderedEnumerable<TSource, TKey> : NestedTopologicallyOrderedEnumerable<TSource>, IOrderedEnumerable<TSource>
    {
        public NestedTopologicallyOrderedEnumerable(
            TopologicallyOrderedEnumerable parent,
            Func<TSource, TKey> keySelector,
            IComparer<TKey>? comparer,
            bool descending) :
            base(parent)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            m_KeySelector = keySelector;
            m_Comparer = comparer;
            m_Descending = descending;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            var list = new List<NestedTopologicallyOrderedEnumerable<TSource>>();
            TopologicallyOrderedEnumerable<TSource> root;

            for (NestedTopologicallyOrderedEnumerable<TSource> i = this; ;)
            {
                list.Add(i);

                var parent = i.Parent;
                if (parent is NestedTopologicallyOrderedEnumerable<TSource> nested)
                {
                    i = nested;
                }
                else
                {
                    root = (TopologicallyOrderedEnumerable<TSource>)parent;
                    break;
                }
            }

            var query = root.Source;

            for (int i = list.Count - 1; i >= 0; --i)
                query = list[i].Order(query);

            query = root.Order(query);

            return query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override IOrderedEnumerable<TSource> Order(IEnumerable<TSource> source)
        {
            if (Parent is TopologicallyOrderedEnumerable<TSource>)
            {
                // Parented by the root enumerable.
                if (m_Descending)
                    return source.OrderByDescending(m_KeySelector, m_Comparer);
                else
                    return source.OrderBy(m_KeySelector, m_Comparer);
            }
            else
            {
                var orderedSource = (IOrderedEnumerable<TSource>)source;
                if (m_Descending)
                    return orderedSource.ThenByDescending(m_KeySelector, m_Comparer);
                else
                    return orderedSource.ThenBy(m_KeySelector, m_Comparer);
            }
        }

        readonly IComparer<TKey>? m_Comparer;
        readonly Func<TSource, TKey> m_KeySelector;
        readonly bool m_Descending;

        IOrderedEnumerable<TSource> IOrderedEnumerable<TSource>.CreateOrderedEnumerable<TSubsequentKey>(
            Func<TSource, TSubsequentKey> keySelector,
            IComparer<TSubsequentKey>? comparer,
            bool descending) =>
            new NestedTopologicallyOrderedEnumerable<TSource, TSubsequentKey>(this, keySelector, comparer, descending);
    }
}
