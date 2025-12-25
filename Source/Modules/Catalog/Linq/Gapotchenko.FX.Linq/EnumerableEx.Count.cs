namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
    /// <summary>
    /// <para>
    /// Returns the number of elements in a sequence.
    /// </para>
    /// <para>
    /// In contrast to <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>,
    /// this method provides optimized coverage of enumerable types that have a known count value such as
    /// arrays, lists, dictionaries, sets and others.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">A sequence that contains elements to be counted.</param>
    /// <returns>The number of elements in the input sequence.</returns>
    public static int Count<TSource>(IEnumerable<TSource> source) => source.TryGetNonEnumeratedCount() ?? Enumerable.Count(source);

    /// <summary>
    /// <para>
    /// Returns a <see cref="long"/> that represents the total number of elements in a sequence.
    /// </para>
    /// <para>
    /// In contrast to <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>,
    /// this method provides optimized coverage of enumerable types that have a known count value such as
    /// arrays, lists, dictionaries, sets and others.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">A sequence that contains elements to be counted.</param>
    /// <returns>The number of elements in the source sequence.</returns>
    public static long LongCount<TSource>(IEnumerable<TSource> source) => source.TryGetNonEnumeratedCount() ?? Enumerable.LongCount(source);
}
