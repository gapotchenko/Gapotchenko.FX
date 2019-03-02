using Gapotchenko.FX.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX
{
    partial class ArrayEqualityComparer
    {
        internal sealed class ByteRank1Comparer : EqualityComparer<byte[]>
        {
            public override bool Equals(byte[] x, byte[] y)
            {
                if (x == y)
                    return true;

                if (x == null || y == null)
                    return false;

                if (x.Length != y.Length)
                    return false;

                if (CodeSafetyStrategy.IsUnsafeCodeRecommended)
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
                var n = (uint)x.Length;
                if (n == 0)
                    return true;

                fixed (byte* p1 = x, p2 = y)
                {
                    byte* ix1 = p1, ix2 = p2;

                    int wordSize = IntPtr.Size;

                    if (n < wordSize)
                    {
                        for (uint i = 0; i < n; i++, ix1++, ix2++)
                        {
                            if (*ix1 != *ix2)
                                return false;
                        }
                    }
                    else
                    {
                        if (wordSize == 4)
                        {
                            // 32-bit architecture.
                            uint n4 = n >> 2;
                            for (uint i = 0; i < n4; i++, ix1 += 4, ix2 += 4)
                            {
                                if (*((uint*)ix1) != *((uint*)ix2))
                                    return false;
                            }
                        }
                        else
                        {
                            // 64-bit architecture.
                            uint n8 = n >> 3;
                            for (uint i = 0; i < n8; i++, ix1 += 8, ix2 += 8)
                            {
                                if (*((ulong*)ix1) != *((ulong*)ix2))
                                    return false;
                            }

                            if ((n & 4) != 0)
                            {
                                if (*((uint*)ix1) != *((uint*)ix2))
                                    return false;
                                ix1 += 4;
                                ix2 += 4;
                            }
                        }

                        if ((n & 2) != 0)
                        {
                            if (*((ushort*)ix1) != *((ushort*)ix2))
                                return false;
                            ix1 += 2;
                            ix2 += 2;
                        }

                        if ((n & 1) != 0)
                        {
                            if (*ix1 != *ix2)
                                return false;
                        }
                    }
                }

                return true;
            }

            public override int GetHashCode(byte[] obj)
            {
                if (obj == null)
                    return 0;

                // FNV-1a
                // The fastest hash function for byte arrays with lowest collision rate so far (10/2014).
                // http://isthe.com/chongo/tech/comp/fnv/

                if (CodeSafetyStrategy.IsUnsafeCodeRecommended)
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
