namespace Gapotchenko.FX.Collections.Tests.Utils;

static class ComparerExtensions
{
    public static IComparer<T> Reverse<T>(this IComparer<T> comparer) =>
        Comparer<T>.Create((a, b) => comparer.Compare(b, a));
}
