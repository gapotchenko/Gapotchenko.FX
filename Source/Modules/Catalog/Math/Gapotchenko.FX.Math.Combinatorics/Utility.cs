// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using Gapotchenko.FX.Collections.Generic.Kits;

namespace Gapotchenko.FX.Math.Combinatorics;

static class Utility
{
    public static bool IsSet<T>(IEnumerable<T> sequence) =>
#if TFF_IREADONLYSET
        sequence is IReadOnlySet<T> ||
#endif
        sequence is ISet<T>;

    public static bool IsCompatibleSet<T>(IEnumerable<T> sequence, IEqualityComparer<T>? comparer)
    {
        var sequenceComparer =
            sequence switch
            {
                ReadOnlySetBase<T> rosb => rosb.Comparer,
                HashSet<T> hs => hs.Comparer,
                _ => null
            };

        return
            sequenceComparer != null &&
            IsCompatibleComparer(sequenceComparer, comparer);
    }

    public static bool IsCompatibleComparer<T>(IEqualityComparer<T>? a, IEqualityComparer<T>? b) =>
        Empty.Nullify(a) == Empty.Nullify(b);
}
