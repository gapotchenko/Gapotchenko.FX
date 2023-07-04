namespace Gapotchenko.FX.Collections.Tests.Utils;

static class EnumerableExensions
{
    public static IEnumerable<T> Enumerate<T>(this IEnumerable<T> source)
    {
        foreach (var i in source)
            yield return i;
    }
}
