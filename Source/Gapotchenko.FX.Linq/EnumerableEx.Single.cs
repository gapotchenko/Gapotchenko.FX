using Gapotchenko.FX.Linq.Properties;
using System.ComponentModel;

namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
    /// <summary>
    /// Returns the only element of a sequence, or the specified default value if the sequence is empty;
    /// this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return the single element of.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// The single element of the input sequence,
    /// or <paramref name="defaultValue"/> if the sequence contains no elements.
    /// </returns>
#if TFF_ENUMERABLE_SINGLEORDEFAULT_VALUE
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource SingleOrDefault<TSource>(IEnumerable<TSource> source, TSource defaultValue) =>
        Enumerable.SingleOrDefault(source, defaultValue);
#else
    public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue) =>
        SingleOrDefaultCore(source, null, defaultValue);
#endif

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or the specified default value if no such element exists;
    /// this method throws an exception if more than one element satisfies the condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return a single element from.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// The single element of the input sequence that satisfies the condition,
    /// or <paramref name="defaultValue"/> if no such element is found.
    /// </returns>
#if TFF_ENUMERABLE_SINGLEORDEFAULT_VALUE
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource SingleOrDefault<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue) =>
        Enumerable.SingleOrDefault(source, predicate, defaultValue);
#else
    public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue) =>
        SingleOrDefaultCore(
            source,
            predicate ?? throw new ArgumentNullException(nameof(predicate)),
            defaultValue);
#endif

#if !TFF_ENUMERABLE_SINGLEORDEFAULT_VALUE
    static TSource SingleOrDefaultCore<TSource>(this IEnumerable<TSource> source, Func<TSource, bool>? predicate, TSource defaultValue)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        Optional<TSource> result = default;

        foreach (var element in source)
        {
            if (predicate == null || predicate(element))
            {
                if (result.HasValue)
                {
                    // More than one element satisfies the condition.        
                    throw new InvalidOperationException(
                        predicate != null ?
                            Resources.MoreThanOneMatchingElement :
                            Resources.MoreThanOneElement);
                }
                else
                {
                    result = element;
                }
            }
        }

        return result.GetValueOrDefault(defaultValue);
    }
#endif
}
