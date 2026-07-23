using Gapotchenko.FX.Runtime.InteropServices;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX;

partial class ArrayEqualityComparer
{
    sealed class ByteArrayComparer : EquatableArrayComparer<byte>
    {
        public static ByteArrayComparer Instance = new();

        ByteArrayComparer()
        {
        }

        public override int GetHashCode(byte[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashOperations.GetHashCode(obj);
        }

        public override bool Equals(object? obj) => obj is ByteArrayComparer;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }

    sealed class BitwiseArrayComparer<T> : EquatableArrayComparer<T>
        where T : struct, IEquatable<T>
    {
        public static BitwiseArrayComparer<T> Instance = new();

        BitwiseArrayComparer()
        {
        }

        public override int GetHashCode(T[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashOperations.GetHashCode(MemoryMarshal.AsBytes(obj));
        }

        public override bool Equals(object? obj) => obj is BitwiseArrayComparer<T>;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }

    abstract class EquatableArrayComparer<T> : ArrayEqualityComparer<T>
        where T : IEquatable<T>
    {
        public sealed override bool Equals(T[]? x, T[]? y)
        {
            return EqualsCore(x, y);
        }
    }
}
