using System.Diagnostics;

namespace Gapotchenko.FX.Math.Topology.Tests.Engine;

sealed class GraphEqualityComparer<T> : IEqualityComparer<IReadOnlyGraph<T>>
{
    public static IEqualityComparer<IReadOnlyGraph<T>> Default =>
        m_CachedDefault ??=
        new GraphEqualityComparer<T>();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static IEqualityComparer<IReadOnlyGraph<T>>? m_CachedDefault;

    GraphEqualityComparer()
    {
    }

    public bool Equals(IReadOnlyGraph<T>? x, IReadOnlyGraph<T>? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        else if (x is null || y is null)
            return false;
        else
            return x.GraphEquals(y);
    }

    public int GetHashCode(IReadOnlyGraph<T> obj) =>
        HashCode.Combine(obj.Vertices.Count, obj.Edges.Count);
}
