namespace Gapotchenko.FX.Math.Intervals.Utils;

readonly struct LazyEqualityComparer<T>(IEqualityComparer<T>? comparer) : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y) => (comparer ?? EqualityComparer<T>.Default).Equals(x, y);

    public int GetHashCode(T obj) => (comparer ?? EqualityComparer<T>.Default).GetHashCode();
}
