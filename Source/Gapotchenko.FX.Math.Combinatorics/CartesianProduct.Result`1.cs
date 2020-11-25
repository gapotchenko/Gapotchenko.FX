using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class CartesianProduct
    {
        /// <summary>
        /// Represents the Cartesian product result.
        /// Exposes accelerated LINQ operations and the enumerator for the rows.
        /// </summary>
        /// <typeparam name="T">The type of elements that the row contains.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public interface IResult<T> : IEnumerable<IRow<T>>
        {
        }

        sealed class Result<T> : IResult<T>
        {
            internal Result(IEnumerable<IEnumerable<T>> factors)
            {
                m_Factors = factors;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IEnumerable<IEnumerable<T>> m_Factors;

            IEnumerable<IRow<T>> Enumerate() => Multiply(m_Factors);

            public IEnumerator<IRow<T>> GetEnumerator() => Enumerate().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        internal static IResult<T> MultiplyAccelerated<T>(IEnumerable<IEnumerable<T>> factors) => new Result<T>(factors);
    }
}
