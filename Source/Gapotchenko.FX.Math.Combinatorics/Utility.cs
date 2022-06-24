using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Combinatorics;

static class Utility
{
    public static bool IsSet<T>(IEnumerable<T> sequence) =>
#if TFF_IREADONLYSET
        sequence is IReadOnlySet<T> ||
#endif
        sequence is ISet<T>;

    public static bool IsCompatibleSet<T>(IEnumerable<T> sequence, IEqualityComparer<T>? comparer) =>
        sequence switch
        {
            HashSet<T> hs => Empty.Nullify(hs.Comparer) == Empty.Nullify(comparer),
            _ => false
        };
}
