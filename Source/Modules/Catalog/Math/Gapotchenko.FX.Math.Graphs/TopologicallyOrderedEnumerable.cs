// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Kirill Rode
// Year of introduction: 2021

// This file contains an implementation of the sequence ordering algorithm
// that orders the sequence according to a dependency graph:
// https://blog.gapotchenko.com/stable-topological-sort

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;
using System.Collections;

namespace Gapotchenko.FX.Math.Graphs;

abstract class TopologicallyOrderedEnumerable
{
    #region Primary

    public sealed class PrimaryEnumerable<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
        GraphFactory<TSource, TKey> graphFactory,
        bool reversed = false) :
        PrimaryEnumerable<TSource>(source),
        ITopologicallyOrderedEnumerable<TSource>
    {
        public IEnumerator<TSource> GetEnumerator() => Order(Source).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override IEnumerable<TSource> Order(IEnumerable<TSource> source) => DoOrderTopologicallyBy(source, keySelector, comparer, graphFactory, reversed);

        public ITopologicallyOrderedEnumerable<TSource> Reverse() =>
            new PrimaryEnumerable<TSource, TKey>(Source, keySelector, comparer, graphFactory, !reversed);

        IOrderedEnumerable<TSource> IOrderedEnumerable<TSource>.CreateOrderedEnumerable<TSubsequentKey>(
            Func<TSource, TSubsequentKey> keySelector,
            IComparer<TSubsequentKey>? comparer,
            bool descending) =>
            new SubsequentEnumerable<TSource, TSubsequentKey>(this, keySelector, comparer, descending);
    }

    public abstract class PrimaryEnumerable<TSource>(IEnumerable<TSource> source) : TopologicallyOrderedEnumerable
    {
        public IEnumerable<TSource> Source { get; } = source;

        public abstract IEnumerable<TSource> Order(IEnumerable<TSource> source);
    }

    #endregion

    #region Subsequent

    sealed class SubsequentEnumerable<TSource, TKey>(
        TopologicallyOrderedEnumerable parent,
        Func<TSource, TKey> keySelector,
        IComparer<TKey>? comparer,
        bool descending) :
        SubsequentEnumerable<TSource>(parent),
        IOrderedEnumerable<TSource>
    {
        public IEnumerator<TSource> GetEnumerator()
        {
            PrimaryEnumerable<TSource> primary;
            var subsequentList = new List<SubsequentEnumerable<TSource>>();

            for (SubsequentEnumerable<TSource> i = this; ;)
            {
                subsequentList.Add(i);

                var parent = i.Parent;
                if (parent is SubsequentEnumerable<TSource> subsequent)
                {
                    i = subsequent;
                }
                else
                {
                    primary = (PrimaryEnumerable<TSource>)parent;
                    break;
                }
            }

            var query = primary.Source;

            // Apply subsequent orders.
            for (int i = subsequentList.Count - 1; i >= 0; --i)
                query = subsequentList[i].Order(query);

            // Apply primary order.
            query = primary.Order(query);

            return query.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override IOrderedEnumerable<TSource> Order(IEnumerable<TSource> source)
        {
            if (Parent is PrimaryEnumerable<TSource>)
            {
                // Parented by the primary enumerable.
                if (descending)
                    return source.OrderByDescending(m_KeySelector, comparer);
                else
                    return source.OrderBy(m_KeySelector, comparer);
            }
            else
            {
                var orderedSource = (IOrderedEnumerable<TSource>)source;
                if (descending)
                    return orderedSource.ThenByDescending(m_KeySelector, comparer);
                else
                    return orderedSource.ThenBy(m_KeySelector, comparer);
            }
        }

        readonly Func<TSource, TKey> m_KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

        IOrderedEnumerable<TSource> IOrderedEnumerable<TSource>.CreateOrderedEnumerable<TSubsequentKey>(
            Func<TSource, TSubsequentKey> keySelector,
            IComparer<TSubsequentKey>? comparer,
            bool descending) =>
            new SubsequentEnumerable<TSource, TSubsequentKey>(this, keySelector, comparer, descending);
    }

    abstract class SubsequentEnumerable<TSource>(TopologicallyOrderedEnumerable parent) : TopologicallyOrderedEnumerable
    {
        public TopologicallyOrderedEnumerable Parent { get; } = parent;

        public abstract IOrderedEnumerable<TSource> Order(IEnumerable<TSource> source);
    }

    #endregion

    static IEnumerable<TSource> DoOrderTopologicallyBy<TSource, TKey>(
        IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer,
        GraphFactory<TSource, TKey> graphFactory,
        bool reversed)
    {
        var list = EnumerableEx.AsList(source);
        int n = list.Count;

        if (n < 2)
            return list;

        var g = graphFactory(new(list, keySelector, comparer));

        if (list == source)
            list = list.Clone();

        // Algorithm: Oleksiy Gapotchenko, 2014. Property of public domain.
        // https://blog.gapotchenko.com/stable-topological-sort

        // The usage of selection sort algorithm compensates for the lack of a partial order support in the comparer.
        // The algorithm performs a full scan through the whole range of candidate elements.
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
        // and thus cannot be used with sorting algorithms
        // that do not do a full scan through the whole range of candidate elements.
        static bool Compare(IReadOnlyGraph<TKey> g, TKey x, TKey y) =>
            // If 'x' depends on 'y' and there is no circular dependency then topological order prevails.
            // Otherwise, the positional order provided by the underlying sorting algorithm prevails.
            g.HasPath(x, y) && !g.HasPath(y, x);
    }

    public delegate IReadOnlyGraph<TKey> GraphFactory<TSource, TKey>(in GraphFactoryContext<TSource, TKey> context);

    public readonly struct GraphFactoryContext<TSource, TKey>(
        IEnumerable<TSource> vertices,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer)
    {
        public IEnumerable<TKey> Vertices => vertices.Select(keySelector).Distinct(comparer);
    }
}
