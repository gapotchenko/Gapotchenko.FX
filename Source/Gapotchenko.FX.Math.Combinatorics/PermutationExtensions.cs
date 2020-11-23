using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gapotchenko.FX.Math.Combinatorics
{
    /// <summary>
    /// Permutation LINQ extensions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PermutationExtensions
    {
        /// <summary>
        /// Returns all possible permutations of elements from a source sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>All possible permutations of elements from the source sequence.</returns>
        public static Permutations.Enumerable<TSource> Permute<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Permutations.PermuteAccelerated(source);
        }
    }
}
