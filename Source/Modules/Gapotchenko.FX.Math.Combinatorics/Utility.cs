// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using Gapotchenko.FX.Collections.Generic.Kit;

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
            Empty.Nullify(sequenceComparer) == Empty.Nullify(comparer);
    }
}
