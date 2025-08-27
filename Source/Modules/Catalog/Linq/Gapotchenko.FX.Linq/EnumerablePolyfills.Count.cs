using System.Collections;

namespace Gapotchenko.FX.Linq;

partial class EnumerablePolyfills
{
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
        int? result = source.TryGetNonEnumeratedCount();
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
#if !(TFF_ENUMERABLE_TRYGETNONENUMERATEDCOUNT && NET10_0_OR_GREATER)
                ICollection<TSource> c => c.Count,
                ICollection c => c.Count,
#endif
                _ => null
            };

#if TFF_ENUMERABLE_TRYGETNONENUMERATEDCOUNT
        if (count is null && source.TryGetNonEnumeratedCount(out int n))
            count = n;
#endif

        return count;
    }
}
