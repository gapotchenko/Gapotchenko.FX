#if BINARY_COMPATIBILITY

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc cref="IncomingVerticesAdjacentTo(TVertex)"/>
    [Obsolete($"Use {nameof(IncomingVerticesAdjacentTo)} instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<TVertex> SourceVerticesAdjacentTo(TVertex vertex) => IncomingVerticesAdjacentTo(vertex);
}

#endif
