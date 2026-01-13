// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Provides the abstraction interface for an continuous interval of untyped values.
/// </summary>
public interface IInterval : IIntervalOperations, IFormattable
{
}

/// <summary>
/// Provides the abstraction interface for a continuous interval.
/// </summary>
/// <typeparam name="T">The type of interval value.</typeparam>
public interface IInterval<T> : IInterval, IIntervalOperations<T>, IFormattable
{
}
