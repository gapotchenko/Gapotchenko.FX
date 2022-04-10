using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics
{
    partial class Permutations
    {
        static IEnumerable<IRow<T>> Permute<T>(IEnumerable<T> sequence, bool distinct, IEqualityComparer<T>? comparer)
        {
            var items = sequence.AsReadOnlyList();

            int length = items.Count;
            var transform = new (int First, int Second)[length];

            if (!distinct || Utility.IsCompatibleSet(sequence, comparer))
            {
                // Identity transform.
                for (int i = 0; i < length; i++)
                    transform[i] = (i, i);
            }
            else
            {
#pragma warning disable CS8714
                var map = new Dictionary<T, List<int>>(comparer);
#pragma warning restore CS8714
                List<int>? nullList = null;

                for (int i = 0; i < length; ++i)
                {
                    var item = items[i];

                    List<int>? list;
                    if (item is null)
                    {
                        nullList ??= new List<int>();
                        list = nullList;
                    }
                    else if (!map.TryGetValue(item, out list))
                    {
                        list = new List<int>();
                        map.Add(item, list);
                    }

                    list.Add(i);
                }

                var lists = map.Values.AsEnumerable();
                if (nullList != null)
                    lists = lists.Prepend(nullList);

                int transformIndex = 0;
                int elementIndex = 0;

                foreach (var list in lists)
                {
                    foreach (var i in list)
                        transform[transformIndex++] = (elementIndex, i);
                    elementIndex += list.Count;
                }
            }

            yield return new Row<T>(items, transform.Select(x => x.Item2).ToArray());

            for (; ; )
            {
                // Reference: E. W. Dijkstra, A Discipline of Programming, Prentice-Hall, 1997
                // Find the largest partition from the back that is in decreasing (non-increasing) order.
                int decreasingPart;
                for (decreasingPart = length - 2;
                    decreasingPart >= 0 && transform[decreasingPart].First >= transform[decreasingPart + 1].First;
                    --decreasingPart)
                {
                }

                // The whole sequence is in decreasing order, finished.
                if (decreasingPart < 0)
                    yield break;

                // Find the smallest element in the decreasing partition that is 
                // greater than (or equal to) the item in front of the decreasing partition.
                int greater;
                for (greater = length - 1;
                    greater > decreasingPart && transform[decreasingPart].First >= transform[greater].First;
                    greater--)
                {
                }

                // Swap the two.
                MathEx.Swap(ref transform[decreasingPart], ref transform[greater]);

                // Reverse the decreasing partition.
                Array.Reverse(transform, decreasingPart + 1, length - decreasingPart - 1);

                yield return new Row<T>(items, transform.Select(x => x.Item2).ToArray());
            }
        }
    }
}
