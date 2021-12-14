using System;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<T>
    {
        /// <inheritdoc/>
        public bool GraphEquals(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                Vertices.SetEquals(other.Vertices) &&
                Edges.SetEquals(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsInducedSubgraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var vertices = Vertices;

            return
                vertices.IsSubsetOf(other.Vertices) &&
                Edges.SetEquals(other.Edges.Where(e => vertices.Contains(e.From) && vertices.Contains(e.To)));
        }

        /// <inheritdoc/>
        public bool IsProperSubgraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                Vertices.IsProperSubsetOf(other.Vertices) ||
                Edges.IsProperSubsetOf(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsProperSupergraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                Vertices.IsProperSupersetOf(other.Vertices) ||
                Edges.IsProperSupersetOf(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsSubgraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                Vertices.IsSubsetOf(other.Vertices) ||
                Edges.IsSubsetOf(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsSupergraphOf(IReadOnlyGraph<T> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                Vertices.IsSupersetOf(other.Vertices) ||
                Edges.IsSupersetOf(other.Edges);
        }
    }
}
