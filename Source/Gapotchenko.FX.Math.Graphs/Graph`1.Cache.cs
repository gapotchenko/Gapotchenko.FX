// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Collections.Specialized;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <summary>
    /// Cached number of vertices.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int? m_CachedOrder = 0;

    /// <summary>
    /// Cached number of edges.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int? m_CachedSize = 0;

    #region Cached flags

    const int CF_IsCyclic_HasValue = 1 << 0;
    const int CF_IsCyclic_Value = 1 << 1;
    const int CF_IsCyclic_Mask = CF_IsCyclic_HasValue | CF_IsCyclic_Value;

    const int CF_IsConnected_HasValue = 1 << 2;
    const int CF_IsConnected_Value = 1 << 3;
    const int CF_IsConnected_Mask = CF_IsConnected_HasValue | CF_IsConnected_Value;

    const int CF_Connectivity_Mask = CF_IsConnected_Mask;
    const int CF_Relations_Mask = CF_IsCyclic_Mask | CF_Connectivity_Mask;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    BitVector32 m_CachedFlags;

    #endregion

    void InvalidateCachedRelations()
    {
        m_CachedFlags[CF_Relations_Mask] = false;
    }

    void InvalidateCachedConnectivity()
    {
        m_CachedFlags[CF_Connectivity_Mask] = false;
    }

    /// <summary>
    /// Invalidates the cache.
    /// </summary>
    /// <remarks>
    /// This method should be called after <see cref="AdjacencyList"/> was manipulated directly.
    /// </remarks>
    protected void InvalidateCache()
    {
        InvalidateCacheCore();
        IncrementVersion();
    }

    /// <summary>
    /// Invalidates the cache without incrementing <see cref="m_Version"/>.
    /// </summary>
    void InvalidateCacheCore()
    {
        m_CachedOrder = null;
        m_CachedSize = null;
        m_ReverseAdjacencyList = null;
        m_CachedFlags = default;
    }

    void CopyCacheFrom(Graph<TVertex> graph)
    {
        m_CachedOrder = graph.m_CachedOrder;
        m_CachedSize = graph.m_CachedSize;
        m_CachedFlags = graph.m_CachedFlags;
    }
}
