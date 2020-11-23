using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Combinatorics
{
    static class Utility
    {
        public static IEnumerable<T> SelectExceptLast<T>(this IEnumerable<T> source, Func<T, T> selector)
        {
            Optional<T> slot = default;

            foreach (var item in source)
            {
                if (slot.HasValue)
                    yield return selector(slot.Value);

                slot = item;
            }

            // Flush the last item as is.
            if (slot.HasValue)
                yield return slot.Value;
        }
    }
}
