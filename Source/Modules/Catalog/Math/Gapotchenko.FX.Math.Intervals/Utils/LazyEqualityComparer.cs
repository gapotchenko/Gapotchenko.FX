#if !NET
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member
#endif

namespace Gapotchenko.FX.Math.Intervals.Utils;

readonly struct LazyEqualityComparer<T>(IEqualityComparer<T>? comparer) : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y) => (comparer ?? EqualityComparer<T>.Default).Equals(x, y);

    public int GetHashCode([DisallowNull] T obj) => (comparer ?? EqualityComparer<T>.Default).GetHashCode(obj);
}
