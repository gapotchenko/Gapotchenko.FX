// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Math.Intervals;

#if TFF_STATIC_INTERFACE

/// <summary>
/// Extension methods for <see cref="IInterval{T}"/> that work on <see cref="IBinaryInteger{TSelf}"/> types.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class BinaryIntegerIntervalExtensions
{
    /// <inheritdoc cref="IntervalExtensions.Clamp(IInterval{int}, int)"/>
    /// <typeparam name="T">The type of value to clamp.</typeparam>
    public static Optional<T> Clamp<T>(this IInterval<T> interval, T value)
        where T : IBinaryInteger<T> =>
        Clamp<T, IInterval<T>>(interval, value);

    /// <inheritdoc cref="Clamp{T}(IInterval{T}, T)"/>
    /// <typeparam name="TValue">The type of value to clamp.</typeparam>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method, so hide it to not drain cognitive energy of a user
    public static Optional<TValue> Clamp<TValue, TInterval>(this TInterval interval, TValue value)
        where TInterval : IInterval<TValue>
        where TValue : IBinaryInteger<TValue> =>
        IntervalEngine.Clamp(interval, value, x => x + TValue.One, x => x - TValue.One);
}

#endif
