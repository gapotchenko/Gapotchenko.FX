// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Math.Graphs.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Graphs;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public bool IsConnected => IsConnectedHint ??= IsConnectedCore();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool? IsConnectedHint
    {
        get => m_CachedFlags.GetNullableBooleanValue(CF_IsConnected_HasValue, CF_IsConnected_Value);
        set => m_CachedFlags.SetNullableBooleanValue(CF_IsConnected_HasValue, CF_IsConnected_Value, value);
    }

    bool IsConnectedCore()
    {
        var vertices = Vertices;
        int order = vertices.Count;

        return
            // A graph containing less than two vertices is connected by definition.
            order < 2 ||
            // A graph is connected if every pair of vertices has a path between them.
            VerticesConnectedUnidirectionallyWithCore(vertices.First()).Count() == order;
    }
}
