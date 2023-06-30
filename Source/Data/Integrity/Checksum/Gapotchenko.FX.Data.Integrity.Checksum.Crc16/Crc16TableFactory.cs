// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Numerics;
using System.Collections.Concurrent;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

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
            {
                wr.SetTarget(table);
            }
            else
            {
                var staleKeys = m_Cache.Where(x => !x.Value.TryGetTarget(out _)).Select(x => x.Key).ToList();
                foreach (var i in staleKeys)
                    m_Cache.TryRemove(i, out _);

                m_Cache[key] = new WeakReference<ushort[]>(table);
            }
        }

        return table;
    }

    static ushort[] CreateTable(ushort polynomial, bool reflectedInput)
    {
        const int Width = 16;
        const ushort FirstBit = 0b1;
        const ushort LastBit = 1 << (Width - 1);

        const int TableSize = 256;
        var table = new ushort[TableSize];

        if (reflectedInput)
        {
            polynomial = BitOperationsEx.Reverse(polynomial);

            for (ushort i = 0; i < TableSize; ++i)
            {
                ushort value = i;

                for (int j = 0; j < 8; ++j)
                {
                    if ((value & FirstBit) != 0)
                        value = (ushort)((value >> 1) ^ polynomial);
                    else
                        value >>= 1;
                }

                table[i] = value;
            }
        }
        else
        {
            for (ushort i = 0; i < TableSize; ++i)
            {
                ushort value = (ushort)(i << (Width - 8));

                for (int j = 0; j < 8; ++j)
                {
                    if ((value & LastBit) != 0)
                        value = (ushort)((value << 1) ^ polynomial);
                    else
                        value <<= 1;
                }

                table[i] = value;
            }
        }

        return table;
    }

    static readonly ConcurrentDictionary<(ushort, bool), WeakReference<ushort[]>> m_Cache = new();
}
