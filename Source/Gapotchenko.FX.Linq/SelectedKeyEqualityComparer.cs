using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Linq
{
    sealed class SelectedKeyEqualityComparer<TSource, TKey> : IEqualityComparer<TSource>
    {
        public SelectedKeyEqualityComparer(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
        {
            _KeySelector = keySelector;
            _KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        readonly Func<TSource, TKey> _KeySelector;
        readonly IEqualityComparer<TKey> _KeyComparer;

        public bool Equals(TSource x, TSource y) => _KeyComparer.Equals(_KeySelector(x), _KeySelector(y));

        public int GetHashCode(TSource obj) => _KeyComparer.GetHashCode(_KeySelector(obj));
    }
}
