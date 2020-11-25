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
        /// Represents the result of permutations.
        /// Exposes accelerated LINQ operations and the enumerator for the rows.
        /// </summary>
        /// <typeparam name="T">The type of elements that the row contains.</typeparam>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public interface IResult<T> : IReadOnlyCollection<IRow<T>>
        {
            /// <summary>
            /// <para>
            /// Returns the number of permutations in a sequence.
            /// </para>
            /// <para>
            /// This is an accelerated LINQ operation provided by the algorithm kernel to automatically reduce the computational complexity.
            /// </para>
            /// </summary>
            /// <returns>The number of permutations in the sequence.</returns>
            new int Count();

            /// <summary>
            /// <para>
            /// Returns a <see cref="long"/> that represents the total number of permutations in a sequence.
            /// </para>
            /// <para>
            /// This is an accelerated LINQ operation provided by the algorithm kernel to automatically reduce the computational complexity.
            /// </para>
            /// </summary>
            /// <returns>The number of permutations in the sequence.</returns>
            long LongCount();

            /// <summary>
            /// <para>
            /// Returns distinct elements from a sequence of permutations by using the default equality comparer to compare values.
            /// </para>
            /// <para>
            /// This is an accelerated LINQ operation provided by the algorithm kernel to automatically reduce the computational complexity.
            /// </para>
            /// </summary>
            /// <returns>An <see cref="IResult{T}"/> that contains distinct elements from the source sequence of permutations.</returns>
            IResult<T> Distinct();

            /// <summary>
            /// <para>
            /// Returns distinct elements from a sequence of permutations by using a specified <see cref="IEqualityComparer{T}"/> to compare values.
            /// </para>
            /// <para>
            /// This is an accelerated LINQ operation provided by the algorithm kernel to automatically reduce the computational complexity.
            /// </para>
            /// </summary>
            /// <returns>An <see cref="IResult{T}"/> that contains distinct elements from the source sequence of permutations.</returns>
            IResult<T> Distinct(IEqualityComparer<T>? comparer);
        }

        sealed class Result<T> : IResult<T>
        {
            internal Result(IEnumerable<T> source, IEqualityComparer<T>? comparer)
            {
                m_Source = source;
                m_Comparer = comparer;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IEnumerable<T> m_Source;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly IEqualityComparer<T>? m_Comparer;

            IEnumerable<IRow<T>> Enumerate() => Permute(m_Source, m_Comparer != null, m_Comparer);

            public IEnumerator<IRow<T>> GetEnumerator() => Enumerate().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            object? m_SyncLock;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            Optional<int> m_CachedCount;

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

            int IReadOnlyCollection<IRow<T>>.Count => Count();

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            Optional<long> m_CachedLongCount;

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

            public IResult<T> Distinct() => Distinct(null);

            public IResult<T> Distinct(IEqualityComparer<T>? comparer)
            {
                if (Utility.IsSet(m_Source))
                {
                    // The permutations are already distinct.
                    return this;
                }

                comparer ??= EqualityComparer<T>.Default;

                if (m_Comparer == null)
                    return new Result<T>(m_Source, comparer);
                else if (ReferenceEquals(comparer, m_Comparer))
                    return this;
                else
                    throw new NotSupportedException("Cannot produce distinct permutations by using different comparers.");
            }
        }

        internal static IResult<T> PermuteAccelerated<T>(IEnumerable<T> sequence)
        {
            if (!Utility.IsSet(sequence))
                sequence = sequence.AsReadOnly();

            return new Result<T>(sequence, null);
        }
    }
}
