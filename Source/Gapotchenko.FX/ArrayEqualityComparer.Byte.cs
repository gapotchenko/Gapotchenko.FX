using Gapotchenko.FX.Runtime.InteropServices;

namespace Gapotchenko.FX
{
    partial class ArrayEqualityComparer
    {
        internal sealed class ByteRank1Comparer : ArrayEqualityComparer<byte>
        {
            public override bool Equals(byte[] x, byte[] y)
            {
                if (x == y)
                    return true;

                if (x == null || y == null)
                    return false;

                if (x.Length != y.Length)
                    return false;

                if (CodeSafetyStrategy.UnsafeCodeRecommended)
                    return _EqualsUnsafeCore(x, y);
                else
                    return _EqualsSafeCore(x, y);
            }

            static bool _EqualsSafeCore(byte[] x, byte[] y)
            {
                for (int i = 0; i < x.Length; i++)
                    if (x[i] != y[i])
                        return false;
                return true;
            }

            static unsafe bool _EqualsUnsafeCore(byte[] x, byte[] y)
            {
                var n = x.Length;
                if (n == 0)
                    return true;

                fixed (byte* px = x, py = y)
                    return MemoryOperations.BlockEquals(px, py, n);
            }

            public override int GetHashCode(byte[] obj)
            {
                if (obj == null)
                    return 0;

                // FNV-1a
                // The fastest hash function for byte arrays with lowest collision rate so far (10/2014).
                // http://isthe.com/chongo/tech/comp/fnv/

                if (CodeSafetyStrategy.UnsafeCodeRecommended)
                    return _GetHashCodeUnsafeCore(obj);
                else
                    return _GetHashCodeSafeCore(obj);
            }

            static int _GetHashCodeSafeCore(byte[] obj)
            {
                uint hash = 2166136261;
                foreach (var i in obj)
                    hash = (hash ^ i) * 16777619;
                return (int)hash;
            }

            static unsafe int _GetHashCodeUnsafeCore(byte[] obj)
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

            public override bool Equals(object obj) => obj is ByteRank1Comparer;

            public override int GetHashCode() => GetType().Name.GetHashCode();
        }
    }
}
