// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Collections;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Combinatorics;

partial class Permutations
{
    /// <summary>
    /// Represents a row in sequence of permutation results.
    /// </summary>
    /// <typeparam name="T">The type of elements that the row contains.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IResultRow<T> : IEnumerable<T>, IEquatable<IResultRow<T>>
    {
    }

    sealed class ResultRow<T>(IReadOnlyList<T> source, int[] transform) : IResultRow<T>, IReadOnlyList<T>
    {
        public T this[int index] => m_Source[transform[index]];

        public int Count => transform.Length;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0, n = Count; i < n; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(IResultRow<T>? other) =>
            ReferenceEquals(this, other) ||
            other is ResultRow<T> otherRow &&
            ReferenceEquals(m_Source, otherRow.m_Source) &&
            this.SequenceEqual(otherRow);

        public override bool Equals(object? obj) => obj is IResultRow<T> other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(
                m_Source.GetHashCode(),
                HashCodeEx.SequenceCombine(this));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<T> m_Source = source;
    }
}
