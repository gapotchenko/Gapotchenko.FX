#if NETCOREAPP3_0_OR_GREATER
#define TFF_ENUMERABLE_ZIP2
#endif

#if NET6_0_OR_GREATER
#define TFF_ENUMERABLE_ZIP3
#endif

namespace Gapotchenko.FX.Linq;

partial class EnumerablePolyfills
{
    /// <summary>
    /// Produces a sequence of tuples with elements from the two specified sequences.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <param name="first">The first sequence to merge.</param>
    /// <param name="second">The second sequence to merge.</param>
    /// <returns>
    /// A sequence of tuples with elements taken from the first and second sequences,
    /// in that order.
    /// </returns>
#if TFF_ENUMERABLE_ZIP2
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IEnumerable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(
#if !TFF_ENUMERABLE_ZIP2
        this
#endif
        IEnumerable<TFirst> first,
        IEnumerable<TSecond> second)
    {
#if TFF_ENUMERABLE_ZIP2
        // Redirect to BCL implementation.
        return Enumerable.Zip(first, second);
#else
        if (first is null)
            throw new ArgumentNullException(nameof(first));
        if (second is null)
            throw new ArgumentNullException(nameof(second));

        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();
        while (e1.MoveNext() && e2.MoveNext())
            yield return (e1.Current, e2.Current);
#endif
    }

    /// <summary>
    /// Produces a sequence of tuples with elements from the three specified sequences.
    /// </summary>
    /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TThird">The type of the elements of the third input sequence.</typeparam>
    /// <param name="first">The first sequence to merge.</param>
    /// <param name="second">The second sequence to merge.</param>
    /// <param name="third">The third sequence to merge.</param>
    /// <returns>
    /// A sequence of tuples with elements taken from the first, seconds, and third sequences,
    /// in that order.
    /// </returns>
#if TFF_ENUMERABLE_ZIP3
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static IEnumerable<(TFirst First, TSecond Second, TThird Third)> Zip<TFirst, TSecond, TThird>(
#if !TFF_ENUMERABLE_ZIP3
        this
#endif
        IEnumerable<TFirst> first,
        IEnumerable<TSecond> second,
        IEnumerable<TThird> third)
    {
#if TFF_ENUMERABLE_ZIP3
        // Redirect to BCL implementation.
        return Enumerable.Zip(first, second, third);
#else
        if (first is null)
            throw new ArgumentNullException(nameof(first));
        if (second is null)
            throw new ArgumentNullException(nameof(second));
        if (third is null)
            throw new ArgumentNullException(nameof(third));

        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();
        using var e3 = third.GetEnumerator();
        while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
            yield return (e1.Current, e2.Current, e3.Current);
#endif
    }
}
