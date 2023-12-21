// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology;

[DebuggerDisplay($"Order = {{{nameof(Vertices)}.Count}}, Size = {{{nameof(Edges)}.Count}}")]
[DebuggerTypeProxy(typeof(Graph<>.DebugView))]
partial class Graph<TVertex>
{
    sealed class DebugView(Graph<TVertex> graph)
    {
        public object Vertices => graph.Vertices;

        public object Edges => graph.Edges;

        public bool IsDirected => graph.IsDirected;

        public object AdjacencyList => graph.AdjacencyList;
    }
}
