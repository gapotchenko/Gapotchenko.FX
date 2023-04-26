using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology;

[DebuggerDisplay($"Order = {{{nameof(Vertices)}.Count}}, Size = {{{nameof(Edges)}.Count}}")]
[DebuggerTypeProxy(typeof(Graph<>.DebugView))]
partial class Graph<TVertex>
{
    sealed class DebugView
    {
        public DebugView(Graph<TVertex> graph)
        {
            m_Graph = graph;
        }

        readonly Graph<TVertex> m_Graph;

        public object Vertices => m_Graph.Vertices;

        public object Edges => m_Graph.Edges;

        public bool IsDirected => m_Graph.IsDirected;

        public object AdjacencyList => m_Graph.AdjacencyList;
    }
}
