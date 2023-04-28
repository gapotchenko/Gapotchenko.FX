using Gapotchenko.FX.Math.Topology.Utils;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology;

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
        if (order < 2)
        {
            // A graph containing less than two vertices is connected by definition.
            return true;
        }
        else
        {
            // A graph is connected if every pair of vertices has a path between them.
            var vertex = vertices.First();
            var connectedVertices = VerticesConnectedWithUndirectedCore(vertex);
            return connectedVertices.Count() == order;
        }
    }
}
