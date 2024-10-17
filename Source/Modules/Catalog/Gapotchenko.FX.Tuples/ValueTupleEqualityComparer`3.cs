// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Tuples.Utils;

namespace Gapotchenko.FX.Tuples;

sealed class ValueTupleEqualityComparer<T1, T2, T3>(
    IEqualityComparer<T1>? comparer1,
    IEqualityComparer<T2>? comparer2,
    IEqualityComparer<T3>? comparer3) :
    IEqualityComparer<(T1, T2, T3)>
{
    public bool Equals((T1, T2, T3) x, (T1, T2, T3) y) =>
        m_Comparer1.Equals(x.Item1, y.Item1) &&
        m_Comparer2.Equals(x.Item2, y.Item2) &&
        m_Comparer3.Equals(x.Item3, y.Item3);

    public int GetHashCode((T1, T2, T3) obj) =>
        HashCode.Combine(
            m_Comparer1.GetNullSafeHashCode(obj.Item1),
            m_Comparer2.GetNullSafeHashCode(obj.Item2),
            m_Comparer3.GetNullSafeHashCode(obj.Item3));

    readonly IEqualityComparer<T1> m_Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
    readonly IEqualityComparer<T2> m_Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
    readonly IEqualityComparer<T3> m_Comparer3 = comparer3 ?? EqualityComparer<T3>.Default;
}
