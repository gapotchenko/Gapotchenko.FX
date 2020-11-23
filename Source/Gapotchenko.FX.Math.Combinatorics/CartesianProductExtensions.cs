using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics
{
    /// <summary>
    /// Cartesian product LINQ extensions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CartesianProductExtensions
    {
        /// <summary>
        /// Returns a Cartesian product of factor sequences.
        /// </summary>
        /// <typeparam name="T">Type of factor sequence elements.</typeparam>
        /// <param name="factors">The factor sequences.</param>
        /// <returns>The Cartesian product of the factor sequences.</returns>
        public static CartesianProduct.Enumerable<T> CrossJoin<T>(this IEnumerable<IEnumerable<T>> factors)
        {
            if (factors == null)
                throw new ArgumentNullException(nameof(factors));

            return CartesianProduct.MultiplyAccelerated(
                factors.Select(x => (x ?? throw new ArgumentNullException(nameof(factors), "A Cartesian product factor cannot be null."))));
        }

        /// <summary>
        /// Returns a Cartesian product of two sequences by enumerating all possible combinations of sequence elements,
        /// and applying a user-defined projection to each combination.
        /// </summary>
        /// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements of resulting sequence.</typeparam>
        /// <param name="first">The first input sequence of elements.</param>
        /// <param name="second">The second input sequence of elements.</param>
        /// <param name="resultSelector">The projection function that maps a combination of input sequence elements to an element of a resulting sequence.</param>
        /// <returns>The Cartesian product of two input sequences with user-defined projection applied.</returns>
        public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return CartesianProduct.Multiply(first, second, resultSelector);
        }

        /// <summary>
        /// Returns a Cartesian product of two sequences by enumerating all possible combinations of sequence elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="first">The first input sequence of elements.</param>
        /// <param name="second">The second input sequence of elements.</param>
        /// <returns>The Cartesian product of two input sequences.</returns>
        public static CartesianProduct.Enumerable<TSource> CrossJoin<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            return CartesianProduct.MultiplyAccelerated(new[] { first, second });
        }
    }
}
