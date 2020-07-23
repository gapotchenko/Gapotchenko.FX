using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Linq
{
    sealed class SelectedEqualityComparer<TSource, TResult> : IEqualityComparer<TSource>
    {
        delegate TResult Selector([AllowNull] TSource source);

        public SelectedEqualityComparer(Func<TSource, TResult> selector, IEqualityComparer<TResult>? comparer)
        {
            m_Selector = new Selector(selector);
            m_Comparer = comparer ?? EqualityComparer<TResult>.Default;
        }

        readonly Selector m_Selector;
        readonly IEqualityComparer<TResult> m_Comparer;

        public bool Equals([AllowNull] TSource x, [AllowNull] TSource y) => m_Comparer.Equals(m_Selector(x), m_Selector(y));

        public int GetHashCode(TSource obj)
        {
            var result = m_Selector(obj);
            if (result is null)
                return 0;
            return m_Comparer.GetHashCode(result);
        }
    }
}
