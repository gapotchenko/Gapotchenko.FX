﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.ValueTuple;

sealed class ValueTupleEqualityComparer<T1, T2>(IEqualityComparer<T1>? comparer1, IEqualityComparer<T2>? comparer2) : IEqualityComparer<ValueTuple<T1, T2>>
{
    public bool Equals(ValueTuple<T1, T2> x, ValueTuple<T1, T2> y) =>
        m_Comparer1.Equals(x.Item1, y.Item1) &&
        m_Comparer2.Equals(x.Item2, y.Item2);

    public int GetHashCode(ValueTuple<T1, T2> obj) =>
        HashCode.Combine(
            Util.GetSafeHashCode(m_Comparer1, obj.Item1),
            Util.GetSafeHashCode(m_Comparer2, obj.Item2));

    readonly IEqualityComparer<T1> m_Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
    readonly IEqualityComparer<T2> m_Comparer2 = comparer2 ?? EqualityComparer<T2>.Default;
}
