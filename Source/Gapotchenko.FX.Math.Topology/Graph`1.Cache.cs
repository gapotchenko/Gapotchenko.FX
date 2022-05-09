using System.Collections.Specialized;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology
{
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

        const int CF_IsCyclic_HasValue = 1 << 0;
        const int CF_IsCyclic_Value = 1 << 1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        BitVector32 m_CachedFlags;

        void InvalidateCachedRelations()
        {
            m_CachedFlags[CF_IsCyclic_HasValue | CF_IsCyclic_Value] = false;
        }

        /// <summary>
        /// Invalidates the cache.
        /// This method should be called if <see cref="AdjacencyList"/> is manipulated directly.
        /// </summary>
        protected void InvalidateCache()
        {
            m_CachedOrder = null;
            m_CachedSize = null;
            m_ReverseAdjacencyList = null;
            InvalidateCachedRelations();

            IncrementVersion();
        }

        void CopyCacheFrom(Graph<TVertex> graph)
        {
            m_CachedOrder = graph.m_CachedOrder;
            m_CachedSize = graph.m_CachedSize;
            m_CachedFlags = graph.m_CachedFlags;
        }
    }
}
