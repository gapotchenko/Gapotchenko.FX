using Gapotchenko.FX.Runtime.InteropServices;
using System.IO.Hashing;

namespace Gapotchenko.FX;

partial class ArrayEqualityComparer
{
    internal sealed class ByteRank1Comparer : ArrayEqualityComparer<byte>
    {
        public static ByteRank1Comparer Instance = new();

        ByteRank1Comparer()
        {
        }

        public override bool Equals(byte[]? x, byte[]? y)
        {
            if (x == y)
                return true;
            if (x is null || y is null)
                return false;
            if (x.Length != y.Length)
                return false;

            return x.AsSpan().SequenceEqual(y);
        }

        public override int GetHashCode(byte[] obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return (int)XxHash3.HashToUInt64(obj);
        }

        public override bool Equals(object? obj) => obj is ByteRank1Comparer;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
