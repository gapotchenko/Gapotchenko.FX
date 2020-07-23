using System;
using System.Collections.Generic;

#nullable enable

namespace Gapotchenko.FX
{
    [Serializable]
    sealed class OptionalComparer<T> : IComparer<Optional<T>>
    {
        public OptionalComparer(IComparer<T>? valueComparer)
        {
            m_ValueComparer = valueComparer ?? Comparer<T>.Default;
        }

        readonly IComparer<T> m_ValueComparer;

        public int Compare(Optional<T> x, Optional<T> y) => CompareCore(x, y, m_ValueComparer);

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
