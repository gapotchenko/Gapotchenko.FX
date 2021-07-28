using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Provides operations for a graph.
    /// </summary>
    public static class Graph
    {
        /// <summary>
        /// Creates a new graph from the sequence of elements and a dependency function that specifies how elements relate to each other.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="source">The sequence of elements.</param>
        /// <param name="df">The dependency function.</param>
        /// <returns>The graph whose vertices are specified by the <paramref name="source"/> and edges represent the relations defined by the dependency function <paramref name="df"/>.</returns>
        public static Graph<T> Create<T>(IEnumerable<T> source, DependencyFunction<T> df) where T : notnull
            => Create(source, df, null);

        /// <summary>
        /// Creates a new graph from the sequence of elements and a dependency function that specifies how elements relate to each other.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="source">The sequence of elements.</param>
        /// <param name="df">The dependency function.</param>
        /// <param name="comparer">The equality comparer.</param>
        /// <returns>The graph whose vertices are specified by the <paramref name="source"/> and edges represent the relations defined by the dependency function <paramref name="df"/>.</returns>
        public static Graph<T> Create<T>(IEnumerable<T> source, DependencyFunction<T> df, IEqualityComparer<T>? comparer) where T : notnull
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (df == null)
                throw new ArgumentNullException(nameof(df));

            var list = source.ToList();

            var graph = new Graph<T>(comparer);

            int count = list.Count;

            for (int i = 0; i < count; ++i)
            {
                var ei = list[i];
                bool edge = false;

                for (int j = 0; j < count; ++j)
                {
                    var ej = list[j];

                    if (df(ei, ej))
                    {
                        graph.AddEdge(ei, ej);
                        edge = true;
                    }
                }

                if (!edge)
                    graph.AddVertex(ei);
            }

            return graph;
        }
    }
}
