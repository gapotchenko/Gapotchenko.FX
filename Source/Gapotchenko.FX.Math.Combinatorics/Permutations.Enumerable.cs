using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class Permutations
    {
        /// <summary>
        /// Exposes the enumerator for permutations.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed class Enumerable<T> : IReadOnlyCollection<Row<T>>
        {
            internal Enumerable(IEnumerable<T> source, IComparer<T>? comparer)
            {
                m_Source = source;
                m_Comparer = comparer;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IEnumerable<T> m_Source;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IComparer<T>? m_Comparer;

            IEnumerable<Row<T>> Enumerate() => Permute(m_Source, m_Comparer);

            /// <inheritdoc/>
            public IEnumerator<Row<T>> GetEnumerator() => Enumerate().GetEnumerator();

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
                            return Cardinality(EnumerableEx.Count(m_Source));
                        else
                            return EnumerableEx.Count(Enumerate());
                    });

            int IReadOnlyCollection<Row<T>>.Count => Count();

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
                            return Cardinality(EnumerableEx.LongCount(m_Source));
                        else
                            return EnumerableEx.LongCount(Enumerate());
                    });

            /// <summary>
            /// Returns distinct elements from a sequence of permutations by using the default equality comparer to compare values.
            /// </summary>
            /// <returns>An <see cref="Enumerable{T}"/> that contains distinct elements from the source sequence of permutations.</returns>
            public Enumerable<T> Distinct() => Distinct(null);

            /// <summary>
            /// Returns distinct elements from a sequence of permutations by using a specified <see cref="IEqualityComparer{T}"/> to compare values.
            /// </summary>
            /// <returns>An <see cref="Enumerable{T}"/> that contains distinct elements from the source sequence of permutations.</returns>
            public Enumerable<T> Distinct(IComparer<T>? comparer)
            {
                if (IsSet(m_Source))
                {
                    // The permutations are already distinct.
                    return this;
                }

                comparer ??= Comparer<T>.Default;

                if (m_Comparer == null)
                    return new Enumerable<T>(m_Source, comparer);
                else if (ReferenceEquals(comparer, m_Comparer))
                    return this;
                else
                    throw new NotSupportedException("Cannot produce distinct permutations by using different comparers.");
            }
        }
    }
}
