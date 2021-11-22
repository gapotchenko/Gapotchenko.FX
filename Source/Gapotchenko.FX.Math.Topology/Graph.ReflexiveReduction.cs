namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc/>
        public void ReduceReflexes()
        {
            bool hasChanges = false;

            foreach (var i in m_AdjacencyList)
            {
                var adjRow = i.Value;
                if (adjRow == null)
                    continue;

                hasChanges |= adjRow.Remove(i.Key);
            }

            if (hasChanges)
                InvalidateCache();
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
