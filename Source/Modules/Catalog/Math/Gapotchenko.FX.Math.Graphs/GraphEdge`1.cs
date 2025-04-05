// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

/// <summary>
/// Represents an edge of a graph.
/// </summary>
/// <typeparam name="TVertex">The type of end-vertices in the edge.</typeparam>
/// <remarks>
/// Initializes a new instance of <see cref="GraphEdge{TVertex}"/> structure with source and destination end-vertices.
/// </remarks>
/// <param name="from">The source vertex.</param>
/// <param name="to">The destination vertex.</param>
[DebuggerDisplay($"{{{nameof(From)}}} -> {{{nameof(To)}}}")]
public readonly struct GraphEdge<TVertex>(TVertex from, TVertex to)
{
    /// <summary>
    /// The source end-vertex of the edge.
    /// </summary>
    public TVertex From { get; init; } = from;

    /// <summary>
    /// The destination end-vertex of the edge.
    /// </summary>
    public TVertex To { get; init; } = to;

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
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IEnumerable<TVertex> IncidentVertices => [From, To];

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
    /// Converts a value tuple to a <see cref="GraphEdge{TVertex}"/> instance.
    /// </summary>
    /// <param name="value">The value tuple to convert.</param>
    public static implicit operator GraphEdge<TVertex>((TVertex From, TVertex To) value) => new(value.From, value.To);

    /// <summary>
    /// Converts a <see cref="GraphEdge{TVertex}"/> value to a value tuple instance.
    /// </summary>
    /// <param name="value">The <see cref="GraphEdge{TVertex}"/> value to convert.</param>
    public static implicit operator (TVertex From, TVertex To)(GraphEdge<TVertex> value) => (value.From, value.To);

    /// <summary>
    /// Converts a tuple to a <see cref="GraphEdge{TVertex}"/> instance.
    /// </summary>
    /// <param name="value">The value tuple to convert.</param>
    public static implicit operator GraphEdge<TVertex>(Tuple<TVertex, TVertex> value)
    {
        if (value == null)
            return default;
        else
            return new(value.Item1, value.Item2);
    }

    /// <summary>
    /// Converts a <see cref="GraphEdge{TVertex}"/> value to a tuple instance.
    /// </summary>
    /// <param name="value">The <see cref="GraphEdge{TVertex}"/> value to convert.</param>
    public static implicit operator Tuple<TVertex, TVertex>(GraphEdge<TVertex> value) => Tuple.Create(value.From, value.To);
}
