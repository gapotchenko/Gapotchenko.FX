namespace Gapotchenko.FX.Collections.Generic;

/// <summary>
/// Provides extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class EnumerableExtensions
{
    /// <summary>
    /// Creates an <see cref="AssociativeArray{TKey, TValue}"/> from an <see cref="IEnumerable{T}"/>
    /// according to a specified key selector function and key comparer.
    /// </summary>
    /// <remarks>
    /// In contrast to <see cref="Dictionary{TKey, TValue}"/>, <see cref="AssociativeArray{TKey, TValue}"/> supports keys with <see langword="null"/> values.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey"> The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to create an <see cref="AssociativeArray{TKey, TValue}"/> from.</param>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>An <see cref="AssociativeArray{TKey, TValue}"/> that contains keys and values.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source"/> or <paramref name="keySelector"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="keySelector"/> produces duplicate keys for two elements.
    /// </exception>
    public static AssociativeArray<TKey, TSource> ToAssociativeArray<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null) =>
        ToAssociativeArray(source, keySelector, Fn.Identity, comparer);

    /// <summary>
    /// Creates an <see cref="AssociativeArray{TKey, TValue}"/> from an <see cref="IEnumerable{T}"/>
    /// according to a specified key selector function, a comparer, and an element selector function.
    /// </summary>
    /// <remarks>
    /// In contrast to <see cref="Dictionary{TKey, TValue}"/>, <see cref="AssociativeArray{TKey, TValue}"/> supports keys with <see langword="null"/> values.
    /// </remarks>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey"> The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to create an <see cref="AssociativeArray{TKey, TValue}"/> from.</param>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="AssociativeArray{TKey, TValue}"/>
    /// that contains values of type <typeparamref name="TElement"/>
    /// selected from the input sequence.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source"/> or <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="keySelector"/> produces duplicate keys for two elements.
    /// </exception>
    public static AssociativeArray<TKey, TElement> ToAssociativeArray<TSource, TKey, TElement>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(elementSelector);

        var associativeArray = new AssociativeArray<TKey, TElement>(comparer);
        foreach (var item in source)
            associativeArray.Add(keySelector(item), elementSelector(item));

        return associativeArray;
    }
}
