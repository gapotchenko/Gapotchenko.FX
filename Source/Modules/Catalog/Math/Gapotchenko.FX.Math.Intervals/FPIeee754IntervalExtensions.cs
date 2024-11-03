// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Math.Intervals;

#if TFF_STATIC_INTERFACE

/// <summary>
/// Extension methods for <see cref="IInterval{T}"/> that work on <see cref="IFloatingPointIeee754{TSelf}"/> types.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class FPIeee754IntervalExtensions
{
    // This is a separate class to allow automatic overload selection.

    /// <inheritdoc cref="IntervalExtensions.Clamp(IInterval{int}, int)"/>
    /// <typeparam name="T">The type of value to clamp.</typeparam>
    public static Optional<T> Clamp<T>(this IInterval<T> interval, T value)
        where T : IFloatingPointIeee754<T> =>
        Clamp<T, IInterval<T>>(interval, value);

    /// <inheritdoc cref="Clamp{T}(IInterval{T}, T)"/>
    /// <typeparam name="TValue">The type of value to clamp.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static Optional<TValue> Clamp<TValue, TInterval>(this TInterval interval, TValue value)
        where TInterval : IInterval<TValue>
        where TValue : IFloatingPointIeee754<TValue> =>
        IntervalEngine.Clamp(interval, value, TValue.BitIncrement, TValue.BitDecrement);
}

#endif
