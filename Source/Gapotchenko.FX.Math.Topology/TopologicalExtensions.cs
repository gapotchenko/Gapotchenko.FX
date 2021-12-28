using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Provides topological extensions for <see cref="IEnumerable{T}"/>.
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
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to order.</param>
        /// <param name="keySelector">The function to extract a key from an element.</param>
        /// <param name="dependencyFunction">
        /// The dependency function that defines dependencies between the elements of a sequence.
        /// Given a pair of element keys as <c>arg1</c> and <c>arg2</c>, returns a Boolean value indicating whether <c>arg2</c> should appear before <c>arg1</c> in topological order.
        /// </param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted according to a key and specified dependency function.</returns>
        public static IEnumerable<T> OrderTopologicallyBy<T, TKey>(
            this IEnumerable<T> source,
            Func<T, TKey> keySelector,
            Func<TKey, TKey, bool> dependencyFunction,
            IEqualityComparer<TKey>? comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (dependencyFunction == null)
                throw new ArgumentNullException(nameof(dependencyFunction));

            return OrderTopologicallyByCore(
                source,
                keySelector,
                comparer,
                vertices => new Graph<TKey>(
                    vertices,
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
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to order.</param>
        /// <param name="keySelector">The function to extract a key from an element.</param>
        /// <param name="dependencyFunction">
        /// The dependency function that defines dependencies between the elements of a sequence.
        /// Given an element key, returns a set of element keys which must appear before it in topological order.
        /// </param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted according to a key and specified dependency function.</returns>
        public static IEnumerable<T> OrderTopologicallyBy<T, TKey>(
            this IEnumerable<T> source,
            Func<T, TKey> keySelector,
            Func<TKey, IEnumerable<TKey>?> dependencyFunction,
            IEqualityComparer<TKey>? comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (dependencyFunction == null)
                throw new ArgumentNullException(nameof(dependencyFunction));

            return OrderTopologicallyByCore(
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

        static IEnumerable<T> OrderTopologicallyByCore<T, TKey>(
            IEnumerable<T> source,
            Func<T, TKey> keySelector,
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
    }
}
