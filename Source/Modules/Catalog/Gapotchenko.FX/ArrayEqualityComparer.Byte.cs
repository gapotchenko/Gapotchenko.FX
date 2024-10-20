using Gapotchenko.FX.Runtime.InteropServices;

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
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            // FNV-1a
            // The fastest hash function for byte arrays with lowest collision rate so far (10/2014).
            // https://en.wikipedia.org/wiki/Fowler-Noll-Vo_hash_function

            if (CodeSafetyStrategy.UnsafeCodeRecommended)
                return GetHashCodeUnsafeCore(obj);
            else
                return GetHashCodeSafeCore(obj);
        }

        static int GetHashCodeSafeCore(byte[] obj)
        {
            uint hash = 2166136261;
            foreach (var i in obj)
                hash = (hash ^ i) * 16777619;
            return (int)hash;
        }

        static unsafe int GetHashCodeUnsafeCore(byte[] obj)
        {
            uint hash = 2166136261;
            fixed (byte* buffer = obj)
            {
                byte* p = buffer;
                byte* p_end = p + obj.Length;
                while (p < p_end)
                {
                    hash = (hash ^ *p) * 16777619;
                    ++p;
                }
            }
            return (int)hash;
        }

        public override bool Equals(object? obj) => obj is ByteRank1Comparer;

        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
