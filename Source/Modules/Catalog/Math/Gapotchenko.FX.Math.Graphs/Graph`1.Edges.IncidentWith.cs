// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public IEnumerable<GraphEdge<TVertex>> IncomingEdgesIncidentWith(TVertex vertex) =>
        IncomingVerticesAdjacentTo(vertex)
        .Select(x => GraphEdge.Create(x, vertex));

    /// <inheritdoc/>
    public IEnumerable<GraphEdge<TVertex>> OutgoingEdgesIncidentWith(TVertex vertex) =>
        OutgoingVerticesAdjacentTo(vertex)
        .Select(x => GraphEdge.Create(vertex, x));

    /// <inheritdoc/>
    public IEnumerable<GraphEdge<TVertex>> EdgesIncidentWith(TVertex vertex)
    {
        return DistinctSelfLoop(
            IncomingEdgesIncidentWith(vertex).Concat(OutgoingEdgesIncidentWith(vertex)),
            VertexComparer);

        static IEnumerable<GraphEdge<TVertex>> DistinctSelfLoop(
            IEnumerable<GraphEdge<TVertex>> source,
            IEqualityComparer<TVertex> vertexComparer)
        {
            bool hasSelfLoop = false;

            foreach (var i in source)
            {
                if (vertexComparer.Equals(i.From, i.To))
                {
                    if (hasSelfLoop)
                        continue;
                    hasSelfLoop = true;
                }

                yield return i;
            }
        }
    }
}
