using System;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class Permutations
    {
        /// <summary>
        /// Returns the number of permutations for a multiset of a specified length.
        /// </summary>
        /// <param name="length">The length of a multiset.</param>
        /// <returns>The number of permutations.</returns>
        public static int Cardinality(int length) => MathEx.Factorial(length);

        /// <summary>
        /// Returns a <see cref="long"/> that represents the total number of permutations for a multiset of a specified length.
        /// </summary>
        /// <param name="length">The length of a sequence.</param>
        /// <returns>The number of permutations.</returns>
        public static long Cardinality(long length) => MathEx.Factorial(length);
    }
}
