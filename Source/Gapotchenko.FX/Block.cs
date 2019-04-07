using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX
{
    static class Block
    {
        public static unsafe void Copy(void* src, void* dest, int size)
        {
            var s = (byte*)src;
            var d = (byte*)dest;
            var n = (uint)size;

            int wordSize = IntPtr.Size;

            if (n < wordSize)
            {
                for (uint i = 0; i < n; i++, d++, s++)
                    *d = *s;
            }
            else
            {
                if (wordSize == 4)
                {
                    // 32-bit architecture.
                    uint n4 = n >> 2;
                    for (uint i = 0; i < n4; i++, d += 4, s += 4)
                        *((uint*)d) = *((uint*)s);
                }
                else
                {
                    // 64-bit architecture.
                    uint n8 = n >> 3;
                    for (uint i = 0; i < n8; i++, d += 8, s += 8)
                        *((ulong*)d) = *((ulong*)s);

                    if ((n & 4) != 0)
                    {
                        *((uint*)d) = *((uint*)s);
                        d += 4;
                        s += 4;
                    }
                }

                if ((n & 2) != 0)
                {
                    *((ushort*)d) = *((ushort*)s);
                    d += 2;
                    s += 2;
                }

                if ((n & 1) != 0)
                    *d = *s;
            }
        }

        public static unsafe bool Equals(void* ptrA, void* ptrB, int size)
        {
            var a = (byte*)ptrA;
            var b = (byte*)ptrB;
            var n = (uint)size;

            int wordSize = IntPtr.Size;

            if (n < wordSize)
            {
                for (uint i = 0; i < n; i++, a++, b++)
                {
                    if (*a != *b)
                        return false;
                }
            }
            else
            {
                if (wordSize == 4)
                {
                    // 32-bit architecture.
                    uint n4 = n >> 2;
                    for (uint i = 0; i < n4; i++, a += 4, b += 4)
                    {
                        if (*((uint*)a) != *((uint*)b))
                            return false;
                    }
                }
                else
                {
                    // 64-bit architecture.
                    uint n8 = n >> 3;
                    for (uint i = 0; i < n8; i++, a += 8, b += 8)
                    {
                        if (*((ulong*)a) != *((ulong*)b))
                            return false;
                    }

                    if ((n & 4) != 0)
                    {
                        if (*((uint*)a) != *((uint*)b))
                            return false;
                        a += 4;
                        b += 4;
                    }
                }

                if ((n & 2) != 0)
                {
                    if (*((ushort*)a) != *((ushort*)b))
                        return false;
                    a += 2;
                    b += 2;
                }

                if ((n & 1) != 0)
                {
                    if (*a != *b)
                        return false;
                }
            }

            return true;
        }

    }
}
