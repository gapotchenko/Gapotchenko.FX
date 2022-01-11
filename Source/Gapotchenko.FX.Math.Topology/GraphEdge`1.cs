using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Represents an edge of a graph.
    /// </summary>
    /// <typeparam name="TVertex">The type of end-vertices in the edge.</typeparam>
    public readonly struct GraphEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GraphEdge{T}"/> struct with source and destination end-vertices.
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The destination vertex.</param>
        public GraphEdge(TVertex from, TVertex to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// The source end-vertex of the edge.
        /// </summary>
        public TVertex From { get; init; }

        /// <summary>
        /// The destination end-vertex of the edge.
        /// </summary>
        public TVertex To { get; init; }

        /// <summary>
        /// Reverses the edge direction by swapping source and destination end-vertices.
        /// </summary>
        /// <returns>The edge with reversed direction.</returns>
        public GraphEdge<TVertex> Reverse() => new(To, From);

        /// <summary>
        /// <para>
        /// Gets a sequence of vertices incident with the current edge.
        /// </para>
        /// <para>
        /// Incident vertices of an edge are its end-vertices, <see cref="From"/> and <see cref="To"/>.
        /// </para>
        /// </summary>
        public IEnumerable<TVertex> IncidentVertices => new[] { From, To };

        /// <inheritdoc/>
        public override string ToString() => $"{From} -> {To}";

        /// <summary>
        /// Deconstructs the current <see cref="GraphEdge{TVertex}"/>.
        /// </summary>
        /// <param name="from">The source end-vertex of the current <see cref="GraphEdge{TVertex}"/>.</param>
        /// <param name="to">The destination end-vertex of the current <see cref="GraphEdge{TVertex}"/>.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out TVertex from, out TVertex to)
        {
            from = From;
            to = To;
        }

        /// <summary>
        /// Converts a value tuple to a <see cref="GraphEdge{T}"/> instance.
        /// </summary>
        /// <param name="value">The value tuple to convert.</param>
        public static implicit operator GraphEdge<TVertex>((TVertex From, TVertex To) value) => new(value.From, value.To);

        /// <summary>
        /// Converts a <see cref="GraphEdge{T}"/> value to a value tuple instance.
        /// </summary>
        /// <param name="value">The <see cref="GraphEdge{T}"/> value to convert.</param>
        public static implicit operator (TVertex From, TVertex To)(GraphEdge<TVertex> value) => (value.From, value.To);

        /// <summary>
        /// Converts a tuple to a <see cref="GraphEdge{T}"/> instance.
        /// </summary>
        /// <param name="value">The value tuple to convert.</param>
        public static implicit operator GraphEdge<TVertex>(Tuple<TVertex, TVertex> value) => new(value.Item1, value.Item2);

        /// <summary>
        /// Converts a <see cref="GraphEdge{T}"/> value to a tuple instance.
        /// </summary>
        /// <param name="value">The <see cref="GraphEdge{T}"/> value to convert.</param>
        public static implicit operator Tuple<TVertex, TVertex>(GraphEdge<TVertex> value) => Tuple.Create(value.From, value.To);
    }
}
