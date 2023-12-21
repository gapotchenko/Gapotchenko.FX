// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

#if BINARY_COMPATIBILITY && TFF_DEFAULT_INTERFACE

namespace Gapotchenko.FX.Math.Topology;

partial interface IReadOnlyGraph<TVertex>
{
    /// <inheritdoc cref="IncomingVerticesAdjacentTo(TVertex)"/>
    [Obsolete($"Use {nameof(IncomingVerticesAdjacentTo)} instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    IEnumerable<TVertex> SourceVerticesAdjacentTo(TVertex vertex) => IncomingVerticesAdjacentTo(vertex);

    /// <inheritdoc cref="OutgoingVerticesAdjacentTo(TVertex)"/>
    [Obsolete($"Use {nameof(OutgoingVerticesAdjacentTo)} instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    IEnumerable<TVertex> DestinationVerticesAdjacentTo(TVertex vertex) => OutgoingVerticesAdjacentTo(vertex);
}

#endif
