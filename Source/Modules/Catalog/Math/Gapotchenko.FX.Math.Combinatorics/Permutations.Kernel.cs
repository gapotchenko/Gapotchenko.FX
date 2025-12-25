// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.Math.Combinatorics;

partial class Permutations
{
    static IEnumerable<IResultRow<T>> Permute<T>(IEnumerable<T> sequence, bool distinct, IEqualityComparer<T>? comparer)
    {
        var items = sequence.ReifyList();

        int n = items.Count;
        var transform = new (int First, int Second)[n];

        if (!distinct || Utility.IsCompatibleSet(sequence, comparer))
        {
            // Identity transform.
            for (int i = 0; i < n; i++)
                transform[i] = (i, i);
        }
        else
        {
            var map = new AssociativeArray<T, List<int>>(comparer);

            for (int i = 0; i < n; ++i)
            {
                var item = items[i];

                if (!map.TryGetValue(item, out var list))
                    map.Add(item, list = []);

                list.Add(i);
            }

            var lists = map.Values.AsEnumerable();

            int transformIndex = 0;
            int elementIndex = 0;

            foreach (var list in lists)
            {
                foreach (int i in list)
                    transform[transformIndex++] = (elementIndex, i);
                elementIndex += list.Count;
            }
        }

        yield return new ResultRow<T>(items, [.. transform.Select(x => x.Second)]);

        for (; ; )
        {
            // Reference: E. W. Dijkstra, A Discipline of Programming, Prentice-Hall, 1997
            // Find the largest partition from the back that is in decreasing (non-increasing) order.
            int decreasingPart;
            for (decreasingPart = n - 2;
                decreasingPart >= 0 && transform[decreasingPart].First >= transform[decreasingPart + 1].First;
                --decreasingPart)
            {
            }

            // The whole sequence is in decreasing order, finished.
            if (decreasingPart < 0)
                yield break;

            ref var a = ref transform[decreasingPart];

            // Find the smallest element in the decreasing partition that is 
            // greater than (or equal to) the item in front of the decreasing partition.
            int greater;
            for (greater = n - 1;
                greater > decreasingPart && a.First >= transform[greater].First;
                --greater)
            {
            }

            ref var b = ref transform[greater];

            // Swap the two.
            (a, b) = (b, a);

            // Reverse the decreasing partition.
            Array.Reverse(transform, decreasingPart + 1, n - decreasingPart - 1);

            yield return new ResultRow<T>(items, [.. transform.Select(x => x.Second)]);
        }
    }
}
