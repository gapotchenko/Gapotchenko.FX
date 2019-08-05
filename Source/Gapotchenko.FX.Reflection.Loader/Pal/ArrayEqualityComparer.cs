using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Reflection.Pal
{
    static class ArrayEqualityComparer
    {
        public static bool Equals(byte[] a, byte[] b)
        {
            if (a == b)
                return true;
            if (a is null || b is null)
                return false;

            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; ++i)
                if (a[i] != b[i])
                    return false;

            return true;
        }
    }
}
