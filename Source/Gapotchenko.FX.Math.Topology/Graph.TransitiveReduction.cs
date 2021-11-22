using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc/>
        public void ReduceTransitions()
        {
            bool hasChanges = false;

            foreach (var i in m_AdjacencyList)
            {
                var adjRow = i.Value;
                if (adjRow == null)
                    continue;

                var a = i.Key;

                var removeList = new List<T>();

                foreach (var b in adjRow)
                {
                    if (adjRow.Contains(b) && HasTransitivePath(a, b))
                        removeList.Add(b);
                }

                if (removeList.Count != 0)
                {
                    adjRow.ExceptWith(removeList);
                    hasChanges = true;
                }
            }

            if (hasChanges)
                InvalidateCache();
        }

        /// <summary>
        /// Gets a transitively reduced graph.
        /// </summary>
        /// <returns>The transitively reduced graph.</returns>
        public Graph<T> GetTransitiveReduction()
        {
            var graph = Clone();
            graph.ReduceTransitions();
            return graph;
        }

        IGraph<T> IGraph<T>.GetTransitiveReduction() => GetTransitiveReduction();

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetTransitiveReduction() => GetTransitiveReduction();
    }
}
