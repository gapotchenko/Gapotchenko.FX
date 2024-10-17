// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Tuples.Utils;

static class EqualityComparerExtensions
{
    public static int GetNullSafeHashCode<T>(this IEqualityComparer<T> comparer, T value) =>
        value is null ? 0 : comparer.GetHashCode(value);
}
