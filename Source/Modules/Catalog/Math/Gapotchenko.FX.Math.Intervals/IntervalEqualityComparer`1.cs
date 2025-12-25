// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Provides a base class for implementations of the <see cref="IEqualityComparer{T}"/> generic interface
/// for <see cref="Interval{T}"/> type.
/// </summary>
/// <typeparam name="T">The type of interval values.</typeparam>
public abstract partial class IntervalEqualityComparer<T> : IEqualityComparer<Interval<T>>, IEqualityComparer<IInterval<T>>
{
    /// <inheritdoc/>
    public bool Equals(Interval<T>? x, Interval<T>? y) => Equals((IInterval<T>?)x, y);

    /// <inheritdoc/>
    public int GetHashCode(Interval<T> obj) => throw new NotImplementedException();

    /// <inheritdoc/>
    public abstract bool Equals(IInterval<T>? x, IInterval<T>? y);

    /// <inheritdoc/>
    public abstract int GetHashCode(IInterval<T> obj);
}
