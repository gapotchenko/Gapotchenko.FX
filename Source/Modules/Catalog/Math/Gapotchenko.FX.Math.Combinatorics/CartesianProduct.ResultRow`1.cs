// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Math.Combinatorics;

partial class CartesianProduct
{
    /// <summary>
    /// Represents a row in a sequence of Cartesian product results.
    /// </summary>
    /// <typeparam name="T">The type of elements that the row contains.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IResultRow<T> :
        IEnumerable<T>,
#if TFF_ITUPLE
        ITuple,
#endif
        IEquatable<IResultRow<T>>
    {
    }

    sealed class ResultRow<T>(IReadOnlyList<T> source) : IResultRow<T>, IReadOnlyList<T>
    {
        public int Count => m_Source.Count;

        public T this[int index] => m_Source[index];

#if TFF_ITUPLE

        int ITuple.Length => Count;

        object? ITuple.this[int index] => this[index];

#endif

        public IEnumerator<T> GetEnumerator() => m_Source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(IResultRow<T>? other)
        {
            return
                ReferenceEquals(this, other) ||
                other is IResultRow<T> otherRow &&
                this.SequenceEqual(otherRow);
        }

        public override bool Equals(object? obj) =>
            obj is IResultRow<T> other &&
            Equals(other);

        public override int GetHashCode()
        {
            return HashCodeEx.SequenceCombine(this);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<T> m_Source = source;
    }
}
