using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class CartesianProduct
    {
        /// <summary>
        /// Represents a row in sequence of Cartesian product results.
        /// </summary>
        /// <typeparam name="T">The type of elements that the row contains.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed class Row<T> : IReadOnlyCollection<T>, IEquatable<Row<T>>
        {
            internal Row(IReadOnlyList<T> source)
            {
                m_Source = source;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IReadOnlyList<T> m_Source;

            /// <inheritdoc/>
            public int Count => m_Source.Count;

            /// <inheritdoc/>
            public IEnumerator<T> GetEnumerator() => m_Source.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            /// <inheritdoc/>
            public bool Equals(Row<T>? other)
            {
                if (ReferenceEquals(this, other))
                    return true;

                if (other is null)
                    return false;

                return
                    ReferenceEquals(m_Source, other.m_Source) &&
                    this.SequenceEqual(other);
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) =>
                obj switch
                {
                    Row<T> other => Equals(other),
                    _ => false
                };

            /// <inheritdoc/>
            public override int GetHashCode() =>
                HashCode.Combine(
                    m_Source.GetHashCode(),
                    HashCodeEx.SequenceCombine(this));
        }
    }
}
