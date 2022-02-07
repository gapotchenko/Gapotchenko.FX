using Gapotchenko.FX.Numerics;
using System;
using System.Collections.Concurrent;

namespace Gapotchenko.FX.Data.Checksum
{
    static class Crc16TableFactory
    {
        public static ushort[] GetTable(ushort polynomial, bool reflectedInput)
        {
            var key = (polynomial, reflectedInput);

            if (m_Cache.TryGetValue(key, out var wr) &&
                wr.TryGetTarget(out var table))
            {
                return table;
            }

            lock (m_Cache)
            {
                if ((wr != null || m_Cache.TryGetValue(key, out wr)) &&
                    wr.TryGetTarget(out table))
                {
                    return table;
                }

                table = CreateTable(polynomial, reflectedInput);

                if (wr != null)
                    wr.SetTarget(table);
                else
                    m_Cache[key] = new WeakReference<ushort[]>(table);
            }

            return table;
        }

        static ushort[] CreateTable(ushort polynomial, bool reflectedInput)
        {
            // TODO

            if (reflectedInput)
                polynomial = BitOperationsEx.Reverse(polynomial);

            const int TableSize = 256;

            var table = new ushort[TableSize];

            for (int i = 0; i < TableSize; ++i)
            {
                ushort value = 0;

                var temp = i;
                for (int j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0b1) != 0)
                        value = (ushort)((value >> 1) ^ polynomial);
                    else
                        value >>= 1;
                    temp >>= 1;
                }

                table[i] = value;
            }

            return table;
        }

        static readonly ConcurrentDictionary<(ushort, bool), WeakReference<ushort[]>> m_Cache = new();
    }
}
