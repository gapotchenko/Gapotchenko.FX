// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Provides the interface for continuous interval abstraction.
/// </summary>
/// <typeparam name="T">The type of interval value.</typeparam>
public interface IInterval<T> : IIntervalOperations<T>
{
}
