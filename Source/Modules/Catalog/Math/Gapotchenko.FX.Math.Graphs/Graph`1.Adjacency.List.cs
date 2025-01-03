﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Collections.Generic;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <summary>
    /// Gets the graph adjacency list.
    /// </summary>
    /// <remarks>
    /// The list consists of rows, each of them representing a set of vertices every item of which relates to another vertex.
    /// </remarks>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected IDictionary<TVertex, AdjacencyRow?> AdjacencyList => m_AdjacencyList;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AssociativeArray<TVertex, AdjacencyRow?> m_AdjacencyList;

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
    protected IReadOnlyDictionary<TVertex, AdjacencyRow?> ReverseAdjacencyList => ReverseAdjacencyListCore;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AssociativeArray<TVertex, AdjacencyRow?> ReverseAdjacencyListCore =>
        m_ReverseAdjacencyList ??= CreateReverseAdjacencyList(m_AdjacencyList);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    AssociativeArray<TVertex, AdjacencyRow?>? m_ReverseAdjacencyList;

    static AssociativeArray<TVertex, AdjacencyRow?> CreateReverseAdjacencyList(AssociativeArray<TVertex, AdjacencyRow?> adjList)
    {
        var comparer = adjList.Comparer;
        var revAdjList = new AssociativeArray<TVertex, AdjacencyRow?>(comparer);

        foreach (var (from, adjRow) in adjList)
        {
            bool isolatedVertex = true;

            if (adjRow != null)
            {
                foreach (var to in adjRow)
                {
                    if (!revAdjList.TryGetValue(to, out var revAdjRow))
                    {
                        revAdjRow = new(comparer);
                        revAdjList.Add(to, revAdjRow);
                    }
                    else if (revAdjRow == null)
                    {
                        revAdjRow = new(comparer);
                        revAdjList[to] = revAdjRow;
                    }

                    revAdjRow.Add(from);
                    isolatedVertex = false;
                }
            }

            if (isolatedVertex)
                revAdjList.TryAdd(from, null);
        }

        return revAdjList;
    }
}
