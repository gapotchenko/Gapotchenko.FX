using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
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

        /// <summary>
        /// Invalidates the cache.
        /// This method should be called if <see cref="AdjacencyList"/> is manipulated directly.
        /// </summary>
        protected void InvalidateCache()
        {
            m_CachedOrder = null;
            m_CachedSize = null;
            IncrementVersion();
        }
    }
}
