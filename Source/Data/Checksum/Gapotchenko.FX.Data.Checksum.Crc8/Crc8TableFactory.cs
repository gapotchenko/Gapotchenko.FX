using Gapotchenko.FX.Numerics;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Gapotchenko.FX.Data.Checksum
{
    static class Crc8TableFactory
    {
        public static byte[] GetTable(byte polynomial, bool reflectedInput)
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
                {
                    wr.SetTarget(table);
                }
                else
                {
                    var staleKeys = m_Cache.Where(x => !x.Value.TryGetTarget(out _)).Select(x => x.Key).ToList();
                    foreach (var i in staleKeys)
                        m_Cache.TryRemove(i, out _);

                    m_Cache[key] = new WeakReference<byte[]>(table);
                }
            }

            return table;
        }

        static byte[] CreateTable(byte polynomial, bool reflectedInput)
        {
            const int Width = 8;
            const byte FirstBit = 0b1;
            const byte LastBit = 1 << (Width - 1);

            const int TableSize = 256;
            var table = new byte[TableSize];

            if (reflectedInput)
            {
                polynomial = BitOperationsEx.Reverse(polynomial);

                for (uint i = 0; i < TableSize; ++i)
                {
                    var value = (byte)i;

                    for (int j = 0; j < 8; ++j)
                    {
                        if ((value & FirstBit) != 0)
                            value = (byte)((value >> 1) ^ polynomial);
                        else
                            value >>= 1;
                    }

                    table[i] = value;
                }
            }
            else
            {
                for (uint i = 0; i < TableSize; ++i)
                {
                    var value = (byte)i;

                    for (int j = 0; j < 8; ++j)
                    {
                        if ((value & LastBit) != 0)
                            value = (byte)((value << 1) ^ polynomial);
                        else
                            value <<= 1;
                    }

                    table[i] = value;
                }
            }

            return table;
        }

        static readonly ConcurrentDictionary<(byte, bool), WeakReference<byte[]>> m_Cache = new();
    }
}
