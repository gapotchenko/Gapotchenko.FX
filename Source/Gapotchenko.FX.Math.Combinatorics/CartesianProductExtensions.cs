using System.Collections.Generic;
using System.ComponentModel;

namespace Gapotchenko.FX.Math.Combinatorics
{
    /// <summary>
    /// Cartesian product LINQ extensions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CartesianProductExtensions
    {
        /// <summary>
        /// Generates Cartesian product of source sequence elements.
        /// </summary>
        /// <typeparam name="T">Type of source sequence elements.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>Cartesian product of source sequence elements.</returns>
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> source) =>
            Combinatorics.CartesianProduct.Generate(source);
    }
}
