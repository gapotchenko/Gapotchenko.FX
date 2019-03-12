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
    }
}
