using Gapotchenko.FX.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics;

#if NETCOREAPP3_0
#pragma warning disable CS8714
#endif

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly AssociativeArray<TVertex, AdjacencyRow?> m_AdjacencyList;

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
    }
}
