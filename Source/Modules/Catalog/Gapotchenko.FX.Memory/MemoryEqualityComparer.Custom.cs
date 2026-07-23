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
            return EqualsCore(x, y, elementComparer);
        }

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            return GetHashCodeCore(obj, elementComparer);
        }

        public override bool Equals(object? obj) => obj is CustomMemoryComparer<T>;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
