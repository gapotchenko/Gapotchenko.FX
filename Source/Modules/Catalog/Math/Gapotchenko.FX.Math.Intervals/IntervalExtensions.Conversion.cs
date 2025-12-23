// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial class IntervalExtensions
{
    /// <summary>
    /// Converts an interval to an instance of <see cref="ValueInterval{T}"/> type.
    /// </summary>
    /// <typeparam name="TInterval">The type of the interval.</typeparam>
    /// <typeparam name="TValue">The type of the interval values.</typeparam>
    /// <param name="interval">The interval to convert.</param>
    /// <returns>The converted <see cref="ValueInterval{T}"/> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is <see langword="null"/>.</exception>
    public static ValueInterval<TValue> ToValueInterval<TInterval, TValue>(this TInterval interval)
        where TInterval : class, IInterval<TValue>
        where TValue : IEquatable<TValue>?, IComparable<TValue>?
    {
        ArgumentNullException.ThrowIfNull(interval);

        return new(interval.From, interval.To);
    }
}
