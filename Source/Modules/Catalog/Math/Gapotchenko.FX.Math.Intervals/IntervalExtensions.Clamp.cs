// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

namespace Gapotchenko.FX.Math.Intervals;

partial class IntervalExtensions
{
    // We provide several overloads for widely used types to compensate for the lack of
    // System.Numerics.IBinaryInteger<T> interface in earlier .NET versions.

    #region Int32

    /// <summary>
    /// Returns a <paramref name="value"/> clamped to the range represented by the interval.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="value">The value to clamp.</param>
    /// <returns>
    /// A clamped <paramref name="value"/>,
    /// or <see cref="Optional{T}.None"/> if the interval is empty
    /// or the clamped value cannot be represented by the underlying type.
    /// </returns>
#if TFF_STATIC_INTERFACE
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Optional<int> Clamp(this IInterval<int> interval, int value) =>
        Clamp<IInterval<int>>(interval, value);

    /// <inheritdoc cref="Clamp(IInterval{int}, int)"/>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static Optional<int> Clamp<TInterval>(this TInterval interval, int value)
        where TInterval : IInterval<int> =>
        IntervalEngine.Clamp(interval, value, x => x + 1, x => x - 1);

    #endregion

    #region Int64

    /// <inheritdoc cref="Clamp(IInterval{int}, int)"/>
#if TFF_STATIC_INTERFACE
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Optional<long> Clamp(this IInterval<long> interval, long value) =>
        Clamp<IInterval<long>>(interval, value);

    /// <inheritdoc cref="Clamp(IInterval{long}, long)"/>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static Optional<long> Clamp<TInterval>(this TInterval interval, long value)
        where TInterval : IInterval<long> =>
        IntervalEngine.Clamp(interval, value, x => x + 1, x => x - 1);

    #endregion

    #region Single

    /// <inheritdoc cref="Clamp(IInterval{int}, int)"/>
#if TFF_STATIC_INTERFACE
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Optional<float> Clamp(this IInterval<float> interval, float value) =>
        Clamp<IInterval<float>>(interval, value);

    /// <inheritdoc cref="Clamp(IInterval{float}, float)"/>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static Optional<float> Clamp<TInterval>(this TInterval interval, float value)
        where TInterval : IInterval<float> =>
        IntervalEngine.Clamp(interval, value, MathEx.BitIncrement, MathEx.BitDecrement);

    #endregion

    #region Double

    /// <inheritdoc cref="Clamp(IInterval{int}, int)"/>
#if TFF_STATIC_INTERFACE
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Optional<double> Clamp(this IInterval<double> interval, double value) =>
        Clamp<IInterval<double>>(interval, value);

    /// <inheritdoc cref="Clamp(IInterval{double}, double)"/>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)] // this is an optimization method which is hidden to not drain cognitive energy of a user
    public static Optional<double> Clamp<TInterval>(this TInterval interval, double value)
        where TInterval : IInterval<double> =>
        IntervalEngine.Clamp(interval, value, MathEx.BitIncrement, MathEx.BitDecrement);

    #endregion
}
