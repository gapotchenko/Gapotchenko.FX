using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc/>
        public void ReduceTransitions()
        {
            foreach (var i in AdjacencyList)
            {
                var adjRow = i.Value;
                if (adjRow == null)
                    continue;

                var a = i.Key;

                var removeList = new List<T>();

                foreach (var b in adjRow)
                {
                    if (adjRow.Contains(b) && AreTransitiveVertices(a, b))
                        removeList.Add(b);
                }

                adjRow.ExceptWith(removeList);
            }
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
