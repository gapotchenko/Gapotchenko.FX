#if BINARY_COMPATIBILITY && TFF_DEFAULT_INTERFACE

namespace Gapotchenko.FX.Math.Topology;

partial interface IReadOnlyGraph<TVertex>
{
    /// <inheritdoc cref="IncomingVerticesAdjacentTo(TVertex)"/>
    [Obsolete($"Use {nameof(IncomingVerticesAdjacentTo)} instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    IEnumerable<TVertex> SourceVerticesAdjacentTo(TVertex vertex) => IncomingVerticesAdjacentTo(vertex);
}

#endif
