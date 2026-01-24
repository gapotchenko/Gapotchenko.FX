// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if TFF_JSON
using System.Text.Json.Serialization;
#endif

namespace Gapotchenko.FX.Math.Intervals;

[TypeConverter(typeof(IntervalTypeConverter))]
#if TFF_JSON
[JsonConverter(typeof(IntervalJsonConverterFactory))]
#endif
partial record Interval<T>
{
}
