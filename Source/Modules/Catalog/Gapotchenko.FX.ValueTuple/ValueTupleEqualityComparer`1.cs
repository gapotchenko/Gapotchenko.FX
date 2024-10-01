﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.ValueTuple;

sealed class ValueTupleEqualityComparer<T1>(IEqualityComparer<T1>? comparer1) : IEqualityComparer<ValueTuple<T1>>
{
    public bool Equals(ValueTuple<T1> x, ValueTuple<T1> y) => m_Comparer1.Equals(x.Item1, y.Item1);

    public int GetHashCode(ValueTuple<T1> obj) => Util.GetSafeHashCode(m_Comparer1, obj.Item1);

    readonly IEqualityComparer<T1> m_Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
}
