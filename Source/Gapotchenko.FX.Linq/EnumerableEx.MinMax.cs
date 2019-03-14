using Gapotchenko.FX.Linq.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Linq
{
    partial class EnumerableEx
    {
        static TSource _MinMaxCore<TSource>(IEnumerable<TSource> source, IComparer<TSource> comparer, bool isMax, bool throwWhenEmpty)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (comparer == null)
                comparer = Comparer<TSource>.Default;

            Optional<TSource> value = default;

            foreach (var candidateValue in source)
            {
                if (value.HasValue)
                {
                    if (_IsMatch(candidateValue, value.Value, isMax, comparer))
                        value = candidateValue;
                }
                else
                {
                    value = candidateValue;
                }
            }

            if (throwWhenEmpty && !value.HasValue)
                throw new InvalidOperationException(Resources.NoElements);

            return value.GetValueOrDefault();
        }

        static bool _IsMatch<T>(T candidateValue, T value, bool isMax, IComparer<T> comparer)
        {
            int d = comparer.Compare(candidateValue, value);
            bool match = isMax ? d > 0 : d < 0;
            return match;
        }

        /// <summary>
        /// Returns the minimum value in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The minimum value in the sequence.</returns>
        public static TSource Min<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer) => _MinMaxCore(source, comparer, false, true);

        /// <summary>
        /// Returns the maximum value in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The maximum value in the sequence.</returns>
        public static TSource Max<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer) => _MinMaxCore(source, comparer, true, true);

        /// <summary>
        /// Returns the minimum value in a sequence, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The minimum value in the sequence, or a default value if the sequence is empty.</returns>
        public static TSource MinOrDefault<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer) => _MinMaxCore(source, comparer, false, false);

        /// <summary>
        /// Returns the maximum value in a sequence, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The maximum value in the sequence, or a default value if the sequence is empty.</returns>
        public static TSource MaxOrDefault<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer) => _MinMaxCore(source, comparer, true, false);

        static TSource _MinMaxCore<TSource, TKey>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            bool isMax,
            bool throwWhenEmpty)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (comparer == null)
                comparer = Comparer<TKey>.Default;

            Optional<TSource> value = default;
            Optional<TKey> key = default;

            foreach (var candidateValue in source)
            {
                if (value.HasValue)
                {
                    if (!key.HasValue)
                        key = keySelector(value.Value);

                    var candidateKey = keySelector(candidateValue);
                    if (_IsMatch(candidateKey, key.Value, isMax, comparer))
                    {
                        value = candidateValue;
                        key = candidateKey;
                    }
                }
                else
                {
                    value = candidateValue;
                }
            }

            if (throwWhenEmpty && !value.HasValue)
                throw new InvalidOperationException(Resources.NoElements);

            return value.GetValueOrDefault();
        }

        /// <summary>
        /// Returns the minimum value in a sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">The key comparer.</param>
        /// <returns>The minimum value in the sequence according to a specified key selector function.</returns>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
            _MinMaxCore(source, keySelector, comparer, false, true);

        /// <summary>
        /// Returns the minimum value in a sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>The minimum value in the sequence according to a specified key selector function.</returns>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MinBy(source, keySelector, null);

        /// <summary>
        /// Returns the maximum value in a sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">The key comparer.</param>
        /// <returns>The maximum value in the sequence according to a specified key selector function.</returns>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
            _MinMaxCore(source, keySelector, comparer, true, true);

        /// <summary>
        /// Returns the maximum value in a sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>The maximum value in the sequence according to a specified key selector function.</returns>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MaxBy(source, keySelector, null);

        /// <summary>
        /// Returns the minimum value in a sequence according to a specified key selector function, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">The key comparer.</param>
        /// <returns>The minimum value in the sequence according to a specified key selector function, or a default value if the sequence is empty.</returns>
        public static TSource MinOrDefaultBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
            _MinMaxCore(source, keySelector, comparer, false, false);

        /// <summary>
        /// Returns the minimum value in a sequence according to a specified key selector function, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>The minimum value in the sequence according to a specified key selector function, or a default value if the sequence is empty.</returns>
        public static TSource MinOrDefaultBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MinOrDefaultBy(source, keySelector, null);

        /// <summary>
        /// Returns the maximum value in a sequence according to a specified key selector function, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">The key comparer.</param>
        /// <returns>The maximum value in the sequence according to a specified key selector function, or a default value if the sequence is empty.</returns>
        public static TSource MaxOrDefaultBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer) =>
            _MinMaxCore(source, keySelector, comparer, true, false);

        /// <summary>
        /// Returns the maximum value in a sequence according to a specified key selector function, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>The maximum value in the sequence according to a specified key selector function, or a default value if the sequence is empty.</returns>
        public static TSource MaxOrDefaultBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MaxOrDefaultBy(source, keySelector, null);
    }
}
