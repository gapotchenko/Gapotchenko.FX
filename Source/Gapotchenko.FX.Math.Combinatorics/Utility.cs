using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Combinatorics
{
    static class Utility
    {
        public static IEqualityComparer<T>? NullifyDefault<T>(IEqualityComparer<T>? comparer)
        {
            if (comparer == null)
                return null;
            else if (comparer == EqualityComparer<T>.Default)
                return null;
            else
                return comparer;
        }

        public static bool IsSet<T>(IEnumerable<T> sequence) =>
#if TFF_IREADONLYSET
            sequence is IReadOnlySet<T> ||
#endif
            sequence is ISet<T>;

        public static bool IsCompatibleSet<T>(IEnumerable<T> sequence, IEqualityComparer<T>? comparer) =>
            sequence switch
            {
                HashSet<T> hs => NullifyDefault(hs.Comparer) == NullifyDefault(comparer),
                _ => false
            };
    }
}
