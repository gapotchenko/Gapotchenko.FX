// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial class ValueInterval
{
    /// <summary>
    /// Converts the specified interval to <see cref="ValueInterval{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of the interval values.</typeparam>
    /// <param name="interval">The interval to convert.</param>
    /// <returns>The converted <see cref="ValueInterval{T}"/> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="interval"/> is <see langword="null"/>.</exception>
    public static ValueInterval<T> From<T>(IInterval<T> interval)
        where T : IEquatable<T>?, IComparable<T>?
    {
        ArgumentNullException.ThrowIfNull(interval);

        return new(interval.From, interval.To);
    }

    /// <inheritdoc cref="From{T}(IInterval{T})" />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("ValueInterval<T> type does not need a conversion from itself.")]
    public static ValueInterval<T> From<T>(in ValueInterval<T> interval)
        where T : IEquatable<T>?, IComparable<T>?
        => interval;
}
