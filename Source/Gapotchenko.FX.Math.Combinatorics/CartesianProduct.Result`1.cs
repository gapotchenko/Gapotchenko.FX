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
    /// <para>
    /// Represents the Cartesian product result.
    /// </para>
    /// <para>
    /// Exposes accelerated LINQ operations and the enumerator for the rows.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of elements that the row contains.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IResult<T> : IEnumerable<IRow<T>>
    {
        /// <summary>
        /// <para>
        /// Returns distinct elements from a sequence of Cartesian product results by using the default equality comparer to compare values.
        /// </para>
        /// <para>
        /// This is an accelerated LINQ operation provided by the algorithm kernel to automatically reduce the computational complexity.
        /// </para>
        /// </summary>
        /// <returns>An <see cref="IResult{T}"/> that contains distinct elements from the source sequence of Cartesian product results.</returns>
        IResult<T> Distinct();

        /// <summary>
        /// <para>
        /// Returns distinct elements from a sequence of Cartesian product results by using a specified <see cref="IEqualityComparer{T}"/> to compare values.
        /// </para>
        /// <para>
        /// This is an accelerated LINQ operation provided by the algorithm kernel to automatically reduce the computational complexity.
        /// </para>
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <returns>An <see cref="IResult{T}"/> that contains distinct elements from the source sequence of Cartesian product results.</returns>
        IResult<T> Distinct(IEqualityComparer<T>? comparer);
    }

    sealed class Result<T> : IResult<T>
    {
        public Result(ResultMode mode, IEnumerable<IEnumerable<T>> factors, IEqualityComparer<T>? comparer)
        {
            m_Mode = mode;
            m_Factors = factors;
            m_Comparer = comparer;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly ResultMode m_Mode;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEnumerable<IEnumerable<T>> m_Factors;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IEqualityComparer<T>? m_Comparer;

        IEnumerable<IRow<T>> Enumerate() => Multiply(m_Factors);

        public IEnumerator<IRow<T>> GetEnumerator() => Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IResult<T> Distinct() => Distinct(null);

        public IResult<T> Distinct(IEqualityComparer<T>? comparer)
        {
            switch (m_Mode)
            {
                case ResultMode.Default:
                    return new Result<T>(
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
    }

    internal static IResult<T> MultiplyAccelerated<T>(IEnumerable<IEnumerable<T>> factors) => new Result<T>(ResultMode.Default, factors, null);
}
