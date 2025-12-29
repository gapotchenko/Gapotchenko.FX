// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// The internal object model of an interval.
/// </summary>
readonly struct IntervalModel<T>(in IntervalBoundary<T> from, in IntervalBoundary<T> to) :
    IIntervalModel<T>
{
    public IntervalBoundary<T> From { get; init; } = from;

    public IntervalBoundary<T> To { get; init; } = to;
}
