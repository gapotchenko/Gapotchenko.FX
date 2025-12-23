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
    /// Converts the interval to <see cref="ValueInterval{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the interval values.</typeparam>
    /// <param name="interval">The interval to convert.</param>
    /// <returns>The converted <see cref="ValueInterval{T}"/> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is <see langword="null"/>.</exception>
    public static ValueInterval<T> ToValueInterval<T>(this IInterval<T> interval)
        where T : IEquatable<T>?, IComparable<T>?
    {
        ArgumentNullException.ThrowIfNull(interval);

        return new(interval.From, interval.To);
    }
}
