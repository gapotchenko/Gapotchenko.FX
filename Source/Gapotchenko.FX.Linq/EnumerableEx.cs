using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

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
    /// <para>
    /// Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
    /// <returns>A <see cref="HashSet{T}"/> that contains values of type <typeparamref name="TSource"/> retrieved from the <paramref name="source"/>.</returns>
#if TFF_ENUMERABLE_TOHASHSET
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashSet<TSource> ToHashSet<TSource>(IEnumerable<TSource> source) => Enumerable.ToHashSet(source);
#else
    public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => ToHashSet(source, null);
#endif

    /// <summary>
    /// <para>
    /// Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/> using the <paramref name="comparer"/> to compare keys.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>A <see cref="HashSet{T}"/> that contains values of type <typeparamref name="TSource"/> retrieved from the <paramref name="source"/>.</returns>
#if TFF_ENUMERABLE_TOHASHSET
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static HashSet<TSource> ToHashSet<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer) => Enumerable.ToHashSet(source, comparer);
#else
    public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return new HashSet<TSource>(source, comparer);
    }
#endif

    /// <summary>
    /// Returns distinct elements from a sequence by using the default equality comparer on the keys extracted by a specified selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence to remove duplicate elements from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
#if TFF_ENUMERABLE_DISTINCTBY
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => Enumerable.DistinctBy(source, keySelector);
#else
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => source.DistinctBy(keySelector, null);
#endif

    /// <summary>
    /// Returns distinct elements from a sequence by using a specified <see cref="IEqualityComparer{T}"/> on the keys extracted by a specified selector function.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    /// <param name="source">The sequence to remove duplicate elements from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
#if TFF_ENUMERABLE_DISTINCTBY
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer) => Enumerable.DistinctBy(source, keySelector, comparer);
#else
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        return source.Distinct(new SelectedEqualityComparer<TSource, TKey>(keySelector, comparer));
    }
#endif

    /// <summary>
    /// Determines whether a sequence contains any elements and all of them satisfy a specified condition.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> that contains the elements to apply the predicate to.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>
    /// <c>true</c> if the source sequence is not empty and every element passes the test in the specified predicate;
    /// otherwise, <c>false</c>.
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
    /// <c>true</c> if the <paramref name="source"/> sequence contains the <paramref name="value"/> sequence; otherwise, <c>false</c>.
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
    /// <c>true</c> if the <paramref name="source"/> sequence contains the <paramref name="value"/> sequence; otherwise, <c>false</c>.
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
    /// <c>true</c> if <paramref name="value"/> sequence matches the beginning of the <paramref name="source"/> sequence; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="value"/> is <c>null</c>.</exception>
    public static bool StartsWith<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value) => StartsWith(source, value, null);

    /// <summary>
    /// Determines whether the beginning of a sequence matches the specified value by using a specified equality comparer.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="value">An <see cref="IEnumerable{T}"/> value to match.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to use to compare elements.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> sequence matches the beginning of the <paramref name="source"/> sequence; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="value"/> is <c>null</c>.</exception>
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
    [return: NotNullIfNotNull("source")]
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
    [return: NotNullIfNotNull("source")]
    public static TSource[]? AsArray<TSource>(IEnumerable<TSource>? source) =>
        source switch
        {
            null => null,
            TSource[] array => array,
            _ => source.ToArray()
        };

    /// <summary>
    /// <para>
    /// Returns a read-only list view of a source sequence.
    /// </para>
    /// <para>
    /// Depending on a source sequence type, the result is either a read-only wrapper, a directly-casted instance, or a copied in-memory list.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A read-only view of a source sequence.</returns>
    [return: NotNullIfNotNull("source")]
    public static IReadOnlyList<TSource>? AsReadOnlyList<TSource>(this IEnumerable<TSource>? source) =>
        source switch
        {
            null => null,
            IReadOnlyList<TSource> readOnlyList => readOnlyList,
            IList<TSource> list => new ReadOnlyCollection<TSource>(list),
            string s => (IReadOnlyList<TSource>)(object)new ReadOnlyCharList(s),
            _ => AsList(source).AsReadOnly()
        };

    /// <summary>
    /// <para>
    /// Returns a read-only list view of a source sequence.
    /// </para>
    /// <para>
    /// Depending on a source sequence type, the result is either a read-only wrapper, a directly-casted instance, or a copied in-memory list.
    /// </para>
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A read-only view of a source sequence.</returns>
    [Obsolete("Use AsReadOnlyList method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull("source")]
    public static IReadOnlyList<TSource>? AsReadOnly<TSource>(this IEnumerable<TSource>? source) => AsReadOnlyList(source);

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
