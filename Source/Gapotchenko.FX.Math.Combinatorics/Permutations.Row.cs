using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class Permutations
    {
        /// <summary>
        /// Represents a row in sequence of permutations.
        /// </summary>
        /// <typeparam name="T">The type of elements that the row contains.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed class Row<T> : IReadOnlyList<T>, IEquatable<Row<T>>
        {
            internal Row(IReadOnlyList<T> source, int[] transform)
            {
                m_Source = source;
                m_Transform = transform;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IReadOnlyList<T> m_Source;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly int[] m_Transform;

            /// <inheritdoc/>
            public T this[int index] => m_Source[m_Transform[index]];

            /// <inheritdoc/>
            public int Count => m_Transform.Length;

            /// <inheritdoc/>
            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0, n = Count; i < n; i++)
                    yield return this[i];
            }

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
