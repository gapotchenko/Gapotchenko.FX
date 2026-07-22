// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.IO.Hashing;
using System.Runtime.InteropServices;

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

            var hash = new XxHash3();
            Span<int> buffer = stackalloc int[1];
            foreach (var i in obj.Span)
            {
                buffer[0] = GetElementHashCode(i, elementComparer);
                hash.Append(MemoryMarshal.AsBytes(buffer));
            }
            return (int)hash.GetCurrentHashAsUInt64();

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
