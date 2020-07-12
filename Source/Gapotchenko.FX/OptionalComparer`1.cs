using System;
using System.Collections.Generic;

namespace Gapotchenko.FX
{
    [Serializable]
    sealed class OptionalComparer<T> : IComparer<Optional<T>>
    {
        public OptionalComparer(IComparer<T> valueComparer)
        {
            _ValueComparer = valueComparer ?? Comparer<T>.Default;
        }

        readonly IComparer<T> _ValueComparer;

        public int Compare(Optional<T> x, Optional<T> y) => CompareCore(x, y, _ValueComparer);

        static internal int CompareCore(Optional<T> x, Optional<T> y, IComparer<T> valueComparer)
        {
            if (x.HasValue)
            {
                if (y.HasValue)
                    return valueComparer.Compare(x.Value, y.Value);
                return 1;
            }
            else if (y.HasValue)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
