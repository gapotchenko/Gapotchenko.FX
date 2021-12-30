using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <inheritdoc/>
        public bool GraphEquals(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.SetEquals(other.Vertices) &&
                Edges.SetEquals(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsEdgeInducedSubgraphOf(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSubsetOf(other.Vertices) &&
                Edges.IsSubsetOf(GetInducedSubgraphEdges(this, other)) &&
                Vertices.All(v => !IsVertexIsolated(v));
        }

        /// <inheritdoc/>
        public bool IsEdgeInducedSupergraphOf(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSupersetOf(other.Vertices) &&
                Edges.IsSupersetOf(GetInducedSubgraphEdges(this, other)) &&
                other.Vertices.All(v => !other.IsVertexIsolated(v));
        }

        static IEnumerable<GraphEdge<TVertex>> GetInducedSubgraphEdges(IReadOnlyGraph<TVertex> a, IReadOnlyGraph<TVertex> b)
        {
            var aVertices = a.Vertices;
            return b.Edges.Where(e => aVertices.Contains(e.From) && aVertices.Contains(e.To));
        }

        /// <inheritdoc/>
        public bool IsProperSubgraphOf(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                !GraphEquals(other) &&
                IsSubgraphOf(other);
        }

        /// <inheritdoc/>
        public bool IsProperSupergraphOf(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                !GraphEquals(other) &&
                IsSupergraphOf(other);
        }

        /// <inheritdoc/>
        public bool IsSubgraphOf(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSubsetOf(other.Vertices) &&
                Edges.IsSubsetOf(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsSupergraphOf(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSupersetOf(other.Vertices) &&
                Edges.IsSupersetOf(other.Edges);
        }

        /// <inheritdoc/>
        public bool IsVertexInducedSubgraphOf(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSubsetOf(other.Vertices) &&
                VertexInducedSubgraphEdgesEqual(this, other);
        }

        /// <inheritdoc/>
        public bool IsVertexInducedSupergraphOf(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return
                other == this ||
                Vertices.IsSupersetOf(other.Vertices) &&
                VertexInducedSubgraphEdgesEqual(other, this);
        }

        static bool VertexInducedSubgraphEdgesEqual(IReadOnlyGraph<TVertex> a, IReadOnlyGraph<TVertex> b) =>
            a.Edges.SetEquals(GetInducedSubgraphEdges(a, b));

        /// <inheritdoc/>
        public void IntersectWith(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            IntersectWithCore(other);
        }

        void IntersectWithCore(IReadOnlyGraph<TVertex> other)
        {
            Vertices.IntersectWith(other.Vertices);
            Edges.IntersectWith(other.Edges);
        }

        /// <summary>
        /// Gets a graph containing vertices and edges that are present in both the current and a specified graphs.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns>The graph containing vertices and edges that are present in both the current and a specified graphs.</returns>
        public Graph<TVertex> Intersect(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var graph = Clone();
            graph.IntersectWithCore(other);
            return graph;
        }

        /// <inheritdoc/>
        public void ExceptWith(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            ExceptWithCore(other);
        }

        void ExceptWithCore(IReadOnlyGraph<TVertex> other)
        {
            Vertices.ExceptWith(other.Vertices);
            Edges.ExceptWith(other.Edges);
        }

        IGraph<TVertex> IGraph<TVertex>.Intersect(IReadOnlyGraph<TVertex> other) => Intersect(other);

        IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.Intersect(IReadOnlyGraph<TVertex> other) => Intersect(other);

        /// <inheritdoc/>
        public void UnionWith(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            UnionWithCore(other);
        }

        void UnionWithCore(IReadOnlyGraph<TVertex> other)
        {
            Vertices.UnionWith(other.Vertices);
            Edges.UnionWith(other.Edges);
        }

        /// <summary>
        /// Gets a graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.
        /// </summary>
        /// <param name="other">The graph to compare to the current one.</param>
        /// <returns>The graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both.</returns>
        public Graph<TVertex> Union(IReadOnlyGraph<TVertex> other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var graph = Clone();
            graph.UnionWithCore(other);
            return graph;
        }

        IGraph<TVertex> IGraph<TVertex>.Union(IReadOnlyGraph<TVertex> other) => Union(other);

        IReadOnlyGraph<TVertex> IReadOnlyGraph<TVertex>.Union(IReadOnlyGraph<TVertex> other) => Union(other);
    }
}
