using Gapotchenko.FX.Linq;
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
        /// Returns the number of multiset permutations for a specified sequence length.
        /// </summary>
        /// <param name="length">The length of a sequence.</param>
        /// <returns>The number of multiset permutations.</returns>
        public static int Cardinality(int length) => MathEx.Factorial(length);

        /// <summary>
        /// Returns a <see cref="long"/> that represents the total number of multiset permutations for a specified sequence length.
        /// </summary>
        /// <param name="length">The length of a sequence.</param>
        /// <returns>The number of multiset permutations.</returns>
        public static long Cardinality(long length) => MathEx.Factorial(length);

        /// <summary>
        /// Generates permutations from the source sequence.
        /// </summary>
        /// <typeparam name="T">Type of items to permute.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>Permutations of elements from the source sequence.</returns>
        public static Enumerable<T> Of<T>(IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new Enumerable<T>(source.AsReadOnly(), null);
        }

        static IEnumerable<IEnumerable<T>> Permute<T>(IReadOnlyList<T> items, IComparer<T>? comparer)
        {
            int length = items.Count;
            var transform = new (int First, int Second)[length];

            if (comparer == null)
            {
                // Multiset permutation.

                // Start with an identity transform.
                for (int i = 0; i < length; i++)
                    transform[i] = (i, i);
            }
            else
            {
                // Set permutation.

                // Figure out where we are in the sequence of all permutations.
                var initialOrder = new int[length];
                for (int i = 0; i < length; i++)
                    initialOrder[i] = i;

                Array.Sort(
                    initialOrder,
                    (x, y) => comparer.Compare(items[x], items[y]));

                for (int i = 0; i < length; i++)
                    transform[i] = (initialOrder[i], i);

                // Handle duplicates.
                for (int i = 1; i < length; i++)
                {
                    if (comparer.Compare(
                        items[transform[i - 1].Second],
                        items[transform[i].Second]) == 0)
                    {
                        transform[i].First = transform[i - 1].First;
                    }
                }
            }

            static IEnumerable<T> ApplyTransform(IReadOnlyList<T> items, (int, int)[] transform)
            {
                for (int i = 0; i < transform.Length; i++)
                    yield return items[transform[i].Item2];
            }

            yield return ApplyTransform(items, transform);

            for (; ; )
            {
                // Reference: E. W. Dijkstra, A Discipline of Programming, Prentice-Hall, 1997
                // Find the largest partition from the back that is in decreasing (non-increasing) order.
                int decreasingPart;
                for (decreasingPart = length - 2;
                    decreasingPart >= 0 && transform[decreasingPart].First >= transform[decreasingPart + 1].First;
                    --decreasingPart)
                {
                }

                // The whole sequence is in decreasing order, finished.
                if (decreasingPart < 0)
                    yield break;

                // Find the smallest element in the decreasing partition that is 
                // greater than (or equal to) the item in front of the decreasing partition.
                int greater;
                for (greater = length - 1;
                    greater > decreasingPart && transform[decreasingPart].First >= transform[greater].First;
                    greater--)
                {
                }

                transform = ((int, int)[])transform.Clone();

                // Swap the two.
                MathEx.Swap(ref transform[decreasingPart], ref transform[greater]);

                // Reverse the decreasing partition.
                Array.Reverse(transform, decreasingPart + 1, length - decreasingPart - 1);

                yield return ApplyTransform(items, transform);
            }
        }
    }
}
