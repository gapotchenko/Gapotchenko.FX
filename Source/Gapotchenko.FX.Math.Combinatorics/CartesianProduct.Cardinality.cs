using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class CartesianProduct
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
    }
}
