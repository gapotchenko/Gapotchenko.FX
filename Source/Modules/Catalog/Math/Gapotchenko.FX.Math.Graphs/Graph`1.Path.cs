// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public bool HasPath(TVertex from, TVertex to) =>
        Edges.Contains(from, to) || // happy path, free of memory allocations
        HasTransitivePath(from, to); // transitive traversing may incur memory allocations

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
    bool HasTransitivePath(TVertex from, TVertex to) =>
        IsDirected ?
            HasDirectedTransitivePathCore(from, to) :
            HasUndirectedTransitivePathCore(from, to);

    bool HasDirectedTransitivePathCore(TVertex from, TVertex to)
    {
        var visitedVertices = new HashSet<TVertex>(VertexComparer);

        // Using local variable instead of a function parameter to reduce the size of stack
        // used by the recursive call chain.
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

    bool HasUndirectedTransitivePathCore(TVertex from, TVertex to)
    {
        var visitedVertices = new HashSet<TVertex>(VertexComparer);

        bool CanBeReachedFrom(TVertex from, bool adjacent)
        {
            if (!visitedVertices.Add(from))
                return false;

            return
                CanBeReachedUsingAdjacencyList(m_AdjacencyList) ||
                CanBeReachedUsingAdjacencyList(ReverseAdjacencyList);

            bool CanBeReachedUsingAdjacencyList(IReadOnlyDictionary<TVertex, AdjacencyRow?> adjList)
            {
                if (adjList.TryGetValue(from, out var adjRow) &&
                    adjRow != null)
                {
                    if (adjacent)
                    {
                        if (adjRow.Contains(to))
                            return true;
                    }

                    foreach (var i in adjRow)
                    {
                        if (CanBeReachedFrom(i, true))
                            return true;
                    }
                }

                return false;
            }
        }

        return CanBeReachedFrom(from, false);
    }
}
