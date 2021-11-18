using System.ComponentModel;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Represents an edge of a graph.
    /// </summary>
    /// <typeparam name="T">The type of vertices in the graph.</typeparam>
    public struct GraphEdge<T>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GraphEdge{T}"/> struct with source and destination vertices.
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The destination vertex.</param>
        public GraphEdge(T from, T to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// The source vertex of the edge.
        /// </summary>
        public T From { get; init; }

        /// <summary>
        /// The destination vertex of the edge.
        /// </summary>
        public T To { get; init; }

        /// <inheritdoc/>
        public override string ToString() => $"{From} -> {To}";

        /// <summary>
        /// Deconstructs the current <see cref="GraphEdge{T}"/>.
        /// </summary>
        /// <param name="from">The source vertex of the current <see cref="GraphEdge{T}"/>.</param>
        /// <param name="to">The destination vertex of the current <see cref="GraphEdge{T}"/>.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out T from, out T to)
        {
            from = From;
            to = To;
        }
    }
}
