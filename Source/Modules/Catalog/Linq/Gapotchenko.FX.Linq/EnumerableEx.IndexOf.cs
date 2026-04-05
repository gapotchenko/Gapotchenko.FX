namespace Gapotchenko.FX.Linq;

partial class EnumerableEx
{
    /// <summary>
    /// Searches for the specified value and returns the index of the first occurrence within the entire sequence
    /// by using the default equality comparer to compare values.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">The value to locate in sequence.</param>
    /// <returns>The index of the first occurrence of value within the entire sequence, if found; otherwise, -1.</returns>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource value) => IndexOf(source, value, null);

    /// <summary>
    /// Searches for the specified value and returns the index of the first occurrence within the entire sequence
    /// by using a specified <see cref="IEqualityComparer{T}"/> to compare values.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">The value to locate in sequence.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>The index of the first occurrence of value within the entire sequence, if found; otherwise, -1.</returns>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);

        comparer ??= EqualityComparer<TSource>.Default;

        using var enumerator = source.GetEnumerator();

        int index = 0;
        checked
        {
            while (enumerator.MoveNext())
            {
                if (comparer.Equals(enumerator.Current, value))
                    return index;
                ++index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Searches for the specified value and returns the index of the first occurrence within the entire sequence
    /// by using the default equality comparer to compare values.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">The value to locate in sequence.</param>
    /// <returns>The index of the first occurrence of value within the entire sequence, if found; otherwise, -1.</returns>
    public static long LongIndexOf<TSource>(this IEnumerable<TSource> source, TSource value) => LongIndexOf(source, value, null);

    /// <summary>
    /// Searches for the specified value and returns the index of the first occurrence within the entire sequence
    /// by using a specified <see cref="IEqualityComparer{T}"/> to compare values.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">The value to locate in sequence.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>The index of the first occurrence of value within the entire sequence, if found; otherwise, -1.</returns>
    public static long LongIndexOf<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);

        comparer ??= EqualityComparer<TSource>.Default;

        using var enumerator = source.GetEnumerator();

        long index = 0;
        checked
        {
            while (enumerator.MoveNext())
            {
                if (comparer.Equals(enumerator.Current, value))
                    return index;
                ++index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Searches for the value that satisfies a condition and returns the index of the first occurrence within the entire sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <returns>The index of the first element that satisfies a condition; otherwise, -1.</returns>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        using var enumerator = source.GetEnumerator();

        int index = 0;
        checked
        {
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                    return index;
                ++index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Searches for the value that satisfies a condition and returns the index of the first occurrence within the entire sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="predicate">A function to test an element for a condition.</param>
    /// <returns>The index of the first element that satisfies a condition; otherwise, -1.</returns>
    public static long LongIndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        using var enumerator = source.GetEnumerator();

        long index = 0;
        checked
        {
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                    return index;
                ++index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Searches for the specified value and returns the index of the first occurrence within the entire sequence
    /// by using a specified <see cref="IEqualityComparer{T}"/> to compare sequence elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">The value to locate in the source sequence.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>The index of the first occurrence of value within the entire sequence, if found; otherwise, -1.</returns>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value, IEqualityComparer<TSource>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(value);

        if (ReferenceEquals(source, value))
            return 0;

        value = value.Memoize();
        using var e2 = value.GetEnumerator();
        if (!e2.MoveNext())
            return 0;

        comparer ??= EqualityComparer<TSource>.Default;

        int index = 0;
        int matchWindowStart = -1;
        var window = new List<(TSource Element, int Index)>();
        int windowPosition = 0;

        checked
        {
            using var e1 = source.GetEnumerator();

            while (true)
            {
                TSource current;
                int currentIndex;

                if (windowPosition < window.Count)
                {
                    (current, currentIndex) = window[windowPosition];
                }
                else if (e1.MoveNext())
                {
                    current = e1.Current;
                    currentIndex = index++;
                    window.Add((current, currentIndex));
                }
                else
                {
                    break;
                }
                windowPosition++;

                if (comparer.Equals(current, e2.Current))
                {
                    if (matchWindowStart == -1)
                        matchWindowStart = windowPosition - 1;

                    if (!e2.MoveNext())
                        return window[matchWindowStart].Index;
                }
                else if (matchWindowStart != -1)
                {
                    windowPosition = matchWindowStart + 1;
                    matchWindowStart = -1;

                    e2.Reset();
                    e2.MoveNext();
                }
                else if (windowPosition == window.Count)
                {
                    window.Clear();
                    windowPosition = 0;
                }
            }

            return -1;
        }
    }

    /// <summary>
    /// Searches for the specified value and returns the index of the first occurrence within the entire sequence
    /// by using a specified <see cref="IEqualityComparer{T}"/> to compare sequence elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">The value to locate in the source sequence.</param>
    /// <returns>The index of the first occurrence of value within the entire sequence, if found; otherwise, -1.</returns>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value) => IndexOf(source, value, null);

    /// <summary>
    /// Searches for the specified value and returns the index of the first occurrence within the entire sequence
    /// by using a specified <see cref="IEqualityComparer{T}"/> to compare sequence elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">The value to locate in the source sequence.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>The index of the first occurrence of value within the entire sequence, if found; otherwise, -1.</returns>
    public static long LongIndexOf<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value, IEqualityComparer<TSource>? comparer)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(value);

        if (ReferenceEquals(source, value))
            return 0;

        value = value.Memoize();
        using var e2 = value.GetEnumerator();
        if (!e2.MoveNext())
            return 0;

        comparer ??= EqualityComparer<TSource>.Default;

        long index = 0;
        int matchWindowStart = -1;
        var window = new List<(TSource Element, long Index)>();
        int windowPosition = 0;

        checked
        {
            using var e1 = source.GetEnumerator();

            while (true)
            {
                TSource current;
                long currentIndex;

                if (windowPosition < window.Count)
                {
                    (current, currentIndex) = window[windowPosition];
                }
                else if (e1.MoveNext())
                {
                    current = e1.Current;
                    currentIndex = index++;
                    window.Add((current, currentIndex));
                }
                else
                {
                    break;
                }
                windowPosition++;

                if (comparer.Equals(current, e2.Current))
                {
                    if (matchWindowStart == -1)
                        matchWindowStart = windowPosition - 1;

                    if (!e2.MoveNext())
                        return window[matchWindowStart].Index;
                }
                else if (matchWindowStart != -1)
                {
                    windowPosition = matchWindowStart + 1;
                    matchWindowStart = -1;

                    e2.Reset();
                    e2.MoveNext();
                }
                else if (windowPosition == window.Count)
                {
                    window.Clear();
                    windowPosition = 0;
                }
            }

            return -1;
        }
    }

    /// <summary>
    /// Searches for the specified value and returns the index of the first occurrence within the entire sequence
    /// by using a specified <see cref="IEqualityComparer{T}"/> to compare sequence elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">The value to locate in the source sequence.</param>
    /// <returns>The index of the first occurrence of value within the entire sequence, if found; otherwise, -1.</returns>
    public static long LongIndexOf<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value) => LongIndexOf(source, value, null);
}
