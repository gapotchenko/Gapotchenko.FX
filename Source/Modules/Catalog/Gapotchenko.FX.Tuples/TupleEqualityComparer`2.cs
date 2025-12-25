// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Tuples.Utils;

namespace Gapotchenko.FX.Tuples;

sealed class TupleEqualityComparer<T1, T2>(
    IEqualityComparer<T1>? comparer1,
    IEqualityComparer<T2>? comparer2) :
    IEqualityComparer<Tuple<T1, T2>>
{
    public bool Equals(Tuple<T1, T2>? x, Tuple<T1, T2>? y) =>
        ReferenceEquals(x, y) ||
        x is not null &&
        y is not null &&
        m_Comparer1.Equals(x.Item1, y.Item1) &&
        m_Comparer2.Equals(x.Item2, y.Item2);

    public int GetHashCode(Tuple<T1, T2> obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return HashCode.Combine(
            m_Comparer1.GetNullableHashCode(obj.Item1),
            m_Comparer2.GetNullableHashCode(obj.Item2));
    }

    readonly IEqualityComparer<T1> m_Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
    readonly IEqualityComparer<T2> m_Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
}
