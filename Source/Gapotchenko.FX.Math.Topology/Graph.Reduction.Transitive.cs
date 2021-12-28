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

                var from = i.Key;

                List<T>? removeList = null;

                foreach (var to in adjRow)
                {
                    if (HasTransitivePath(from, to))
                    {
                        removeList ??= new List<T>();
                        removeList.Add(to);
                    }
                }

                if (removeList != null)
                {
                    adjRow.ExceptWith(removeList);
                    hasChanges = true;
                }
            }

            if (hasChanges)
                InvalidateCache();
        }

        /// <summary>
        /// <para>
        /// Gets a transitively reduced graph.
        /// </para>
        /// <para>
        /// Transitive reduction prunes the transitive relations that have shorter paths.
        /// </para>
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
