using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Linq
{
    partial class EnumerableEx
    {
        /// <summary>
        /// <para>
        /// Returns the number of elements in a sequence.
        /// </para>
        /// <para>
        /// In contrast to <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>,
        /// this method provides optimized coverage of enumerable types that have a known count value such as
        /// arrays, lists, dictionaries, sets and others.
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <returns>The number of elements in the input sequence.</returns>
        public static int Count<TSource>(IEnumerable<TSource> source) => TryGetOptimizedCount(source) ?? Enumerable.Count(source);

        /// <summary>
        /// <para>
        /// Returns a <see cref="long"/> that represents the total number of elements in a sequence.
        /// </para>
        /// <para>
        /// In contrast to <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>,
        /// this method provides optimized coverage of enumerable types that have a known count value such as
        /// arrays, lists, dictionaries, sets and others.
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <returns>The number of elements in the source sequence.</returns>
        public static long LongCount<TSource>(IEnumerable<TSource> source) => TryGetOptimizedCount(source) ?? Enumerable.LongCount(source);

        static int? TryGetOptimizedCount<TSource>(IEnumerable<TSource>? source) =>
            source switch
            {
                IReadOnlyCollection<TSource> roc => roc.Count,
                ICollection<TSource> c => c.Count,
                _ => null
            };

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

            var optimizedCount = TryGetOptimizedCount(source);
            if (optimizedCount.HasValue)
                return optimizedCount.Value >= value;

            using var enumerator = source.GetEnumerator();

            int i = 0;
            while (enumerator.MoveNext())
            {
                checked { ++i; }
                if (i >= value)
                    return true;
            }

            return false;
        }
    }
}
