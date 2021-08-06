namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc/>
        public void ReduceReflexes()
        {
            foreach (var i in AdjacencyList)
            {
                var adjRow = i.Value;
                if (adjRow == null)
                    continue;

                adjRow.Remove(i.Key);
            }
        }

        /// <summary>
        /// Gets a reflexively reduced graph.
        /// </summary>
        /// <returns>The reflexively reduced graph.</returns>
        public Graph<T> GetReflexiveReduction()
        {
            var graph = Clone();
            graph.ReduceReflexes();
            return graph;
        }

        IGraph<T> IGraph<T>.GetReflexiveReduction() => GetReflexiveReduction();

        IReadOnlyGraph<T> IReadOnlyGraph<T>.GetReflexiveReduction() => GetReflexiveReduction();
    }
}
