namespace Gapotchenko.FX.Math.Topology
{
    sealed class GraphDebugView<TVertex>
    {
        public GraphDebugView(Graph<TVertex> graph)
        {
            m_Graph = graph;
        }

        readonly Graph<TVertex> m_Graph;

        public object Vertices => m_Graph.Vertices;

        public object Edges => m_Graph.Edges;

        public object AdjacencyList => m_Graph.AdjacencyList;
    }
}
