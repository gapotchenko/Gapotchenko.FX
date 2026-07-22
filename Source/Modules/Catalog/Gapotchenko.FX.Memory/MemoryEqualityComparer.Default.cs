// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using Gapotchenko.FX.Runtime.InteropServices;

namespace Gapotchenko.FX.Memory;

partial class MemoryEqualityComparer
{
    sealed class DefaultComparer<T>(IEqualityComparer<T>? elementComparer) : MemoryEqualityComparer<T>
    {
        public override bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
        {
            int n = x.Length;
            if (n != y.Length)
                return false;

            if (x.Equals(y))
                return true;

            var xs = x.Span;
            var ys = y.Span;

            for (int i = 0; i < n; ++i)
            {
                if (!m_ElementComparer.Equals(xs[i], ys[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            return HashOperations.GetHashCode(obj.Span, m_ElementComparer);
        }

        readonly IEqualityComparer<T> m_ElementComparer = elementComparer ?? EqualityComparer<T>.Default;
    }
}
