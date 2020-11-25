﻿using System;
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
        public interface IRow<T> : IReadOnlyCollection<T>, IEquatable<IRow<T>>
        {
        }

        sealed class Row<T> : IRow<T>
        {
            internal Row(IReadOnlyList<T> source)
            {
                m_Source = source;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IReadOnlyList<T> m_Source;

            public int Count => m_Source.Count;

            public IEnumerator<T> GetEnumerator() => m_Source.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public bool Equals(IRow<T>? other) =>
                ReferenceEquals(this, other) ||
                other is Row<T> otherRow &&
                ReferenceEquals(m_Source, otherRow.m_Source) &&
                this.SequenceEqual(otherRow);

            public override bool Equals(object? obj) =>
                obj switch
                {
                    Row<T> other => Equals(other),
                    _ => false
                };

            public override int GetHashCode() =>
                HashCode.Combine(
                    m_Source.GetHashCode(),
                    HashCodeEx.SequenceCombine(this));
        }
    }
}
