namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <summary>
    /// Gets a value indicating whether there is a transitive path from the specified source vertex to a destination.
    /// </summary>
    /// <remarks>
    /// A transitive path consists of two or more edges with at least one intermediate vertex.
    /// </remarks>
    /// <param name="from">The source vertex.</param>
    /// <param name="to">The destination vertex.</param>
    /// <returns>
    /// <see langword="true"/> when the specified source vertex can reach the destination via one or more intermediate vertices; 
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool HasTransitivePath(TVertex from, TVertex to) => new PathTraverser(this, to, false).CanBeReachedFrom(from);

    /// <inheritdoc/>
    public bool HasPath(TVertex from, TVertex to) => Edges.Contains(from, to) || HasTransitivePath(from, to);

    struct PathTraverser
    {
        public PathTraverser(Graph<TVertex> graph, TVertex destination, bool adjacent)
        {
            m_Graph = graph;
            m_Destination = destination;

            m_VisitedNodes = new HashSet<TVertex>(graph.VertexComparer);
            m_Adjacent = adjacent;
        }

        readonly Graph<TVertex> m_Graph;
        readonly TVertex m_Destination;

        readonly HashSet<TVertex> m_VisitedNodes;
        bool m_Adjacent;

        public bool CanBeReachedFrom(TVertex source)
        {
            if (!m_VisitedNodes.Add(source))
                return false;

            if (m_Graph.m_AdjacencyList.TryGetValue(source, out var adjRow) &&
                adjRow != null)
            {
                if (m_Adjacent)
                {
                    if (adjRow.Contains(m_Destination))
                        return true;
                }
                else
                {
                    m_Adjacent = true;
                }

                foreach (var i in adjRow)
                {
                    if (CanBeReachedFrom(i))
                        return true;
                }
            }

            return false;
        }
    }
}
