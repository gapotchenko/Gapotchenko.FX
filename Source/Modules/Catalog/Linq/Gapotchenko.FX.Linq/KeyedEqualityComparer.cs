// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

#if !TFF_ENUMERABLE_DISTINCTBY

namespace Gapotchenko.FX.Linq;

sealed class KeyedEqualityComparer<TSource, TKey>(
    Func<TSource, TKey> selector,
    IEqualityComparer<TKey>? comparer) :
    IEqualityComparer<TSource>
{
    public bool Equals(TSource? x, TSource? y) =>
        x is null && y is null ||
        x is not null &&
        y is not null &&
        m_Comparer.Equals(selector(x), selector(y));

    public int GetHashCode(TSource obj) =>
        selector(obj) is not null and var key ?
            m_Comparer.GetHashCode(key) :
            0;

    readonly IEqualityComparer<TKey> m_Comparer = comparer ?? EqualityComparer<TKey>.Default;
}

#endif
