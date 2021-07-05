using Gapotchenko.FX.Linq.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gapotchenko.FX.Linq
{
    partial class EnumerableEx
    {
        /// <summary>
        /// Returns the minimum value in a sequence by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The minimum value in the sequence.</returns>
#if TFF_ENUMERABLE_MIN_COMPARER
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TSource? Min<TSource>(IEnumerable<TSource> source, IComparer<TSource>? comparer) => Enumerable.Min(source, comparer);
#else
        public static TSource? Min<TSource>(this IEnumerable<TSource> source, IComparer<TSource>? comparer) => MinBy(source, Fn.Identity, comparer);
#endif

        /// <summary>
        /// Returns the maximum value in a sequence by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The maximum value in the sequence.</returns>
#if TFF_ENUMERABLE_MAX_COMPARER
        public static TSource? Max<TSource>(IEnumerable<TSource> source, IComparer<TSource>? comparer) => Enumerable.Max(source, comparer);
#else
        public static TSource? Max<TSource>(this IEnumerable<TSource> source, IComparer<TSource>? comparer) => MaxBy(source, Fn.Identity, comparer);
#endif

        /// <summary>
        /// Returns the minimum value in a sequence, or a default value if the sequence is empty by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The minimum value in the sequence, or a default value if the sequence is empty.</returns>
        public static TSource? MinOrDefault<TSource>(this IEnumerable<TSource> source, IComparer<TSource>? comparer) => MinOrDefaultBy(source, Fn.Identity, comparer);

        /// <summary>
        /// Returns the minimum value in a sequence, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence, or a default value if the sequence is empty.</returns>
        public static TSource? MinOrDefault<TSource>(this IEnumerable<TSource> source) => MinOrDefault(source, comparer: null);

        /// <summary>
        /// Returns the maximum value in a sequence, or a default value if the sequence is empty by using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The maximum value in the sequence, or a default value if the sequence is empty.</returns>
        public static TSource? MaxOrDefault<TSource>(this IEnumerable<TSource> source, IComparer<TSource>? comparer) => MaxOrDefaultBy(source, Fn.Identity, comparer);

        /// <summary>
        /// Returns the maximum value in a sequence, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence, or a default value if the sequence is empty.</returns>
        public static TSource? MaxOrDefault<TSource>(this IEnumerable<TSource> source) => MaxOrDefault(source, comparer: null);

        static TSource? _MinMaxCore<TSource, TKey>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey>? comparer,
            bool isMax,
            Optional<TSource?> defaultValue)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            using var e = source.GetEnumerator();

            bool referenceType = default(TKey) == null;

            if (!e.MoveNext())
            {
                if (defaultValue.HasValue)
                    return defaultValue.Value;
                else if (referenceType)
                    return default;
                else
                    throw new InvalidOperationException(Resources.NoElements);
            }

            var value = e.Current;

            if (e.MoveNext())
            {
                comparer ??= Comparer<TKey>.Default;
                var key = keySelector(value);

                do
                {
                    var candidateValue = e.Current;
                    var candidateKey = keySelector(candidateValue);

                    if (referenceType && candidateKey == null)
                        continue;

                    static bool _IsMatch<T>(T candidateValue, T value, bool isMax, IComparer<T> comparer)
                    {
                        int d = comparer.Compare(candidateValue, value);
                        bool match = isMax ? d > 0 : d < 0;
                        return match;
                    }

                    if (_IsMatch(candidateKey, key, isMax, comparer))
                    {
                        value = candidateValue;
                        key = candidateKey;
                    }
                }
                while (e.MoveNext());
            }

            return value;
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
        public static TSource? MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer) =>
            _MinMaxCore(source, keySelector, comparer, false, Optional<TSource?>.None);

        /// <summary>
        /// Returns the minimum value in a sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>The minimum value in the sequence according to a specified key selector function.</returns>
        public static TSource? MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MinBy(source, keySelector, null);

        /// <summary>
        /// Returns the maximum value in a sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">The key comparer.</param>
        /// <returns>The maximum value in the sequence according to a specified key selector function.</returns>
        public static TSource? MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer) =>
            _MinMaxCore(source, keySelector, comparer, true, Optional<TSource?>.None);

        /// <summary>
        /// Returns the maximum value in a sequence according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>The maximum value in the sequence according to a specified key selector function.</returns>
        public static TSource? MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MaxBy(source, keySelector, null);

        /// <summary>
        /// Returns the minimum value in a sequence according to a specified key selector function, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">The key comparer.</param>
        /// <returns>The minimum value in the sequence according to a specified key selector function, or a default value if the sequence is empty.</returns>
        public static TSource? MinOrDefaultBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer) =>
            _MinMaxCore(source, keySelector, comparer, false, Optional.Some(default(TSource)));

        /// <summary>
        /// Returns the minimum value in a sequence according to a specified key selector function, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>The minimum value in the sequence according to a specified key selector function, or a default value if the sequence is empty.</returns>
        public static TSource? MinOrDefaultBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MinOrDefaultBy(source, keySelector, comparer: null);

        /// <summary>
        /// Returns the maximum value in a sequence according to a specified key selector function, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="comparer">The key comparer.</param>
        /// <returns>The maximum value in the sequence according to a specified key selector function, or a default value if the sequence is empty.</returns>
        public static TSource? MaxOrDefaultBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer) =>
            _MinMaxCore(source, keySelector, comparer, true, Optional.Some(default(TSource)));

        /// <summary>
        /// Returns the maximum value in a sequence according to a specified key selector function, or a default value if the sequence is empty.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum value of.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>The maximum value in the sequence according to a specified key selector function, or a default value if the sequence is empty.</returns>
        public static TSource? MaxOrDefaultBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) => MaxOrDefaultBy(source, keySelector, comparer: null);
    }
}
