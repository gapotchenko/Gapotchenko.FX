using Gapotchenko.FX.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#if NETCOREAPP3_0
#pragma warning disable CS8714
#endif

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AssociativeArray<TVertex, AdjacencyRow?> m_AdjacencyList;

    /// <summary>
    /// <para>
    /// Gets the graph adjacency list.
    /// </para>
    /// <para>
    /// The list consists of a number of rows, each of them representing a set of vertices that relate to another vertex.
    /// </para>
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected internal IDictionary<TVertex, AdjacencyRow?> AdjacencyList => m_AdjacencyList;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AssociativeArray<TVertex, AdjacencyRow?>? m_ReverseAdjacencyList;

    /// <summary>
    /// Gets a value indicating whether a reverse adjacency list for the current graph is created.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [MemberNotNull(nameof(m_ReverseAdjacencyList))]
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
    protected bool HasReverseAdjacencyList => m_ReverseAdjacencyList != null;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

    /// <summary>
    /// Gets a reverse adjacency list for the current graph.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected IReadOnlyDictionary<TVertex, AdjacencyRow?> ReverseAdjacencyList
    {
        get
        {
            if (!IsDirected)
                throw new InvalidOperationException("Undirected graph cannot have a reverse adjacency list.");

            return m_ReverseAdjacencyList ??= CreateReverseAdjacencyList();
        }
    }

    AssociativeArray<TVertex, AdjacencyRow?> CreateReverseAdjacencyList()
    {
        var adjList = m_AdjacencyList;
        var revAdjList = new AssociativeArray<TVertex, AdjacencyRow?>(adjList.Comparer);

        foreach (var i in adjList)
        {
            var from = i.Key;
            bool storeFrom = true;

            var adjRow = i.Value;
            if (adjRow != null)
            {
                foreach (var to in adjRow)
                {
                    if (!revAdjList.TryGetValue(to, out var revAdjRow))
                    {
                        revAdjRow = NewAdjacencyRow();
                        revAdjList.Add(to, revAdjRow);
                    }
                    else if (revAdjRow == null)
                    {
                        revAdjRow = NewAdjacencyRow();
                        revAdjList[to] = revAdjRow;
                    }

                    revAdjRow.Add(from);
                    storeFrom = false;
                }
            }

            if (storeFrom)
                revAdjList.TryAdd(from, null);
        }

        return revAdjList;
    }
}
