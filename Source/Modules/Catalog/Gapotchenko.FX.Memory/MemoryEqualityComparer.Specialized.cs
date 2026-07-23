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
        public static ByteMemoryComparer Instance = new();

        ByteMemoryComparer()
        {
        }

        public override int GetHashCode(ReadOnlyMemory<byte> obj)
        {
            return GetHashCodeCore(obj);
        }

        public override bool Equals(object? obj) => obj is ByteMemoryComparer;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }

    sealed class StructMemoryComparer<T> : EquatableMemoryComparer<T>
        where T : struct, IEquatable<T>
    {
        public static StructMemoryComparer<T> Instance = new();

        StructMemoryComparer()
        {
        }

        public override int GetHashCode(ReadOnlyMemory<T> obj)
        {
            return GetBlittableStructHashCodeCore(obj);
        }

        public override bool Equals(object? obj) => obj is StructMemoryComparer<T>;

        public override int GetHashCode() => GetType().Name.GetHashCode();
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
