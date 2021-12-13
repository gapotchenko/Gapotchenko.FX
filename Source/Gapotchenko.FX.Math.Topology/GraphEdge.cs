using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Provides static methods for creating <see cref="GraphEdge{T}"/> objects.
    /// </summary>
    public static class GraphEdge
    {
        /// <summary>
        /// Creates a new instance of <see cref="GraphEdge{T}"/> object with source and destination vertices.
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The destination vertex.</param>
        /// <returns>The new instance of <see cref="GraphEdge{T}"/> object with specified source and destination vertices.</returns>
        public static GraphEdge<T> Create<T>(T from, T to) => new(from, to);

        /// <summary>
        /// Creates graph edge equality comparer with specified vertex comparer.
        /// </summary>
        /// <param name="vertexComparer">The vertex comparer.</param>
        /// <returns>A new instance of graph edge equality comparer.</returns>
        public static IEqualityComparer<GraphEdge<T>> CreateComparer<T>(IEqualityComparer<T> vertexComparer) =>
            new GraphEdgeEqualityComparer<T>(vertexComparer);
    }
}
