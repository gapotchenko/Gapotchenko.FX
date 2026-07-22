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
    sealed class CustomMemoryComparer<T>(IEqualityComparer<T> elementComparer) : MemoryEqualityComparer<T>
    {
        public override bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
        {
            int n = x.Length;
            if (n != y.Length)
                return false;

            if (x.Equals(y))
                return true;

            return x.Span.SequenceEqual(y.Span, elementComparer);
        }

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            return HashOperations.GetHashCode(obj.Span, elementComparer);
        }
    }
}
