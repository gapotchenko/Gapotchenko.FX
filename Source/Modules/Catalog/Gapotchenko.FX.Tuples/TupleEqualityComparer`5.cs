﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Tuples.Utils;

namespace Gapotchenko.FX.Tuples;

sealed class TupleEqualityComparer<T1, T2, T3, T4, T5>(
    IEqualityComparer<T1>? comparer1,
    IEqualityComparer<T2>? comparer2,
    IEqualityComparer<T3>? comparer3,
    IEqualityComparer<T4>? comparer4,
    IEqualityComparer<T5>? comparer5) :
    IEqualityComparer<Tuple<T1, T2, T3, T4, T5>>
{
    public bool Equals(Tuple<T1, T2, T3, T4, T5>? x, Tuple<T1, T2, T3, T4, T5>? y) =>
        ReferenceEquals(x, y) ||
        x is not null &&
        y is not null &&
        m_Comparer1.Equals(x.Item1, y.Item1) &&
        m_Comparer2.Equals(x.Item2, y.Item2) &&
        m_Comparer3.Equals(x.Item3, y.Item3) &&
        m_Comparer4.Equals(x.Item4, y.Item4) &&
        m_Comparer5.Equals(x.Item5, y.Item5);

    public int GetHashCode(Tuple<T1, T2, T3, T4, T5> obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        return HashCode.Combine(
            m_Comparer1.GetNullableHashCode(obj.Item1),
            m_Comparer2.GetNullableHashCode(obj.Item2),
            m_Comparer3.GetNullableHashCode(obj.Item3),
            m_Comparer4.GetNullableHashCode(obj.Item4),
            m_Comparer5.GetNullableHashCode(obj.Item5));
    }

    readonly IEqualityComparer<T1> m_Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
    readonly IEqualityComparer<T2> m_Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
    readonly IEqualityComparer<T3> m_Comparer3 = comparer3 ?? EqualityComparer<T3>.Default;
    readonly IEqualityComparer<T4> m_Comparer4 = comparer4 ?? EqualityComparer<T4>.Default;
    readonly IEqualityComparer<T5> m_Comparer5 = comparer5 ?? EqualityComparer<T5>.Default;
}
