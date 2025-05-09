namespace Gapotchenko.FX.Math.Graphs.Tests.Engine;

static class Util
{
    public static bool SetEquals<T>(this ISet<GraphEdge<T>> edgeSet, IEnumerable<(T From, T To)> other)
    {
        ArgumentNullException.ThrowIfNull(edgeSet);
        ArgumentNullException.ThrowIfNull(other);

        return edgeSet.SetEquals(Conv(other));
    }

    static IEnumerable<GraphEdge<T>> Conv<T>(IEnumerable<(T, T)> other) => other.Select(x => (GraphEdge<T>)x);
}
