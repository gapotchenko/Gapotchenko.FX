using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Provides static methods for creating <see cref="GraphEdge{TVertex}"/> objects.
    /// </summary>
    public static class GraphEdge
    {
        /// <summary>
        /// Creates a new instance of <see cref="GraphEdge{TVertex}"/> object with source and destination vertices.
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The destination vertex.</param>
        /// <returns>The new instance of <see cref="GraphEdge{TVertex}"/> object with specified source and destination vertices.</returns>
        public static GraphEdge<TVertex> Create<TVertex>(TVertex from, TVertex to) => new(from, to);

        /// <summary>
        /// Creates graph edge equality comparer with specified vertex comparer.
        /// </summary>
        /// <param name="vertexComparer">The vertex comparer.</param>
        /// <returns>A new instance of graph edge equality comparer.</returns>
        internal static IEqualityComparer<GraphEdge<TVertex>> CreateComparer<TVertex>(IEqualityComparer<TVertex> vertexComparer) =>
            new GraphEdgeEqualityComparer<TVertex>(vertexComparer);
    }
}
