using Gapotchenko.FX.Threading;
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
        /// Exposes the enumerator for permutations.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed class Enumerable<T> : IEnumerable<IEnumerable<T>>
        {
            internal Enumerable(IReadOnlyList<T> items, IComparer<T>? comparer)
            {
                m_Items = items;
                m_Comparer = comparer;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IReadOnlyList<T> m_Items;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IComparer<T>? m_Comparer;

            /// <inheritdoc/>
            public IEnumerator<IEnumerable<T>> GetEnumerator() => Permute(m_Items, m_Comparer).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            object? m_SyncLock;

            Optional<int> m_CachedCount;

            /// <summary>
            /// Returns the number of permutations in a sequence.
            /// </summary>
            /// <returns>The number of permutations in the sequence.</returns>
            public int Count() =>
                LazyInitializerEx.EnsureInitialized(
                    ref m_CachedCount,
                    ref m_SyncLock,
                    () =>
                    {
                        if (m_CachedLongCount.HasValue)
                            return checked((int)LongCount());

                        if (m_Comparer == null)
                            return Cardinality(m_Items.Count);
                        else
                            return Enumerable.Count(this);
                    });

            Optional<long> m_CachedLongCount;

            /// <summary>
            /// Returns a <see cref="long"/> that represents the total number of permutations in a sequence.
            /// </summary>
            /// <returns>The number of permutations in the sequence.</returns>
            public long LongCount() =>
                LazyInitializerEx.EnsureInitialized(
                    ref m_CachedLongCount,
                    ref m_SyncLock,
                    () =>
                    {
                        if (m_CachedCount.HasValue)
                            return Count();

                        if (m_Comparer == null)
                            return Cardinality((long)m_Items.Count);
                        else
                            return Enumerable.LongCount(this);
                    });

            /// <summary>
            /// Returns distinct elements from a sequence of permutations by using the default equality comparer to compare values.
            /// </summary>
            /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence of permutations.</returns>
            public IEnumerable<IEnumerable<T>> Distinct() => Distinct(null);

            /// <summary>
            /// Returns distinct elements from a sequence of permutations by using a specified <see cref="IEqualityComparer{T}"/> to compare values.
            /// </summary>
            /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence of permutations.</returns>
            public IEnumerable<IEnumerable<T>> Distinct(IComparer<T>? comparer) =>
                new Enumerable<T>(m_Items, comparer ?? Comparer<T>.Default);
        }
    }
}
