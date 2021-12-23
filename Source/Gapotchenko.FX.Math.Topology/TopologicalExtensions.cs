﻿using Gapotchenko.FX.Collections.Generic;
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
        /// Circular dependencies are tolerated and resolved according to the original order of elements in the sequence.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to order.</param>
        /// <param name="keySelector">The function to extract a key from an element.</param>
        /// <param name="dependencyFunction">
        /// <para>
        /// The dependency function that defines dependencies between elements.
        /// </para>
        /// <para>
        /// Given elements <c>a</c> and <c>b</c>, returns a Boolean value indicating whether <c>b</c> should appear before <c>a</c> in topological order.
        /// </para>
        /// </param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted according to a key and specified dependency function.</returns>
        public static IEnumerable<T> TopologicalOrderBy<T, TKey>(
            this IEnumerable<T> source,
            Func<T, TKey> keySelector,
            DependencyFunction<TKey> dependencyFunction,
            IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (dependencyFunction == null)
                throw new ArgumentNullException(nameof(dependencyFunction));

            var list = EnumerableEx.AsList(source);
            int n = list.Count;

            if (n < 2)
                return list;

            var g = new Graph<TKey>(
                list.Select(keySelector).Distinct(comparer),
                new GraphIncidenceFunction<TKey>(dependencyFunction),
                comparer);

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

            // Selection sort algorithm compensates the lack of partial orders in comparer by doing a full scan.
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
