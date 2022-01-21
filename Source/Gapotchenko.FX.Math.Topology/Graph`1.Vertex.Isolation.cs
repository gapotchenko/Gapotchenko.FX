using Gapotchenko.FX.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <inheritdoc/>
        public bool IsVertexIsolated(TVertex vertex)
        {
            var adjList = m_AdjacencyList;

            if (adjList.TryGetValue(vertex, out var adjRow) &&
                adjRow?.Count > 0)
            {
                return false;
            }

            foreach (var i in adjList)
            {
                adjRow = i.Value;
                if (adjRow == null)
                    continue;

                if (adjRow.Contains(vertex))
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public IEnumerable<TVertex> IsolatedVertices
        {
            get
            {
                var map = new AssociativeArray<TVertex, bool>(VertexComparer);

                foreach (var (v, adjRow) in m_AdjacencyList)
                {
                    if (adjRow?.Count > 0)
                    {
                        foreach (var u in adjRow)
                            map[u] = false;
                        map[v] = false;
                    }
                    else if (!map.ContainsKey(v))
                    {
                        map.Add(v, true);
                    }
                }

                return map.Where(x => x.Value).Select(x => x.Key);
            }
        }
    }
}
