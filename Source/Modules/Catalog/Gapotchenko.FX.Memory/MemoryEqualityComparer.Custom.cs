// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Memory;

partial class MemoryEqualityComparer
{
    sealed class CustomMemoryComparer<T>(IEqualityComparer<T> elementComparer) : MemoryEqualityComparer<T>
    {
        public override bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
        {
            return EqualsCore(x, y, m_ElementComparer);
        }

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            return GetHashCodeCore(obj, m_ElementComparer);
        }

        public override bool Equals(object? obj)
        {
            return
                obj is CustomMemoryComparer<T> other &&
                m_ElementComparer.Equals(other.m_ElementComparer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                0x1d2f8650,
                m_ElementComparer.GetHashCode());
        }

        readonly IEqualityComparer<T> m_ElementComparer = elementComparer;
    }
}
