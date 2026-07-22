// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

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
            var elementComparer = m_ElementComparer;

            // FNV-1a
            uint hash = 2166136261;
            foreach (var i in obj.Span)
                hash = (hash ^ (uint)GetElementHashCode(i, elementComparer)) * 16777619;
            return (int)hash;

            static int GetElementHashCode(T value, IEqualityComparer<T> comparer)
            {
                if (value is null)
                    return 0;
                else
                    return comparer.GetHashCode(value);
            }
        }

        readonly IEqualityComparer<T> m_ElementComparer = elementComparer ?? EqualityComparer<T>.Default;
    }
}
