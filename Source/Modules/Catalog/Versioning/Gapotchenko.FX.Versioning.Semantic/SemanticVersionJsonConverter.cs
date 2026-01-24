// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if TFF_JSON

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gapotchenko.FX.Versioning;

sealed class SemanticVersionJsonConverter : JsonConverter<SemanticVersion>
{
    public override SemanticVersion? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("SemanticVersion must be a string.");

        string value = reader.GetString()!;
        try
        {
            return SemanticVersion.Parse(value);
        }
        catch (FormatException e)
        {
            throw new JsonException($"Invalid semantic version '{value}'.", e);
        }
    }

    public override void Write(Utf8JsonWriter writer, SemanticVersion value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

#endif
