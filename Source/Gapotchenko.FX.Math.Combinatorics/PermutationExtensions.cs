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
        /// Generates permutations from the source sequence.
        /// </summary>
        /// <typeparam name="T">Type of items to permute.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>Permutations of elements from the source sequence.</returns>
        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> source) => Permutations.Generate(source);

        /// <summary>
        /// Generates permutations from the source sequence.
        /// </summary>
        /// <typeparam name="T">Type of items to permute.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="comparer">
        /// Optional comparer to use.
        /// If supplied <paramref name="comparer"/> is not <c>null</c>
        /// then permutations are ordered according to the <paramref name="comparer"/>.
        /// </param>
        /// <returns>Permutations of elements from the source sequence.</returns>
        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> source, IComparer<T>? comparer) => Permutations.Generate(source, comparer);
    }
}
