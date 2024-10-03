// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Threading;
using System.Collections;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Combinatorics;

partial class Permutations
{
    /// <summary>
    /// <para>
    /// Represents the result of permutations.
    /// </para>
    /// <para>
    /// Exposes accelerated LINQ operations and the enumerator for the rows.
    /// </para>
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
        /// <param name="comparer">The comparer.</param>
        /// <returns>An <see cref="IResult{T}"/> that contains distinct elements from the source sequence of permutations.</returns>
        IResult<T> Distinct(IEqualityComparer<T>? comparer);
    }

    sealed class Result<T>(ResultMode mode, IEnumerable<T> source, IEqualityComparer<T>? comparer) : IResult<T>
    {
        int IReadOnlyCollection<IRow<T>>.Count => Count();

        public int Count() =>
            LazyInitializerEx.EnsureInitialized(
                ref m_CachedCount,
                ref m_SyncLock,
                () =>
                {
                    if (m_CachedLongCount.HasValue)
                        return checked((int)LongCount());

                    return
                        mode switch
                        {
                            ResultMode.Default or ResultMode.DistinctView => Cardinality(EnumerableEx.Count(source)),
                            _ => EnumerableEx.Count(Enumerate())
                        };
                });

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Optional<int> m_CachedCount;

        public long LongCount() =>
            LazyInitializerEx.EnsureInitialized(
                ref m_CachedLongCount,
                ref m_SyncLock,
                () =>
                {
                    if (m_CachedCount.HasValue)
                        return Count();

                    return
                        mode switch
                        {
                            ResultMode.Default or ResultMode.DistinctView => Cardinality(EnumerableEx.LongCount(source)),
                            _ => EnumerableEx.LongCount(Enumerate())
                        };
                });

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Optional<long> m_CachedLongCount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object? m_SyncLock;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IRow<T>> GetEnumerator() => Enumerate().GetEnumerator();

        IEnumerable<IRow<T>> Enumerate() => Permute(source, mode == ResultMode.Distinct, m_Comparer);

        public IResult<T> Distinct() => Distinct(null);

        public IResult<T> Distinct(IEqualityComparer<T>? comparer)
        {
            switch (mode)
            {
                case ResultMode.Default:
                    if (Utility.IsCompatibleSet(source, comparer))
                    {
                        // The permutations are already distinct.
                        return this;
                    }
                    else
                    {
                        return new Result<T>(ResultMode.Distinct, source, comparer);
                    }

                case ResultMode.Distinct:
                case ResultMode.DistinctView:
                    if (Utility.IsCompatibleComparer(comparer, m_Comparer))
                        return this;
                    else
                        throw new NotSupportedException("Cannot produce distinct permutations by using different comparers.");

                default:
                    throw new InvalidOperationException();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<T>? m_Comparer = comparer;
    }

    internal static IResult<T> PermuteAccelerated<T>(IEnumerable<T> sequence)
    {
        if (!Utility.IsSet(sequence))
            sequence = sequence.ReifyList();

        return new Result<T>(ResultMode.Default, sequence, null);
    }
}
