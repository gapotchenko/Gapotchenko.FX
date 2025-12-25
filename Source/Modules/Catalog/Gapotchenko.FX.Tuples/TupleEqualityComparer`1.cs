// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Tuples.Utils;

namespace Gapotchenko.FX.Tuples;

sealed class TupleEqualityComparer<T1>(IEqualityComparer<T1>? comparer1) : IEqualityComparer<Tuple<T1>>
{
    public bool Equals(Tuple<T1>? x, Tuple<T1>? y) =>
        ReferenceEquals(x, y) ||
        x is not null &&
        y is not null &&
        m_Comparer1.Equals(x.Item1, y.Item1);

    public int GetHashCode(Tuple<T1> obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return m_Comparer1.GetNullableHashCode(obj.Item1);
    }

    readonly IEqualityComparer<T1> m_Comparer1 = comparer1 ?? EqualityComparer<T1>.Default;
}
