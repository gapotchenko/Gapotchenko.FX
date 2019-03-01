using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Represents a pair of values.
    /// </summary>
    public static class Pair
    {
        /// <summary>
        /// Creates a new pair of values.
        /// </summary>
        /// <typeparam name="TFirst">The type of the first value.</typeparam>
        /// <typeparam name="TSecond">The type of the second value.</typeparam>
        /// <param name="first">The first value.</param>
        /// <param name="second">The second value.</param>
        /// <returns>A new pair of values.</returns>
        public static Pair<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second) =>
            new Pair<TFirst, TSecond>(first, second);
    }
}
