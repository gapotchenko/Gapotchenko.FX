// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial class IntervalEqualityComparer<T>
{
    internal static class DefaultComparer
    {
        public static IntervalEqualityComparer<T> Instance { get; } = new GenericComparer(null);
    }
}
