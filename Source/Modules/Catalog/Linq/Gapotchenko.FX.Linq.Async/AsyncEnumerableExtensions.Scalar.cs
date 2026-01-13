// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Linq;

partial class AsyncEnumerableExtensions
{
    /// <summary>
    /// Returns the only element of a sequence, or a <see langword="default"/> value if the sequence is empty or contains several elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IAsyncEnumerable{T}"/> to return the scalar element of.</param>
    /// <param name="cancellationToken">The cancelation token.</param>
    /// <returns>
    /// The only element of the input sequence, or <see langword="default"/> value if the sequence is empty or contains several elements.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static ValueTask<TSource?> ScalarOrDefaultAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        CancellationToken cancellationToken = default) =>
        ScalarOrDefaultAsync(source, default(TSource), cancellationToken);

    /// <summary>
    /// Returns the only element of a sequence, or the specified default value if the sequence is empty or contains several elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return the scalar element of.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancelation token.</param>
    /// <returns>
    /// The only element of the input sequence, or <paramref name="defaultValue"/> if the sequence is empty or contains several elements.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static async ValueTask<TSource> ScalarOrDefaultAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        TSource defaultValue,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        await using var enumerator = source.GetAsyncEnumerator(cancellationToken);

        if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            // The sequence is empty.
            return defaultValue;
        }

        var result = enumerator.Current;

        if (await enumerator.MoveNextAsync().ConfigureAwait(false))
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
    /// <param name="source">An <see cref="IAsyncEnumerable{T}"/> to return the scalar element of.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <param name="cancellationToken">The cancelation token.</param>
    /// <returns>
    /// The only element of the input sequence that satisfies a specified condition,
    /// or <see langword="default"/> value when no such element exists or more than one element satisfies the condition.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static ValueTask<TSource?> ScalarOrDefaultAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        CancellationToken cancellationToken = default) =>
        ScalarOrDefaultAsync(source, predicate!, default, cancellationToken);

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition,
    /// or the specified default value when no such element exists or more than one element satisfies the condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IAsyncEnumerable{T}"/> to return the scalar element of.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="cancellationToken">The cancelation token.</param>
    /// <returns>
    /// The only element of the input sequence that satisfies a specified condition,
    /// or <paramref name="defaultValue"/> when no such element exists or more than one element satisfies the condition.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
    public static async ValueTask<TSource> ScalarOrDefaultAsync<TSource>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        TSource defaultValue,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        Optional<TSource> result = default;

        await foreach (var element in source.WithCancellation(cancellationToken).ConfigureAwait(false))
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
