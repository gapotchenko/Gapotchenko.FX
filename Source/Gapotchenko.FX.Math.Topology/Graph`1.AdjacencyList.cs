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
    /// Gets the graph adjacency list.
    /// </summary>
    /// <remarks>
    /// The list consists of rows, each of them representing a set of vertices every item of which relates to another vertex.
    /// </remarks>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected internal IDictionary<TVertex, AdjacencyRow?> AdjacencyList => m_AdjacencyList;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AssociativeArray<TVertex, AdjacencyRow?>? m_ReverseAdjacencyList;

    /// <summary>
    /// Gets a value indicating whether a reverse adjacency list for the current graph is created.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [MemberNotNullWhen(true, nameof(m_ReverseAdjacencyList))]
    protected bool HasReverseAdjacencyList => m_ReverseAdjacencyList != null;

    /// <summary>
    /// Gets a reverse adjacency list for the current graph.
    /// </summary>
    /// <remarks><inheritdoc cref="AdjacencyList"/></remarks>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected IReadOnlyDictionary<TVertex, AdjacencyRow?> ReverseAdjacencyList =>
        m_ReverseAdjacencyList ??= CreateReverseAdjacencyList();

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
