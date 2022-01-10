using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Provides topological extension methods.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class TopologicalExtensions
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
        /// <typeparam name="TElement">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to order.</param>
        /// <param name="keySelector">The function to extract a key from an element.</param>
        /// <param name="dependencyFunction">
        /// The dependency function that defines dependencies between the elements of a sequence.
        /// Given a pair of element keys as <c>arg1</c> and <c>arg2</c>, returns a Boolean value indicating whether <c>arg2</c> should appear before <c>arg1</c> in topological order.
        /// </param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted according to a key and specified dependency function.</returns>
        public static IOrderedEnumerable<TElement> OrderTopologicallyBy<TElement, TKey>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            Func<TKey, TKey, bool> dependencyFunction,
            IEqualityComparer<TKey>? comparer = null)
            => OrderTopologicallyBy(source, keySelector, dependencyFunction, comparer, false);

        /// <summary>
        /// <para>
        /// Sorts the elements of a sequence in reverse topological order according to a key and specified dependency function.
        /// </para>
        /// <para>
        /// The sort is stable.
        /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
        /// </para>
        /// </summary>
        /// <typeparam name="TElement">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to order.</param>
        /// <param name="keySelector">The function to extract a key from an element.</param>
        /// <param name="dependencyFunction">
        /// The dependency function that defines dependencies between the elements of a sequence.
        /// Given a pair of element keys as <c>arg1</c> and <c>arg2</c>, returns a Boolean value indicating whether <c>arg2</c> should appear before <c>arg1</c> in topological order.
        /// </param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted according to a key and specified dependency function.</returns>
        public static IOrderedEnumerable<TElement> OrderTopologicallyByReverse<TElement, TKey>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            Func<TKey, TKey, bool> dependencyFunction,
            IEqualityComparer<TKey>? comparer = null)
            => OrderTopologicallyBy(source, keySelector, dependencyFunction, comparer, true);

        static IOrderedEnumerable<TElement> OrderTopologicallyBy<TElement, TKey>(
            IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            Func<TKey, TKey, bool> dependencyFunction,
            IEqualityComparer<TKey>? comparer,
            bool reverse)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (dependencyFunction == null)
                throw new ArgumentNullException(nameof(dependencyFunction));

            return OrderTopologicallyBy(
                source,
                keySelector,
                comparer,
                vertices => new Graph<TKey>(
                    vertices,
                    reverse ?
                        (a, b) => dependencyFunction(b, a) :
                        new GraphIncidenceFunction<TKey>(dependencyFunction),
                    comparer,
                    GraphIncidenceOptions.ReflexiveReduction | GraphIncidenceOptions.ExcludeIsolatedVertices));
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
        /// <typeparam name="TElement">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to order.</param>
        /// <param name="keySelector">The function to extract a key from an element.</param>
        /// <param name="dependencyFunction">
        /// The dependency function that defines dependencies between the elements of a sequence.
        /// Given an element key, returns a set of element keys which must appear before it in topological order.
        /// </param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted according to a key and specified dependency function.</returns>
        public static IOrderedEnumerable<TElement> OrderTopologicallyBy<TElement, TKey>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            Func<TKey, IEnumerable<TKey>?> dependencyFunction,
            IEqualityComparer<TKey>? comparer = null)
            => OrderTopologicallyBy(source, keySelector, dependencyFunction, comparer, false);

        /// <summary>
        /// <para>
        /// Sorts the elements of a sequence in reverse topological order according to a key and specified dependency function.
        /// </para>
        /// <para>
        /// The sort is stable.
        /// Circular dependencies are ignored and resolved according to the original order of elements in the sequence.
        /// </para>
        /// </summary>
        /// <typeparam name="TElement">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to order.</param>
        /// <param name="keySelector">The function to extract a key from an element.</param>
        /// <param name="dependencyFunction">
        /// The dependency function that defines dependencies between the elements of a sequence.
        /// Given an element key, returns a set of element keys which must appear before it in topological order.
        /// </param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted according to a key and specified dependency function.</returns>
        public static IOrderedEnumerable<TElement> OrderTopologicallyByReverse<TElement, TKey>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            Func<TKey, IEnumerable<TKey>?> dependencyFunction,
            IEqualityComparer<TKey>? comparer = null)
            => OrderTopologicallyBy(source, keySelector, dependencyFunction, comparer, true);

        static IOrderedEnumerable<TElement> OrderTopologicallyBy<TElement, TKey>(
            this IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            Func<TKey, IEnumerable<TKey>?> dependencyFunction,
            IEqualityComparer<TKey>? comparer,
            bool reverse)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (dependencyFunction == null)
                throw new ArgumentNullException(nameof(dependencyFunction));

            return OrderTopologicallyBy(
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
                            {
                                if (reverse)
                                    edges.Add(adjacentVertex, vertex);
                                else
                                    edges.Add(vertex, adjacentVertex);
                            }
                        }
                    }

                    return graph;
                });
        }

        static IOrderedEnumerable<TElement> OrderTopologicallyBy<TElement, TKey>(
            IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? comparer,
            Func<IEnumerable<TKey>, Graph<TKey>> graphFactory)
        {
            return new TopologicallyOrderedEnumerable<TElement, TKey>(
                source,
                keySelector,
                comparer,
                graphFactory);
        }

        static IEnumerable<TElement> OrderTopologicallyByCore<TElement, TKey>(
            IEnumerable<TElement> source,
            Func<TElement, TKey> keySelector,
            IEqualityComparer<TKey>? comparer,
            Func<IEnumerable<TKey>, Graph<TKey>> graphFactory)
        {
            var list = EnumerableEx.AsList(source);
            int n = list.Count;

            if (n < 2)
                return list;

            var g = graphFactory(list.Select(keySelector).Distinct(comparer));

            if (list == source)
                list = list.Clone();

            // Note that this comparer does not support partial orders
            // and thus cannot be used with most other sorting algorithms,
            // unless they do a full scan as selection sort does.
            bool compare(TKey x, TKey y)
            {
                // If x depends on y and there is no circular dependency then topological order should prevail.
                // Otherwise, the positional order provided by the underlying sorting algorithm prevails.
                return g.HasPath(x, y) && !g.HasPath(y, x);
            }

            // Selection sort algorithm compensates the lack of partial order support in comparer by
            // doing a full scan through the whole range of candidate elements.
            for (int i = 0; i < n - 1; ++i)
            {
                int jMin = i;
                for (int j = i + 1; j < n; ++j)
                {
                    if (compare(keySelector(list[jMin]), keySelector(list[j])))
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
        }

        abstract class TopologicallyOrderedEnumerable
        {
        }

        abstract class TopologicallyOrderedEnumerable<TElement> : TopologicallyOrderedEnumerable
        {
            public TopologicallyOrderedEnumerable(IEnumerable<TElement> source)
            {
                Source = source;
            }

            public IEnumerable<TElement> Source { get; }

            public abstract IEnumerable<TElement> Order(IEnumerable<TElement> source);
        }

        sealed class TopologicallyOrderedEnumerable<TElement, TKey> : TopologicallyOrderedEnumerable<TElement>, IOrderedEnumerable<TElement>
        {
            public TopologicallyOrderedEnumerable(
                IEnumerable<TElement> source,
                Func<TElement, TKey> keySelector,
                IEqualityComparer<TKey>? comparer,
                Func<IEnumerable<TKey>, Graph<TKey>> graphFactory) :
                base(source)
            {
                m_KeySelector = keySelector;
                m_Comparer = comparer;
                m_GraphFactory = graphFactory;
            }

            readonly Func<TElement, TKey> m_KeySelector;
            readonly IEqualityComparer<TKey>? m_Comparer;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Func<IEnumerable<TKey>, Graph<TKey>> m_GraphFactory;

            public override IEnumerable<TElement> Order(IEnumerable<TElement> source) => OrderTopologicallyByCore(source, m_KeySelector, m_Comparer, m_GraphFactory);

            public IEnumerator<TElement> GetEnumerator() => Order(Source).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IOrderedEnumerable<TElement> IOrderedEnumerable<TElement>.CreateOrderedEnumerable<TNestedKey>(
                Func<TElement, TNestedKey> keySelector,
                IComparer<TNestedKey>? comparer,
                bool descending)
                => new NestedTopologicallyOrderedEnumerable<TElement, TNestedKey>(keySelector, comparer, descending, this);
        }

        abstract class NestedTopologicallyOrderedEnumerable<TElement> : TopologicallyOrderedEnumerable
        {
            public NestedTopologicallyOrderedEnumerable(TopologicallyOrderedEnumerable next)
            {
                Next = next;
            }

            public TopologicallyOrderedEnumerable Next { get; }

            public abstract IOrderedEnumerable<TElement> Order(IEnumerable<TElement> source);
        }

        sealed class NestedTopologicallyOrderedEnumerable<TElement, TKey> : NestedTopologicallyOrderedEnumerable<TElement>, IOrderedEnumerable<TElement>
        {
            public NestedTopologicallyOrderedEnumerable(
                Func<TElement, TKey> keySelector,
                IComparer<TKey>? comparer,
                bool descending,
                TopologicallyOrderedEnumerable next)
                : base(next)
            {
                m_KeySelector = keySelector;
                m_Comparer = comparer;
                m_Descending = descending;
            }

            readonly Func<TElement, TKey> m_KeySelector;
            readonly IComparer<TKey>? m_Comparer;
            readonly bool m_Descending;

            public override IOrderedEnumerable<TElement> Order(IEnumerable<TElement> source)
            {
                if (Next is TopologicallyOrderedEnumerable<TElement>)
                {
                    // Parented by the root enumerable.
                    if (m_Descending)
                        return source.OrderByDescending(m_KeySelector, m_Comparer);
                    else
                        return source.OrderBy(m_KeySelector, m_Comparer);
                }
                else
                {
                    var orderedSource = (IOrderedEnumerable<TElement>)source;
                    if (m_Descending)
                        return orderedSource.ThenByDescending(m_KeySelector, m_Comparer);
                    else
                        return orderedSource.ThenBy(m_KeySelector, m_Comparer);
                }
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                var list = new List<NestedTopologicallyOrderedEnumerable<TElement>>();

                TopologicallyOrderedEnumerable<TElement> root;

                for (NestedTopologicallyOrderedEnumerable<TElement> i = this; ;)
                {
                    list.Add(i);

                    var next = i.Next;
                    if (next is NestedTopologicallyOrderedEnumerable<TElement> nested)
                    {
                        i = nested;
                    }
                    else
                    {
                        root = (TopologicallyOrderedEnumerable<TElement>)next;
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

            IOrderedEnumerable<TElement> IOrderedEnumerable<TElement>.CreateOrderedEnumerable<TNestedKey>(
                Func<TElement, TNestedKey> keySelector,
                IComparer<TNestedKey>? comparer,
                bool descending)
                => new NestedTopologicallyOrderedEnumerable<TElement, TNestedKey>(keySelector, comparer, descending, this);
        }
    }
}
