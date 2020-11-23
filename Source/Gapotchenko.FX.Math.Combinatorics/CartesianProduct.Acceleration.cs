using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class CartesianProduct
    {
        static Enumerable<T> MultiplyAccelerated<T>(IEnumerable<IEnumerable<T>?> factors) => new Enumerable<T>(factors);
    }
}
