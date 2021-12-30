using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    [DebuggerDisplay("Count = {Count}")]
    sealed class CountedEnumerableView<T>
    {
        public CountedEnumerableView(IEnumerable<T> source)
        {
            m_Source = source;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEnumerable<T> m_Source;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => m_Source.ToArray();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Count => m_Source.Count();
    }

    sealed class GraphDebugView<TVertex>
    {
        public GraphDebugView(Graph<TVertex> graph)
        {
            m_Graph = graph;
        }

        readonly Graph<TVertex> m_Graph;

        public object Vertices => new CountedEnumerableView<TVertex>(m_Graph.Vertices);

        public object Edges => new CountedEnumerableView<GraphEdge<TVertex>>(m_Graph.Edges);

        public object AdjacencyList => m_Graph.AdjacencyList;
    }
}
