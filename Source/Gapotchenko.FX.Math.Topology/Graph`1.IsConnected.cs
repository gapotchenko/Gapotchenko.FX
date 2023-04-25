using Gapotchenko.FX.Math.Topology.Utils;

namespace Gapotchenko.FX.Math.Topology;

partial class Graph<TVertex>
{
    /// <inheritdoc/>
    public bool IsConnected => IsConnectedHint ??= IsConnectedCore();

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
            return true;

        var vertex = vertices.First();

        // TODO

        throw new NotImplementedException();
    }
}
