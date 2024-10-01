#if !TFF_ENUMERABLE_DISTINCTBY

namespace Gapotchenko.FX.Linq;

sealed class SelectedEqualityComparer<TSource, TResult> : IEqualityComparer<TSource>
{
    public SelectedEqualityComparer(Func<TSource, TResult> selector, IEqualityComparer<TResult>? comparer)
    {
        m_Selector = selector;
        m_Comparer = comparer ?? EqualityComparer<TResult>.Default;
    }

    readonly Func<TSource, TResult> m_Selector;
    readonly IEqualityComparer<TResult> m_Comparer;

    public bool Equals(TSource? x, TSource? y) =>
        (x is null && y is null) ||
        (x is not null) &&
        (y is not null) &&
        m_Comparer.Equals(m_Selector(x), m_Selector(y));

    public int GetHashCode(TSource obj)
    {
        var result = m_Selector(obj);
        if (result is null)
            return 0;
        return m_Comparer.GetHashCode(result);
    }
}

#endif
