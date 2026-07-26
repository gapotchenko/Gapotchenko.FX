// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Memory;

partial class MemoryEqualityComparer
{
    sealed class ByteMemoryComparer : EquatableMemoryComparer<byte>
    {
        public static ByteMemoryComparer Instance { get; } = new();

        ByteMemoryComparer()
        {
        }

        public override int GetHashCode(ReadOnlyMemory<byte> obj)
        {
            return GetHashCodeCore(obj);
        }

        public override bool Equals(object? obj) => obj is ByteMemoryComparer;

        public override int GetHashCode() => HashCode.Combine(0xa6f8af60);
    }

    sealed class BitwiseMemoryComparer<T> : EquatableMemoryComparer<T>
        where T : struct, IEquatable<T>
    {
        public static BitwiseMemoryComparer<T> Instance { get; } = new();

        BitwiseMemoryComparer()
        {
        }

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            return GetBitwiseHashCodeCore(obj);
        }

        public override bool Equals(object? obj) => obj is BitwiseMemoryComparer<T>;

        public override int GetHashCode() => HashCode.Combine(0x3863f10f, typeof(T).GetHashCode());
    }

    abstract class EquatableMemoryComparer<T> : MemoryEqualityComparer<T>
        where T : IEquatable<T>
    {
        public sealed override bool Equals(ReadOnlyMemory<T> x, ReadOnlyMemory<T> y)
        {
            return EqualsCore(x, y);
        }
    }
}
