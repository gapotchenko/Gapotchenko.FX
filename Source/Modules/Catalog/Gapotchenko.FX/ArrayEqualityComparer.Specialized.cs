// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Runtime.InteropServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX;

partial class ArrayEqualityComparer
{
    sealed class ByteArrayComparer : EquatableArrayComparer<byte>
    {
        public static ByteArrayComparer Instance { get; } = new();

        ByteArrayComparer()
        {
        }

        public override int GetHashCode(byte[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashOperations.GetHashCode(obj);
        }

        public override bool Equals(object? obj) => obj is ByteArrayComparer;

        public override int GetHashCode() => HashCode.Combine(0xf0b6b327);
    }

    sealed class BitwiseArrayComparer<T> : EquatableArrayComparer<T>
        where T : struct, IEquatable<T>
    {
        public static BitwiseArrayComparer<T> Instance { get; } = new();

        BitwiseArrayComparer()
        {
        }

        public override int GetHashCode(T[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashOperations.GetHashCode(MemoryMarshal.AsBytes(obj));
        }

        public override bool Equals(object? obj) => obj is BitwiseArrayComparer<T>;

        public override int GetHashCode() => HashCode.Combine(0x4e282909, typeof(T).GetHashCode());
    }

    abstract class EquatableArrayComparer<T> : ArrayEqualityComparer<T>
        where T : IEquatable<T>?
    {
        public sealed override bool Equals(T[]? x, T[]? y)
        {
            return EqualsCore(x, y);
        }
    }
}
