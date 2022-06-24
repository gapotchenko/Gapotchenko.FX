using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public static int Count<TSource>(IEnumerable<TSource> source) => TryGetNonEnumeratedCount(source) ?? Enumerable.Count(source);

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
    public static long LongCount<TSource>(IEnumerable<TSource> source) => TryGetNonEnumeratedCount(source) ?? Enumerable.LongCount(source);

    /// <summary>
    /// Checks whether the number of elements in a sequence is greater or equal to a specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">A sequence that contains elements to be counted.</param>
    /// <param name="value">The value to compare the count of elements in a sequence with.</param>
    /// <returns><c>true</c> if the number of elements in a sequence is greater or equal to a specified <paramref name="value"/>; otherwise, <c>false</c>.</returns>
    public static bool CountIsAtLeast<TSource>(this IEnumerable<TSource> source, int value)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (value <= 0)
            return true;

        if (TryGetNonEnumeratedCount(source, out var optimizedCount))
            return optimizedCount >= value;

        using var enumerator = source.GetEnumerator();

        int count = 0;
        checked
        {
            while (enumerator.MoveNext())
            {
                ++count;
                if (count >= value)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to determine the number of elements in a sequence without forcing an enumeration.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">A sequence that contains elements to be counted.</param>
    /// <param name="count">
    /// When this method returns,
    /// contains the count of <paramref name="source" /> if successful,
    /// or zero if the method failed to determine the count.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the count of <paramref name="source"/> can be determined without enumeration;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool TryGetNonEnumeratedCount<TSource>(
#if !TFF_ENUMERABLE_TRYGETNONENUMERATEDCOUNT
        this
#endif
        IEnumerable<TSource> source,
        out int count)
    {
        var result = TryGetNonEnumeratedCount(source);
        count = result.GetValueOrDefault();
        return result.HasValue;
    }

    /// <summary>
    /// Attempts to determine the number of elements in a sequence without forcing an enumeration.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <param name="source">A sequence that contains elements to be counted.</param>
    /// <returns>
    /// The number of elements in <paramref name="source" /> sequence,
    /// or <see langword="null"/> if the number cannot be determined without enumeration.
    /// </returns>
    public static int? TryGetNonEnumeratedCount<TSource>(this IEnumerable<TSource> source)
    {
        int? count =
            source switch
            {
                null => throw new ArgumentNullException(nameof(source)),
                IReadOnlyCollection<TSource> roc => roc.Count,
                ICollection<TSource> c => c.Count,
                ICollection c => c.Count,
                _ => null
            };

#if TFF_ENUMERABLE_TRYGETNONENUMERATEDCOUNT
        if (count == null && source.TryGetNonEnumeratedCount(out var n))
            count = n;
#endif

        return count;
    }
}
