namespace Gapotchenko.FX.Linq;

/// <summary>
/// Provides an extended set of static methods for querying objects that implement <see cref="IEnumerable{T}"/>.
/// </summary>
public static partial class EnumerableEx
{
    /// <summary>
    /// <para>
    /// Appends a value to the end of the sequence.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="element">The value to append to <paramref name="source"/>.</param>
    /// <returns>A new sequence that ends with <paramref name="element"/>.</returns>
#if TFF_ENUMERABLE_APPEND
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> Append<TSource>(IEnumerable<TSource> source, TSource element) => Enumerable.Append(source, element);
#else
    public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        foreach (var i in source)
            yield return i;

        yield return element;
    }
#endif

    /// <summary>
    /// <para>
    /// Adds a value to the beginning of the sequence.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">A sequence of values.</param>
    /// <param name="element">The value to prepend to <paramref name="source"/>.</param>
    /// <returns>A new sequence that begins with <paramref name="element"/>.</returns>
#if TFF_ENUMERABLE_PREPEND
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> Prepend<TSource>(IEnumerable<TSource> source, TSource element) => Enumerable.Prepend(source, element);
#else
    public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        yield return element;

        foreach (var i in source)
            yield return i;
    }
#endif

    /// <summary>
    /// Determines whether a sequence contains any elements and all of them satisfy a specified condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> that contains the elements to apply the predicate to.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>
    /// <see langword="true"/> if the source sequence is not empty and every element passes the test in the specified predicate;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Source or predicate is null.</exception>
    public static bool AnyAndAll<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        using var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            // Sequence is empty.
            return false;
        }

        do
        {
            if (!predicate(enumerator.Current))
                return false;
        }
        while (enumerator.MoveNext());

        return true;
    }

    /// <summary>
    /// Determines whether the <paramref name="source"/> sequence contains the <paramref name="value"/> sequence
    /// by using a specified equality comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">An <see cref="IEnumerable{T}"/> to compare to the source sequence.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to use to compare elements.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="source"/> sequence contains the <paramref name="value"/> sequence; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="value"/> is null.</exception>
    public static bool Contains<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value, IEqualityComparer<TSource>? comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (ReferenceEquals(source, value))
            return true;

        comparer ??= EqualityComparer<TSource>.Default;

        value = value.Memoize();
        bool match = true;

        using var e2 = value.GetEnumerator();

        foreach (var e1Current in source)
        {
            if (!e2.MoveNext())
                return match;

            if (comparer.Equals(e1Current, e2.Current))
            {
                match = true;
            }
            else
            {
                match = false;
                e2.Reset();
            }
        }

        if (match)
            return !e2.MoveNext();

        return false;
    }

    /// <summary>
    /// Determines whether the <paramref name="source"/> sequence contains the <paramref name="value"/> sequence
    /// by using the default equality comparer to compare values.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">An <see cref="IEnumerable{T}"/> to compare to the source sequence.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="source"/> sequence contains the <paramref name="value"/> sequence; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="value"/> is null.</exception>
    public static bool Contains<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value) => Contains(source, value, null);

    /// <summary>
    /// Determines whether the beginning of a sequence matches the specified value by using the default equality comparer for elements' type.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">An <see cref="IEnumerable{T}"/> value to match.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> sequence matches the beginning of the <paramref name="source"/> sequence; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool StartsWith<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value) => StartsWith(source, value, null);

    /// <summary>
    /// Determines whether the beginning of a sequence matches the specified value by using a specified equality comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">An <see cref="IEnumerable{T}"/> value to match.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to use to compare elements.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> sequence matches the beginning of the <paramref name="source"/> sequence; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool StartsWith<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value, IEqualityComparer<TSource>? comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (ReferenceEquals(source, value))
            return true;

        comparer ??= EqualityComparer<TSource>.Default;

        using var e1 = source.GetEnumerator();
        using var e2 = value.GetEnumerator();

        while (e2.MoveNext())
        {
            if (!(e1.MoveNext() && comparer.Equals(e1.Current, e2.Current)))
                return false;
        }

        return true;
    }

    /// <summary>
    /// <para>
    /// Returns a list view of a source sequence.
    /// </para>
    /// <para>
    /// Depending on a source sequence type, the result is either a directly-casted instance or a copied in-memory list.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A list view of a source sequence.</returns>
    [return: NotNullIfNotNull(nameof(source))]
    public static List<TSource>? AsList<TSource>(IEnumerable<TSource>? source) =>
        source switch
        {
            null => null,
            List<TSource> list => list,
            _ => source.ToList()
        };

    /// <summary>
    /// <para>
    /// Returns an array view of a source sequence.
    /// </para>
    /// <para>
    /// Depending on a source sequence type, the result is either a directly-casted instance or a copied in-memory array.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>An array view of a source sequence.</returns>
    [return: NotNullIfNotNull(nameof(source))]
    public static TSource[]? AsArray<TSource>(IEnumerable<TSource>? source) =>
        source switch
        {
            null => null,
            TSource[] array => array,
            _ => source.ToArray()
        };

    /// <summary>
    /// Determines whether any elements of a sequence satisfy the specified conditions.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to apply the predicates to.</param>
    /// <param name="predicate1">The first function to test each element for a condition.</param>
    /// <param name="predicate2">The second function to test each element for a condition.</param>
    /// <returns>A value tuple whose items signify whether any elements in the source sequence pass the test in the corresponding predicate.</returns>
    public static ValueTuple<bool, bool> Any<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate1,
        Func<TSource, bool> predicate2)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (predicate1 == null)
            throw new ArgumentNullException(nameof(predicate1));
        if (predicate2 == null)
            throw new ArgumentNullException(nameof(predicate2));

        bool result1 = false;
        bool result2 = false;

        foreach (var i in source)
        {
            if (!result1 && predicate1(i))
            {
                result1 = true;
                if (result2)
                    break;
            }

            if (!result2 && predicate2(i))
            {
                result2 = true;
                if (result1)
                    break;
            }
        }

        return (result1, result2);
    }

    /// <summary>
    /// Determines whether any elements of a sequence satisfy the specified conditions.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to apply the predicates to.</param>
    /// <param name="predicate1">The first function to test each element for a condition.</param>
    /// <param name="predicate2">The second function to test each element for a condition.</param>
    /// <param name="predicate3">The third function to test each element for a condition.</param>
    /// <returns>A value tuple whose items signify whether any elements in the source sequence pass the test in the corresponding predicate.</returns>
    public static ValueTuple<bool, bool, bool> Any<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, bool> predicate1,
        Func<TSource, bool> predicate2,
        Func<TSource, bool> predicate3)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (predicate1 == null)
            throw new ArgumentNullException(nameof(predicate1));
        if (predicate2 == null)
            throw new ArgumentNullException(nameof(predicate2));
        if (predicate3 == null)
            throw new ArgumentNullException(nameof(predicate3));

        bool result1 = false;
        bool result2 = false;
        bool result3 = false;

        foreach (var i in source)
        {
            if (!result1 && predicate1(i))
            {
                result1 = true;
                if (result2 && result3)
                    break;
            }

            if (!result2 && predicate2(i))
            {
                result2 = true;
                if (result1 && result3)
                    break;
            }

            if (!result3 && predicate3(i))
            {
                result3 = true;
                if (result1 && result2)
                    break;
            }
        }

        return (result1, result2, result3);
    }

}
