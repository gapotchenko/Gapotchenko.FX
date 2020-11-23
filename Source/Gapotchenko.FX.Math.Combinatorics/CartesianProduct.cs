using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics
{
    /// <summary>
    /// Defines Cartesian product operations.
    /// </summary>
    public static partial class CartesianProduct
    {
        /// <summary>
        /// Returns a Cartesian product cardinality for specified factor lengths.
        /// </summary>
        /// <param name="factorLengths">The factor lengths.</param>
        /// <returns>The Cartesian product cardinality.</returns>
        public static int Cardinality(IEnumerable<int> factorLengths)
        {
            if (factorLengths == null)
                throw new ArgumentNullException(nameof(factorLengths));

            bool hasFactor = false;
            int cardinality = 1;

            foreach (var length in factorLengths)
            {
                if (length == 0)
                    return 0;

                cardinality *= length;
                hasFactor = true;
            }

            if (!hasFactor)
                return 0;

            return cardinality;
        }

        /// <summary>
        /// Returns a Cartesian product cardinality for specified factor lengths.
        /// </summary>
        /// <param name="factorLengths">The factor lengths.</param>
        /// <returns>The Cartesian product cardinality.</returns>
        public static int Cardinality(params int[] factorLengths) => Cardinality((IEnumerable<int>)factorLengths);

        /// <summary>
        /// Returns a Cartesian product cardinality for specified factor lengths.
        /// </summary>
        /// <param name="factorLengths">The factor lengths.</param>
        /// <returns>The Cartesian product cardinality.</returns>
        public static long Cardinality(IEnumerable<long> factorLengths)
        {
            if (factorLengths == null)
                throw new ArgumentNullException(nameof(factorLengths));

            bool hasFactor = false;
            long cardinality = 1;

            foreach (var length in factorLengths)
            {
                if (length == 0)
                    return 0;

                cardinality *= length;
                hasFactor = true;
            }

            if (!hasFactor)
                return 0;

            return cardinality;
        }

        /// <summary>
        /// Returns a Cartesian product cardinality for specified factor lengths.
        /// </summary>
        /// <param name="factorLengths">The factor lengths.</param>
        /// <returns>The Cartesian product cardinality.</returns>
        public static long Cardinality(params long[] factorLengths) => Cardinality((IEnumerable<long>)factorLengths);

        /// <summary>
        /// Calculates a Cartesian product of specified factors.
        /// </summary>
        /// <typeparam name="T">The factor type.</typeparam>
        /// <param name="factors">The factors.</param>
        /// <returns>The Cartesian product of the specified factors.</returns>
        public static Enumerable<T> Of<T>(IEnumerable<IEnumerable<T>?> factors)
        {
            if (factors == null)
                throw new ArgumentNullException(nameof(factors));

            return new Enumerable<T>(factors);
        }

        /// <summary>
        /// Calculates a Cartesian product of specified factors.
        /// </summary>
        /// <typeparam name="T">Type of factor items.</typeparam>
        /// <param name="first">The first factor.</param>
        /// <param name="second">The second factor.</param>
        /// <param name="rest">The rest of factors.</param>
        /// <returns>The Cartesian product of the specified factors.</returns>
        public static IEnumerable<IEnumerable<T>> Of<T>(IEnumerable<T>? first, IEnumerable<T>? second, params IEnumerable<T>?[] rest)
        {
            if (rest == null)
                throw new ArgumentNullException(nameof(rest));

            return Of(new[] { first, second }.Concat(rest));
        }

        static IEnumerable<Row<T>> Multiply<T>(IEnumerable<IEnumerable<T>?> factors)
        {
            IReadOnlyList<IEnumerable<T>> items =
                SelectExceptLast(
                    factors.Where(x => x != null),
                    EnumerableEx.Memoize)
                .AsReadOnly()!;

            if (items.Count == 0)
                yield break;

            var enumerators = items.Select(x => x.GetEnumerator()).ToArray();

            int n = enumerators.Length;

            foreach (var i in enumerators)
            {
                if (!i.MoveNext())
                {
                    // At least one multiplier is empty.
                    yield break;
                }
            }

            for (; ; )
            {
                var result = new T[n];
                for (int i = 0; i != n; i++)
                    result[i] = enumerators[i].Current;

                yield return new Row<T>(result);

                for (int i = 0; i != n; i++)
                {
                    var enumerator = enumerators[i];
                    if (enumerator.MoveNext())
                        break;

                    var newEnumerator = items[i].GetEnumerator();
                    if (!newEnumerator.MoveNext())
                        throw new InvalidOperationException("Cartesian product pool has been emptied unexpectedly.");

                    enumerators[i] = newEnumerator;

                    if (i == n - 1)
                        yield break;
                }
            }
        }

        static IEnumerable<T> SelectExceptLast<T>(IEnumerable<T> source, Func<T, T> selector)
        {
            Optional<T> slot = default;

            foreach (var item in source)
            {
                if (slot.HasValue)
                    yield return selector(slot.Value);

                slot = item;
            }

            // Flush the last item as is.
            if (slot.HasValue)
                yield return slot.Value;
        }
    }
}
