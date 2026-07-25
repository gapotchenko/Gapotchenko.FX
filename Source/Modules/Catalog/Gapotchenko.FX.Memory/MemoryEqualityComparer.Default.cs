// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Memory;

partial class MemoryEqualityComparer
{
    sealed class DefaultMemoryComparer<T> : MemoryEqualityComparer<T>
    {
        public static DefaultMemoryComparer<T> Instance { get; } = new();

        DefaultMemoryComparer()
        {
        }

        public override bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
        {
            return EqualsCore(x, y);
        }

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            return GetHashCodeCore(obj);
        }

        public override bool Equals(object? obj) => obj is DefaultMemoryComparer<T>;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
