// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if BINARY_COMPATIBILITY

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc cref="IncomingVerticesAdjacentTo(TVertex)"/>
    [Obsolete($"Use {nameof(IncomingVerticesAdjacentTo)} instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<TVertex> SourceVerticesAdjacentTo(TVertex vertex) => IncomingVerticesAdjacentTo(vertex);

    /// <inheritdoc cref="OutgoingVerticesAdjacentTo(TVertex)"/>
    [Obsolete($"Use {nameof(OutgoingVerticesAdjacentTo)} instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<TVertex> DestinationVerticesAdjacentTo(TVertex vertex) => OutgoingVerticesAdjacentTo(vertex);
}

#endif
