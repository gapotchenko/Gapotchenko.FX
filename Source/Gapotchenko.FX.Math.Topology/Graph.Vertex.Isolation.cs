using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc/>
        public bool IsVertexIsolated(T vertex)
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
        public IEnumerable<T> IsolatedVertices
        {
            get
            {
                var map = new Dictionary<T, bool>(Comparer);

                foreach (var (v, adjRow) in m_AdjacencyList)
                {
                    if (adjRow?.Count > 0)
                    {
                        foreach (var u in adjRow)
                        {
                            if (!map.ContainsKey(u))
                                map.Add(u, true);
                            else
                                map[u] = false;
                        }

                        map[v] = false;
                        continue;
                    }

                    if (!map.ContainsKey(v))
                        map.Add(v, true);
                }

                return map.Where(x => x.Value).Select(x => x.Key);
            }
        }
    }
}
