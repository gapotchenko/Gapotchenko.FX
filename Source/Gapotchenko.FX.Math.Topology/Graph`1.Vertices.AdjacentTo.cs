// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public IEnumerable<TVertex> IncomingVerticesAdjacentTo(TVertex vertex) =>
        IsDirected ?
            new ModificationGuard(this).Protect(IncomingVerticesAdjacentToCore(vertex)) :
            VerticesAdjacentTo(vertex);

    IEnumerable<TVertex> IncomingVerticesAdjacentToCore(TVertex vertex)
    {
        ReverseAdjacencyList.TryGetValue(vertex, out var adjRow);
        return adjRow ?? Enumerable.Empty<TVertex>();
    }

    /// <inheritdoc/>
    public IEnumerable<TVertex> OutgoingVerticesAdjacentTo(TVertex vertex) =>
        IsDirected ?
            new ModificationGuard(this).Protect(OutgoingVerticesAdjacentToCore(vertex)) :
            VerticesAdjacentTo(vertex);

    IEnumerable<TVertex> OutgoingVerticesAdjacentToCore(TVertex vertex)
    {
        m_AdjacencyList.TryGetValue(vertex, out var adjRow);
        return adjRow ?? Enumerable.Empty<TVertex>();
    }

    /// <inheritdoc/>
    public IEnumerable<TVertex> VerticesAdjacentTo(TVertex vertex) =>
        new ModificationGuard(this).Protect(VerticesAdjacentToCore(vertex));

    IEnumerable<TVertex> VerticesAdjacentToCore(TVertex vertex) =>
        IncomingVerticesAdjacentToCore(vertex)
        .Union(OutgoingVerticesAdjacentToCore(vertex), VertexComparer);
}
