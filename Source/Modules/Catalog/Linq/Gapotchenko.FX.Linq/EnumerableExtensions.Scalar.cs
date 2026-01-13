// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Linq;

partial class EnumerableExtensions
{
    /// <summary>
    /// Returns the only element of a sequence, or a <see langword="default"/> value if the sequence is empty or contains several elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return the scalar element of.</param>
    /// <returns>
    /// The only element of the input sequence, or <see langword="default"/> value if the sequence is empty or contains several elements.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static TSource? ScalarOrDefault<TSource>(this IEnumerable<TSource> source) => ScalarOrDefault(source, default(TSource));

    /// <summary>
    /// Returns the only element of a sequence, or the specified default value if the sequence is empty or contains several elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return the scalar element of.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// The only element of the input sequence, or <paramref name="defaultValue"/> if the sequence is empty or contains several elements.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static TSource ScalarOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
    {
        ArgumentNullException.ThrowIfNull(source);

        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            // The sequence is empty.
            return defaultValue;
        }

        var result = enumerator.Current;

        if (enumerator.MoveNext())
        {
            // The sequence contains several elements.
            return defaultValue;
        }

        return result;
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition,
    /// or <see langword="default"/> value when no such element exists or more than one element satisfies the condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return the scalar element of.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <returns>
    /// The only element of the input sequence that satisfies a specified condition,
    /// or <see langword="default"/> value when no such element exists or more than one element satisfies the condition.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static TSource? ScalarOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => ScalarOrDefault(source, predicate!, default);

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition,
    /// or the specified default value when no such element exists or more than one element satisfies the condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return the scalar element of.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// The only element of the input sequence that satisfies a specified condition,
    /// or <paramref name="defaultValue"/> when no such element exists or more than one element satisfies the condition.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static TSource ScalarOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        Optional<TSource> result = default;

        foreach (var element in source)
        {
            if (predicate(element))
            {
                if (result.HasValue)
                {
                    // More than one element satisfies the condition.
                    return defaultValue;
                }
                else
                {
                    result = element;
                }
            }
        }

        return result.GetValueOrDefault(defaultValue);
    }
}
