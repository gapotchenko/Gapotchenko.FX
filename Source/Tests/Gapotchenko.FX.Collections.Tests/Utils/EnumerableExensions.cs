namespace Gapotchenko.FX.Collections.Tests.Utils;

static class EnumerableExensions
{
    /// <summary>
    /// Iterates over the sequence using its enumerator and iteratively returns the enumerated sequence elements.
    /// </summary>
    public static IEnumerable<T> Enumerate<T>(this IEnumerable<T> source)
    {
        foreach (var i in source)
            yield return i;
    }
}
