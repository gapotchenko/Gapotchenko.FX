using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
        public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
        {
#if NETCOREAPP
            return Enumerable.Append(source, element);
#else
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var i in source)
                yield return i;

            yield return element;
#endif
        }

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
        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
#if NETCOREAPP
            return Enumerable.Prepend(source, element);
#else
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            yield return element;

            foreach (var i in source)
                yield return i;
#endif
        }

        /// <summary>
        /// Returns the only element of a sequence, or a default value if the sequence is empty or contains several elements.
        /// </summary>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to return the scalar element of.</param>
        /// <returns>
        /// The only element of the input sequence, or default value if the sequence is empty or contains several elements.
        /// </returns>
        public static TSource ScalarOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using (var enumerator = source.GetEnumerator())
            {
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
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition,
        /// or default value when no such element exists or more than one element satisfies the condition.
        /// </summary>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to return the scalar element of.</param>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <returns>
        /// The only element of the input sequence that satisfies a specified condition,
        /// or default value when no such element exists or more than one element satisfies the condition.
        /// </returns>
        public static TSource ScalarOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
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
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => source.DistinctBy(keySelector, null);

        /// <summary>
        /// Returns distinct elements from a sequence by using a specified <see cref="IEqualityComparer{T}"/> on the keys extracted by a specified selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            return source.Distinct(new KeyedEqualityComparer<TSource, TKey>(keySelector, comparer));
        }

        /// <summary>
        /// Checks whether the number of elements in a sequence is greater or equal to a specified <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <param name="value">The value to compare the count of elements in a sequence with.</param>
        /// <returns><c>true</c> if the number of elements in a sequence is greater or equal to a specified <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        public static bool CountIsAtLeast<TSource>(this IEnumerable<TSource> source, int value)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (value <= 0)
                return true;

            using (var enumerator = source.GetEnumerator())
            {
                int i = 0;
                while (enumerator.MoveNext())
                {
                    ++i;
                    if (i >= value)
                        return true;
                }
            }

            return false;
        }

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

            using (var enumerator = source.GetEnumerator())
            {
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
            }

            return true;
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing their elements by using a specified <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first">An <see cref="IEnumerable{T}"/> to compare to second.</param>
        /// <param name="second">An <see cref="IEnumerable{T}"/> to compare to the first sequence.</param>
        /// <param name="predicate">The equality predicate for sequence elements.</param>
        /// <returns>
        /// <c>true</c> if the two source sequences are of equal length and
        /// their corresponding elements are equal according to a specified <paramref name="predicate"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>, <paramref name="second"/> or <paramref name="predicate"/> is null.</exception>
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> predicate)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (!e2.MoveNext())
                        return false;
                    if (!predicate(e1.Current, e2.Current))
                        return false;
                }
                if (e2.MoveNext())
                    return false;
            }

            return true;
        }

#if TFF_READONLY_LIST
        /// <summary>
        /// Determines whether two lists are equal by comparing their elements by using a specified <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first">An <see cref="IList{T}"/> to compare to the second list.</param>
        /// <param name="second">An <see cref="IList{T}"/> to compare to the first list.</param>
        /// <param name="predicate">The equality predicate for sequence elements.</param>
        /// <returns>
        /// <c>true</c> if the two source lists are of equal length and
        /// their corresponding elements are equal according to a specified <paramref name="predicate"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/>, <paramref name="second"/> or <paramref name="predicate"/> is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool SequenceEqual<TSource>(this IReadOnlyList<TSource> first, IReadOnlyList<TSource> second, Func<TSource, TSource, bool> predicate)
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
#endif

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
        public static bool Contains<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value, IEqualityComparer<TSource> comparer)
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
            bool eqRegion = true;

            using (var e2 = value.GetEnumerator())
            {
                foreach (var e1Current in source)
                {
                    if (!e2.MoveNext())
                        return eqRegion;

                    if (comparer.Equals(e1Current, e2.Current))
                    {
                        eqRegion = true;
                    }
                    else
                    {
                        eqRegion = false;
                        e2.Reset();
                    }
                }

                if (eqRegion)
                    return !e2.MoveNext();
            }

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
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            using (var enumerator = source.GetEnumerator())
            {
                int index = 0;
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
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            using (var enumerator = source.GetEnumerator())
            {
                int index = 0;
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
        public static bool StartsWith<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> value, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (ReferenceEquals(source, value))
                return true;

            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            using (var e1 = source.GetEnumerator())
            using (var e2 = value.GetEnumerator())
            {
                while (e2.MoveNext())
                {
                    if (!(e1.MoveNext() && comparer.Equals(e1.Current, e2.Current)))
                        return false;
                }
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
        public static List<TSource> AsList<TSource>(IEnumerable<TSource> source)
        {
            switch (source)
            {
                case null:
                    return null;
                case List<TSource> list:
                    return list;
                default:
                    return source.ToList();
            }
        }

        /// <summary>
        /// Returns an array view of a source sequence.
        /// Depending on a source sequence kind, the result is either a directly casted instance or a copied in-memory array.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>An array view of a source sequence.</returns>
        public static TSource[] AsArray<TSource>(IEnumerable<TSource> source)
        {
            switch (source)
            {
                case null:
                    return null;
                case TSource[] array:
                    return array;
                default:
                    return source.ToArray();
            }
        }

        /// <summary>
        /// Returns a read-only view of a source sequence.
        /// Depending on a source sequence kind, the result is either a read-only wrapper, a directly casted instance or a copied in-memory list.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>A read-only view of a source sequence.</returns>
        public static IReadOnlyList<TSource> AsReadOnly<TSource>(IEnumerable<TSource> source)
        {
            switch (source)
            {
                case null:
                    return null;
                case IList<TSource> list:
                    return new ReadOnlyCollection<TSource>(list);
                case IReadOnlyList<TSource> readOnlyList:
                    return readOnlyList;
                case string s:
                    return (IReadOnlyList<TSource>)(object)new ReadOnlyCharList(s);
                default:
#if TFF_READONLY_COLLECTION
                    return AsList(source).AsReadOnly();
#else
                    return new ReadOnlyCollection<TSource>(AsList(source));
#endif
            }
        }
    }
}
