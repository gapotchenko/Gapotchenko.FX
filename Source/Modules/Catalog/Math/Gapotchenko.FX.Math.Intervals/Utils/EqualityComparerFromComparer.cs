namespace Gapotchenko.FX.Math.Intervals.Utils;

#pragma warning disable CA1065

readonly struct EqualityComparerFromComparer<T>(IComparer<T>? comparer) : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y) => (comparer ?? Comparer<T>.Default).Compare(x, y) == 0;

    public int GetHashCode(T obj) => throw new NotSupportedException();
}
