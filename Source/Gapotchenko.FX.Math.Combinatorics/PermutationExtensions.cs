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
        /// Generates permutations from a source sequence.
        /// </summary>
        /// <typeparam name="T">Type of items to permute.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>Permutations of elements from the source sequence.</returns>
        public static Permutations.Enumerable<T> Permute<T>(this IEnumerable<T> source) => Permutations.Of(source);
    }
}
