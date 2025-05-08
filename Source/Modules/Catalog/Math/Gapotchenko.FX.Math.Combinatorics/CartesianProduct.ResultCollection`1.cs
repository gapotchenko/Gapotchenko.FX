// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Collections;
using System.Diagnostics;

namespace Gapotchenko.FX.Math.Combinatorics;

partial class CartesianProduct
{
    /// <summary>
    /// A collection of rows representing a sequence of Cartesian product results.
    /// </summary>
    /// <typeparam name="T">The type of elements that the row contains.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IResultCollection<T> : IEnumerable<IResultRow<T>>
    {
        /// <summary>
        /// <para>
        /// Returns distinct elements from a sequence of Cartesian product results by using the default equality comparer to compare values.
        /// </para>
        /// <para>
        /// This is an accelerated LINQ operation provided by the algorithm kernel to automatically reduce the computational complexity.
        /// </para>
        /// </summary>
        /// <returns>An <see cref="IResultCollection{T}"/> that contains distinct elements from the source sequence of Cartesian product results.</returns>
        IResultCollection<T> Distinct();

        /// <summary>
        /// <para>
        /// Returns distinct elements from a sequence of Cartesian product results by using a specified <see cref="IEqualityComparer{T}"/> to compare values.
        /// </para>
        /// <para>
        /// This is an accelerated LINQ operation provided by the algorithm kernel to automatically reduce the computational complexity.
        /// </para>
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <returns>An <see cref="IResultCollection{T}"/> that contains distinct elements from the source sequence of Cartesian product results.</returns>
        IResultCollection<T> Distinct(IEqualityComparer<T>? comparer);
    }

    sealed class ResultCollection<T>(ResultMode mode, IEnumerable<IEnumerable<T>> factors, IEqualityComparer<T>? comparer) :
        IResultCollection<T>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IResultRow<T>> GetEnumerator() => Multiply(m_Factors).GetEnumerator();

        public IResultCollection<T> Distinct() => Distinct(null);

        public IResultCollection<T> Distinct(IEqualityComparer<T>? comparer)
        {
            switch (m_Mode)
            {
                case ResultMode.Default:
                    return new ResultCollection<T>(
                        ResultMode.Distinct,
                        DistinctMultipliers(m_Factors, comparer),
                        comparer);

                case ResultMode.Distinct:
                    if (Empty.Nullify(comparer) == Empty.Nullify(m_Comparer))
                        return this;
                    else
                        throw new NotSupportedException("Cannot produce distinct Cartesian product results by using different comparers.");

                default:
                    throw new InvalidOperationException();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly ResultMode m_Mode = mode;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEnumerable<IEnumerable<T>> m_Factors = factors;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<T>? m_Comparer = comparer;
    }

    internal static IResultCollection<T> MultiplyAccelerated<T>(IEnumerable<IEnumerable<T>> factors) => new ResultCollection<T>(ResultMode.Default, factors, null);
}
