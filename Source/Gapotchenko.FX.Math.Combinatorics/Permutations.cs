using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Combinatorics
{
    /// <summary>
    /// Defines permutation operations.
    /// </summary>
    public static partial class Permutations
    {
        /// <summary>
        /// Returns all possible permutations of elements from a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements of sequence.</typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns>An enumerable that contains all possible permutations of elements from the sequence.</returns>
        public static Enumerable<T> Of<T>(IEnumerable<T> sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            return PermuteAccelerated(sequence);
        }
    }
}
