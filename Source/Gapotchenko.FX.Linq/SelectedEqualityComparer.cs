using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Linq
{
    sealed class SelectedEqualityComparer<TSource, TResult> : IEqualityComparer<TSource>
    {
        public SelectedEqualityComparer(Func<TSource, TResult> selector, IEqualityComparer<TResult> comparer)
        {
            m_Selector = selector;
            m_Comparer = comparer ?? EqualityComparer<TResult>.Default;
        }

        readonly Func<TSource, TResult> m_Selector;
        readonly IEqualityComparer<TResult> m_Comparer;

        public bool Equals(TSource x, TSource y) => m_Comparer.Equals(m_Selector(x), m_Selector(y));

        public int GetHashCode(TSource obj) => m_Comparer.GetHashCode(m_Selector(obj));
    }
}
