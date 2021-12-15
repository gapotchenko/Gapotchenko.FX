using System;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <summary>
        /// Clones the current graph.
        /// </summary>
        /// <returns>The new graph instance which is a clone of the current graph.</returns>
        public Graph<T> Clone() => new(this, Comparer);

        IGraph<T> ICloneable<IGraph<T>>.Clone() => Clone();

        IReadOnlyGraph<T> ICloneable<IReadOnlyGraph<T>>.Clone() => Clone();

        object ICloneable.Clone() => Clone();
    }
}
