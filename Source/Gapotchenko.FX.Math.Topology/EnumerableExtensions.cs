using Gapotchenko.FX.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Provides topological extensions for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Sorts the elements of a sequence in topological order according to a key and specified dependency function.
        /// The sort is stable.
        /// Cyclic dependencies are tolerated and resolved according to the original order of elements in the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence of values to order.</param>
        /// <param name="keySelector">The function to extract a key from an element.</param>
        /// <param name="dependencyFunction">The dependency function that defines dependencies between elements.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are sorted according to a key and specified dependency function.</returns>
        public static IEnumerable<T> TopologicalOrderBy<T, TKey>(
            this IEnumerable<T> source,
            Func<T, TKey> keySelector,
            DependencyFunction<TKey> dependencyFunction,
            IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull
        {
            var positions = new Dictionary<TKey, int>(comparer);
            var list = new List<T>();

            var vertices =
                source
                .Select(
                    value =>
                    {
                        var key = keySelector(value);
                        bool unique = positions.TryAdd(key, list.Count);

                        list.Add(value);

                        if (unique)
                            return Optional.Some(key);
                        else
                            return Optional<TKey>.None;
                    })
                .Where(x => x.HasValue)
                .Select(x => x.Value);

            var g = new Graph<TKey>(
                vertices,
                new GraphIncidenceFunction<TKey>(dependencyFunction),
                comparer);

            // Multi-level hierarchal comparer to target two objectives in one shot:
            //   1. Topological order
            //   2. Positional order
            // Note that this comparer do not support partial orders and thus cannot be used with other sorting algorithms.
            int compare(TKey x, TKey y)
            {
                bool xDependsOnY = g.HasPath(x, y);
                bool yDependsOnX = g.HasPath(y, x);

                // If x depends on y
                if (xDependsOnY)
                {
                    // and there is no circular dependency
                    if (!yDependsOnX)
                    {
                        // then topological order should prevail.
                        return 1;
                    }
                }

                // If y depends on x
                if (yDependsOnX)
                {
                    // and there is no circular dependency
                    if (!xDependsOnY)
                    {
                        // then topological order should prevail.
                        return -1;
                    }
                }

                // Followed by the positional order.
                return 0; // positions[x].CompareTo(positions[y]);
            }

            // Selection sort allows to compensate for lack of partial orders support in comparer.
            int n = list.Count;
            for (int i = 0; i < n - 1; ++i)
            {
                int jMin = i;
                for (int j = i + 1; j < n; ++j)
                {
                    if (compare(keySelector(list[jMin]), keySelector(list[j])) > 0)
                        jMin = j;
                }

                if (jMin != i)
                {
                    var t = list[jMin];
                    list.RemoveAt(jMin);
                    list.Insert(i, t);
                    //var t = list[i];
                    //list[i] = list[jMin];
                    //list[jMin] = t;
                }
            }

            return list;
        }
    }
}
