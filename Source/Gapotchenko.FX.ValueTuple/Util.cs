namespace Gapotchenko.FX.ValueTuple;

static class Util
{
    public static int GetSafeHashCode<T>(IEqualityComparer<T> comparer, T value) =>
        value is null ?
            HashCode.Combine(0) :
            comparer.GetHashCode(value);
}
