// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if TFF_JSON

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gapotchenko.FX.Math.Intervals;

sealed class IntervalJsonConverterFactory : IntervalJsonConverterFactoryBase
{
    public IntervalJsonConverterFactory() :
        base(typeof(Interval<>), typeof(IntervalJsonConverter<>))
    {
    }

    sealed class IntervalJsonConverter<T> : BaseIntervalJsonConverter<Interval<T>>
    {
        [return: NotNullIfNotNull(nameof(value))]
        protected override Interval<T> ParseInterval(string value, IFormatProvider provider) => Interval.Parse<T>(value, provider);
    }
}

sealed class ValueIntervalJsonConverterFactory : IntervalJsonConverterFactoryBase
{
    public ValueIntervalJsonConverterFactory() :
        base(typeof(ValueInterval<>), typeof(ValueIntervalJsonConverter<>))
    {
    }

    sealed class ValueIntervalJsonConverter<T> : BaseIntervalJsonConverter<ValueInterval<T>>
         where T : IEquatable<T>?, IComparable<T>?
    {
        [return: NotNullIfNotNull(nameof(value))]
        protected override ValueInterval<T> ParseInterval(string value, IFormatProvider provider) => ValueInterval.Parse<T>(value, provider);
    }
}

abstract class IntervalJsonConverterFactoryBase(Type intervalType, Type intervalConverterType) : JsonConverterFactory
{
    public sealed override bool CanConvert(Type typeToConvert)
    {
        return
            typeToConvert.IsGenericType &&
            typeToConvert.GetGenericTypeDefinition() == intervalType;
    }

    public sealed override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var t = typeToConvert.GetGenericArguments()[0];
        var converterType = intervalConverterType.MakeGenericType(t);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

abstract class BaseIntervalJsonConverter<TInterval> : JsonConverter<TInterval>
    where TInterval : IFormattable
{
    public sealed override void Write(Utf8JsonWriter writer, TInterval value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(null, CultureInfo.InvariantCulture));
    }

    public sealed override TInterval? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Interval must be a string.");

        string value = reader.GetString()!;
        try
        {
            return ParseInterval(value, CultureInfo.InvariantCulture);
        }
        catch (FormatException e)
        {
            throw new JsonException($"Invalid interval '{value}'.", e);
        }
    }

    [return: NotNullIfNotNull(nameof(value))]
    protected abstract TInterval ParseInterval(string value, IFormatProvider provider);
}

#endif
