using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gapotchenko.FX.Linq
{
    partial class EnumerableEx
    {
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

            if (first is IReadOnlyList<TFirst> firstList &&
                second is IReadOnlyList<TSecond> secondList)
            {
                return SequenceEqualCore(firstList, secondList, predicate);
            }

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

            return SequenceEqualCore(first, second, predicate);
        }

        static bool SequenceEqualCore<TFirst, TSecond>(this IReadOnlyList<TFirst> first, IReadOnlyList<TSecond> second, Func<TFirst, TSecond, bool> predicate)
        {
            int count = first.Count;
            if (second.Count != count)
                return false;

            for (int i = 0; i < count; ++i)
                if (!predicate(first[i], second[i]))
                    return false;

            return true;
        }
    }
}
