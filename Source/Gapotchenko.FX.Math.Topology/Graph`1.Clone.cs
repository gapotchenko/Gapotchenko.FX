using System;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <summary>
        /// Clones the current graph.
        /// </summary>
        /// <returns>The new graph instance which is a clone of the current graph.</returns>
        public Graph<TVertex> Clone() => new(this, VertexComparer);

        IGraph<TVertex> ICloneable<IGraph<TVertex>>.Clone() => Clone();

        IReadOnlyGraph<TVertex> ICloneable<IReadOnlyGraph<TVertex>>.Clone() => Clone();

        object ICloneable.Clone() => Clone();
    }
}
