using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    public static class Graph
    {
        public static Graph<T> Create<T>(IEnumerable<T> source, DependencyFunction<T> df) where T : notnull
            => Create(source, df, null);

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
