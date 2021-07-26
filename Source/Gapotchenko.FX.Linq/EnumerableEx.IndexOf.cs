using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Linq
{
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
            if (source == null)
                throw new ArgumentNullException(nameof(source));

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
            if (source == null)
                throw new ArgumentNullException(nameof(source));

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
            checked
            {
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
    }
}
