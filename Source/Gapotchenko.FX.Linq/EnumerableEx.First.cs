using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
    /// <summary>
    /// Returns the first element of a sequence, or the specified default value if the sequence contains no elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return the first element of.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// <paramref name="defaultValue"/> if source is empty;
    /// otherwise, the first element in source.
    /// </returns>
#if TFF_ENUMERABLE_FIRSTORDEFAULT_VALUE
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, TSource defaultValue) => Enumerable.FirstOrDefault(source, defaultValue);
#else
    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            // The sequence is empty.
            return defaultValue;
        }

        return enumerator.Current;
    }
#endif

    /// <summary>
    /// Returns the first element of the sequence that satisfies a condition or the specified default value if no such element is found.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return the an element from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// <paramref name="defaultValue"/> if source is empty or if no element passes the test specified by predicate;
    /// otherwise, the first element in source that passes the test specified by predicate.
    /// </returns>
#if TFF_ENUMERABLE_FIRSTORDEFAULT_VALUE
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue) =>
        Enumerable.FirstOrDefault(source, predicate, defaultValue);
#else
    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        foreach (var i in source)
            if (predicate(i))
                return i;

        return defaultValue;
    }
#endif
}
