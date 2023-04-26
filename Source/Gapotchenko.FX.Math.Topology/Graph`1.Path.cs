namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public bool HasPath(TVertex from, TVertex to) =>
        Edges.Contains(from, to) || // happy path, free of memory allocations
        HasTransitivePath(from, to); // transitions may incur memory allocations

    /// <summary>
    /// Gets a value indicating whether there is a transitive path from the specified source vertex to the destination.
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
    bool HasTransitivePath(TVertex from, TVertex to)
    {
        var visitedVertices = new HashSet<TVertex>(VertexComparer);
        bool adjacent = false;

        bool CanBeReachedFrom(TVertex from)
        {
            if (!visitedVertices.Add(from))
                return false;

            if (m_AdjacencyList.TryGetValue(from, out var adjRow) &&
                adjRow != null)
            {
                if (adjacent)
                {
                    if (adjRow.Contains(to))
                        return true;
                }
                else
                {
                    adjacent = true;
                }

                foreach (var i in adjRow)
                {
                    if (CanBeReachedFrom(i))
                        return true;
                }
            }

            return false;
        }

        return CanBeReachedFrom(from);
    }
}
