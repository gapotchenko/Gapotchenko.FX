using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Combinatorics.Tests
{
    static class Utility
    {
        public static IDictionary<T, int> ToMultiset<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
            where T : notnull
        {
            var multiset = new Dictionary<T, int>(comparer);

            foreach (var i in source)
            {
                if (multiset.TryGetValue(i, out var count))
                    multiset[i] = count + 1;
                else
                    multiset.Add(i, 1);
            }

            return multiset;
        }
    }
}
