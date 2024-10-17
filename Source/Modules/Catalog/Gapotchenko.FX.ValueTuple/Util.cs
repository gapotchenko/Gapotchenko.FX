// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.Tuples;

static class Util
{
    public static int GetSafeHashCode<T>(IEqualityComparer<T> comparer, T value) =>
        value is null ?
            HashCode.Combine(0) :
            comparer.GetHashCode(value);
}
