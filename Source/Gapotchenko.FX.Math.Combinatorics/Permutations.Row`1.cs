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
        /// Represents a row in sequence of permutation results.
        /// </summary>
        /// <typeparam name="T">The type of elements that the row contains.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public interface IRow<T> : IEnumerable<T>, IEquatable<IRow<T>>
        {
        }

        sealed class Row<T> : IRow<T>, IReadOnlyList<T>
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

            public T this[int index] => m_Source[m_Transform[index]];

            public int Count => m_Transform.Length;

            public IEnumerator<T> GetEnumerator()
            {
                for (int i = 0, n = Count; i < n; i++)
                    yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public bool Equals(IRow<T>? other) =>
                ReferenceEquals(this, other) ||
                other is Row<T> otherRow &&
                ReferenceEquals(m_Source, otherRow.m_Source) &&
                this.SequenceEqual(otherRow);

            public override bool Equals(object? obj) =>
                obj switch
                {
                    IRow<T> other => Equals(other),
                    _ => false
                };

            public override int GetHashCode() =>
                HashCode.Combine(
                    m_Source.GetHashCode(),
                    HashCodeEx.SequenceCombine(this));
        }
    }
}
