// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

#if TFF_JSON
using System.Text.Json.Serialization;
#endif

namespace Gapotchenko.FX.Math.Intervals;

#if TFF_JSON
[JsonConverter(typeof(ValueIntervalJsonConverterFactory))]
#endif
partial struct ValueInterval<T>
{
    /// <summary>
    /// Converts a specified <see cref="Interval{T}"/> instance to <see cref="ValueInterval{T}"/>.
    /// </summary>
    /// <param name="interval">The <see cref="Interval{T}"/> instance to convert.</param>
    public static implicit operator ValueInterval<T>(Interval<T> interval) =>
        interval is null ?
            default :
            new(interval.From, interval.To);

    /// <summary>
    /// Converts a specified <see cref="ValueInterval{T}"/> instance to <see cref="Interval{T}"/>.
    /// </summary>
    /// <param name="interval">The <see cref="ValueInterval{T}"/> instance to convert.</param>
    public static implicit operator Interval<T>(ValueInterval<T> interval) => new(interval.From, interval.To);
}
