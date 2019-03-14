using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Linq
{
    sealed class KeyedEqualityComparer<TSource, TKey> : IEqualityComparer<TSource>
    {
        public KeyedEqualityComparer(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            _KeySelector = keySelector;
            _Comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        readonly Func<TSource, TKey> _KeySelector;
        readonly IEqualityComparer<TKey> _Comparer;

        public bool Equals(TSource x, TSource y) => _Comparer.Equals(_KeySelector(x), _KeySelector(y));

        public int GetHashCode(TSource obj) => _Comparer.GetHashCode(_KeySelector(obj));
    }
}
