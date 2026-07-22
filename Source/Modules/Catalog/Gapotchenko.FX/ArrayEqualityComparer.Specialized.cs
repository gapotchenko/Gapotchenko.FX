using Gapotchenko.FX.Runtime.InteropServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX;

partial class ArrayEqualityComparer
{
    sealed class ByteArrayComparer : ArrayEqualityComparer<byte>
    {
        public static ByteArrayComparer Instance = new();

        ByteArrayComparer()
        {
        }

        public override bool Equals(byte[]? x, byte[]? y)
        {
            if (x == y)
                return true;
            if (x is null || y is null)
                return false;

            return x.SequenceEqual(y);
        }

        public override int GetHashCode(byte[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashOperations.GetHashCode(obj);
        }

        public override bool Equals(object? obj) => obj is ByteArrayComparer;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }

    sealed class StructArrayComparer<T> : ArrayEqualityComparer<T>
        where T : struct
    {
        public static StructArrayComparer<T> Instance = new();

        StructArrayComparer()
        {
        }

        public override bool Equals(T[]? x, T[]? y)
        {
            if (x == y)
                return true;
            if (x is null || y is null)
                return false;

            return x.SequenceEqual(y);
        }

        public override int GetHashCode(T[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashOperations.GetHashCode(MemoryMarshal.AsBytes(obj));
        }

        public override bool Equals(object? obj) => obj is StructArrayComparer<T>;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
