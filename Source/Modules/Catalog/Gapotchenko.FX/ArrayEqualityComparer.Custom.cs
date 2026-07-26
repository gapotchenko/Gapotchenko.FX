// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Runtime.InteropServices;

namespace Gapotchenko.FX;

partial class ArrayEqualityComparer
{
    sealed class CustomArrayComparer<T>(IEqualityComparer<T> elementComparer) : ArrayEqualityComparer<T>
    {
        public override bool Equals(T[]? x, T[]? y)
        {
            return EqualsCore(x, y, m_ElementComparer);
        }

        public override int GetHashCode(T[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashOperations.GetHashCode(obj, m_ElementComparer);
        }

        public override bool Equals(object? obj)
        {
            return
                obj is CustomArrayComparer<T> other &&
                m_ElementComparer.Equals(other.m_ElementComparer);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                0x593ab91c,
                m_ElementComparer.GetHashCode());
        }

        readonly IEqualityComparer<T> m_ElementComparer = elementComparer;
    }
}
