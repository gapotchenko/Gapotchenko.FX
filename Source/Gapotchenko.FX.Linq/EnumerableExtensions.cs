using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Gapotchenko.FX.Linq
{
    /// <summary>
    /// Enumerable extensions.
    /// </summary>
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// <para>
        /// Appends a value to the end of the sequence.
        /// </para>
        /// <para>
        /// This is a polyfill provided by Gapotchenko.FX.Linq.
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
        /// This is a polyfill provided by Gapotchenko.FX.Linq.
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
        /// Returns distinct elements from a sequence by using a specified <see cref="IEqualityComparer{T}"/> comparer on the keys extracted by a specified selector function.
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
            return source.Distinct(new SelectedKeyEqualityComparer<TSource, TKey>(keySelector, comparer));
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
        /// Determines whether a sequence contains any elements and all of them satisfy a condition.
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
        /// Determines whether the <paramref name="first"/> sequence contains the <paramref name="second"/> sequence
        /// by using a specified equality comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{T}"/> to compare to second.</param>
        /// <param name="second">An <see cref="IEnumerable{T}"/> to compare to the first sequence.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to use to compare elements.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="first"/> sequence contains the <paramref name="second"/> sequence; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
        public static bool Contains<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            if (ReferenceEquals(first, second))
                return true;

            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            second = second.Memoize();
            bool eqRegion = true;

            using (var e2 = second.GetEnumerator())
            {
                foreach (var e1Current in first)
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
        /// Determines whether the <paramref name="first"/> sequence contains the <paramref name="second"/> sequence
        /// by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="IEnumerable{T}"/> to compare to second.</param>
        /// <param name="second">An <see cref="IEnumerable{T}"/> to compare to the first sequence.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="first"/> sequence contains the <paramref name="second"/> sequence; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
        public static bool Contains<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) => Contains(first, second, null);

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
    }
}
