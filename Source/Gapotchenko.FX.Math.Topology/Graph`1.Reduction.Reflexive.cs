namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
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
        /// <para>
        /// Gets a reflexive reduction of the current graph.
        /// </para>
        /// <para>
        /// Reflexive reduction prunes the reflexive relations.
        /// Reflexive relation is caused by a vertex that has a connection (edge) to itself.
        /// The removal of such connections prunes the reflexive relations, making a graph reflexively reduced.
        /// </para>
        /// </summary>
        /// <returns>The reflexively reduced graph.</returns>
        public Graph<TVertex> GetReflexiveReduction()
        {
            var graph = Clone();
            graph.ReduceReflexes();
            return graph;
        }

        IGraph<TVertex> IGraph<TVertex>.GetReflexiveReduction() => GetReflexiveReduction();

        IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.GetReflexiveReduction() => GetReflexiveReduction();
    }
}
