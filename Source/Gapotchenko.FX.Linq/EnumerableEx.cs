using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Gapotchenko.FX.Linq
{
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
        /// Returns the only element of a sequence, or a default value if the sequence is empty or contains several elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to return the scalar element of.</param>
        /// <returns>
        /// The only element of the input sequence, or default value if the sequence is empty or contains several elements.
        /// </returns>
        public static TSource? ScalarOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                // The sequence is empty.
                return default;
            }

            var scalar = enumerator.Current;

            if (enumerator.MoveNext())
            {
                // The sequence contains several elements.
                return default;
            }

            return scalar;
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition,
        /// or default value when no such element exists or more than one element satisfies the condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to return the scalar element of.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <returns>
        /// The only element of the input sequence that satisfies a specified condition,
        /// or default value when no such element exists or more than one element satisfies the condition.
        /// </returns>
        public static TSource? ScalarOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            Optional<TSource> scalar = default;

            foreach (var element in source)
            {
                if (predicate(element))
                {
                    if (scalar.HasValue)
                    {
                        // More than one element satisfies the condition.
                        return default;
                    }
                    else
                    {
                        scalar = element;
                    }
                }
            }

            return scalar.GetValueOrDefault();
        }

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
        /// Determines whether two sequences are equal by comparing their elements with a specified <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{T}"/> to compare to the second sequence.</param>
        /// <param name="second">An <see cref="IEnumerable{T}"/> to compare to the first sequence.</param>
        /// <param name="predicate">The equality predicate for sequence elements.</param>
        /// <returns>
        /// <c>true</c> if both input sequences are of equal length and
        /// their corresponding elements are equal according to a specified <paramref name="predicate"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>, <paramref name="second"/> or <paramref name="predicate"/> is null.</exception>
        public static bool SequenceEqual<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> predicate)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            using var e1 = first.GetEnumerator();
            using var e2 = second.GetEnumerator();

            while (e1.MoveNext())
            {
                if (!e2.MoveNext())
                {
                    // The second sequence is shorter than the first.
                    return false;
                }

                if (!predicate(e1.Current, e2.Current))
                    return false;
            }

            if (e2.MoveNext())
            {
                // The second sequence is longer than the first.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether two lists are equal by comparing their elements with a specified <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first input list.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input list.</typeparam>
        /// <param name="first">An <see cref="IList{T}"/> to compare to the second list.</param>
        /// <param name="second">An <see cref="IList{T}"/> to compare to the first list.</param>
        /// <param name="predicate">The equality predicate for sequence elements.</param>
        /// <returns>
        /// <c>true</c> if both input lists are of equal length and
        /// their corresponding elements are equal according to a specified <paramref name="predicate"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>, <paramref name="second"/> or <paramref name="predicate"/> is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool SequenceEqual<TFirst, TSecond>(this IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, Func<TFirst, TSecond, bool> predicate)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            int count = first.Count;
            if (second.Count != count)
                return false;

            for (int i = 0; i < count; ++i)
                if (!predicate(first[i], second[i]))
                    return false;

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

            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

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
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            using var enumerator = source.GetEnumerator();

            int index = 0;
            while (enumerator.MoveNext())
            {
                if (comparer.Equals(enumerator.Current, value))
                    return index;
                ++index;
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
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            using var enumerator = source.GetEnumerator();

            int index = 0;
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                    return index;
                ++index;
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
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (ReferenceEquals(source, value))
                return 0;

            comparer ??= EqualityComparer<TSource>.Default;

            value = value.Memoize();
            int match = 0;

            using var e2 = value.GetEnumerator();

            int index = 0;
            foreach (var e1Current in source)
            {
                if (!e2.MoveNext())
                    return match;

                if (comparer.Equals(e1Current, e2.Current))
                {
                    if (match == -1)
                        match = index;
                }
                else
                {
                    match = -1;
                    e2.Reset();
                }

                ++index;
            }

            if (match != -1)
            {
                if (!e2.MoveNext())
                    return match;
                else
                    return -1;
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
        /// <returns>The index of the first occurrence of value within the entire sequence, if found; otherwise, -1.</returns>
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value) => IndexOf(source, value, null);

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
        /// Returns a list view of a source sequence.
        /// Depending on a source sequence kind, the result is either a directly casted instance or a copied in-memory list.
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
        /// Returns an array view of a source sequence.
        /// Depending on a source sequence kind, the result is either a directly casted instance or a copied in-memory array.
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
        /// Returns a read-only view of a source sequence.
        /// </para>
        /// <para>
        /// Depending on a source sequence kind, the result is either a read-only wrapper, a directly casted instance or a copied in-memory list.
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>A read-only view of a source sequence.</returns>
        [return: NotNullIfNotNull("source")]
        public static IReadOnlyList<TSource>? AsReadOnly<TSource>(this IEnumerable<TSource>? source) =>
            source switch
            {
                null => null,
                IReadOnlyList<TSource> readOnlyList => readOnlyList,
                IList<TSource> list => new ReadOnlyCollection<TSource>(list),
                string s => (IReadOnlyList<TSource>)(object)new ReadOnlyCharList(s),
                _ => AsList(source).AsReadOnly()
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

        /// <summary>
        /// <para>
        /// Returns a new enumerable collection that contains the last count elements from source.
        /// </para>
        /// <para>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the enumerable collection.</typeparam>
        /// <param name="source">An enumerable collection instance.</param>
        /// <param name="count">The number of elements to take from the end of the collection.</param>
        /// <returns>A new enumerable collection that contains the last count elements from source.</returns>
#if TFF_ENUMERABLE_TAKELAST
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<TSource> TakeLast<TSource>(IEnumerable<TSource> source, int count) => Enumerable.TakeLast(source, count);
#else
        public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
                throw new ArgumentException(nameof(source));

            static IEnumerable<TSource> Iterator(IEnumerable<TSource> source, int count)
            {
                Queue<TSource> queue;

                using (var e = source.GetEnumerator())
                {
                    if (!e.MoveNext())
                        yield break;

                    queue = new Queue<TSource>();
                    queue.Enqueue(e.Current);

                    while (e.MoveNext())
                    {
                        if (queue.Count < count)
                        {
                            // Fill the queue.
                            queue.Enqueue(e.Current);
                        }
                        else
                        {
                            // Swap old queue elements with the new ones.
                            do
                            {
                                queue.Dequeue();
                                queue.Enqueue(e.Current);
                            }
                            while (e.MoveNext());

                            break;
                        }
                    }
                }

                // Flush the queue.
                do
                {
                    yield return queue.Dequeue();
                }
                while (queue.Count > 0);
            }

            return count <= 0 ?
                Enumerable.Empty<TSource>() :
                Iterator(source, count);
        }
#endif

        /// <summary>
        /// <para>
        /// Returns a new enumerable collection that contains the elements from source
        /// with the last count elements of the source collection omitted.
        /// </para>
        /// <para>
        /// This is a polyfill provided by Gapotchenko.FX.
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the enumerable collection.</typeparam>
        /// <param name="source">An enumerable collection instance.</param>
        /// <param name="count">The number of elements to omit from the end of the collection.</param>
        /// <returns>A new enumerable collection that contains the elements from source minus count elements from the end of the collection.</returns>
#if TFF_ENUMERABLE_SKIPLAST
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<TSource> SkipLast<TSource>(IEnumerable<TSource> source, int count) => Enumerable.SkipLast(source, count);
#else
        public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
                throw new ArgumentException(nameof(source));

            static IEnumerable<TSource> Iterator(IEnumerable<TSource> source, int count)
            {
                var queue = new Queue<TSource>();
                using var e = source.GetEnumerator();

                while (e.MoveNext())
                {
                    if (queue.Count == count)
                    {
                        do
                        {
                            yield return queue.Dequeue();
                            queue.Enqueue(e.Current);
                        }
                        while (e.MoveNext());

                        break;
                    }
                    else
                    {
                        queue.Enqueue(e.Current);
                    }
                }
            }

            return count <= 0 ?
                source.Skip(0) :
                Iterator(source, count);
        }
#endif
    }
}
