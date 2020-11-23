using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class CartesianProduct
    {
        /// <summary>
        /// Exposes the enumerator for Cartesian product results.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed class Enumerable<T> : IEnumerable<Row<T>>
        {
            internal Enumerable(IEnumerable<IEnumerable<T>?> factors)
            {
                m_Factors = factors;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IEnumerable<IEnumerable<T>?> m_Factors;

            IEnumerable<Row<T>> Enumerate() => Multiply(m_Factors);

            /// <inheritdoc/>
            public IEnumerator<Row<T>> GetEnumerator() => Enumerate().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
