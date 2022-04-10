using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Memory
{
    partial class MemoryEqualityComparer
    {
        sealed class DefaultComparer<T> : MemoryEqualityComparer<T>
        {
            internal DefaultComparer(IEqualityComparer<T>? elementComparer)
            {
                m_ElementComparer = elementComparer ?? EqualityComparer<T>.Default;
            }

            readonly IEqualityComparer<T> m_ElementComparer;

            public override bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
            {
                int n = x.Length;

                if (y.Length != n)
                    return false;

                if (x.Equals(y))
                    return true;

                var xs = x.Span;
                var ys = y.Span;

                for (int i = 0; i < n; ++i)
                    if (!m_ElementComparer.Equals(xs[i], ys[i]))
                        return false;

                return true;
            }

            public override int GetHashCode(ReadOnlyMemory<T> obj)
            {
                if (obj.IsEmpty)
                    return 0;

                var elementComparer = m_ElementComparer;

                // FNV-1a
                uint hash = 2166136261;
                foreach (var i in obj.Span)
                    hash = (hash ^ (uint)InternalGetHashCode(i, elementComparer)) * 16777619;
                return (int)hash;
            }

            static int InternalGetHashCode(T value, IEqualityComparer<T> comparer)
            {
                if (value is null)
                    return 0;
                return comparer.GetHashCode(value);
            }
        }
    }
}
