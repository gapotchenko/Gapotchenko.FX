// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Math.Intervals.Properties;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Math.Intervals;

static class IntervalParser
{
    public static IntervalModel<T> Parse<T>(ReadOnlySpan<char> input, IFormatProvider? provider) =>
        ParseCore<T>(input, provider, true) ??
        throw new FormatException(Resources.IntervalHasInvalidFormat);

    public static IntervalModel<T>? TryParse<T>(ReadOnlySpan<char> input, IFormatProvider? provider) =>
        ParseCore<T>(input, provider, false);

    static IntervalModel<T>? ParseCore<T>(ReadOnlySpan<char> input, IFormatProvider? provider, bool throwOnError)
    {
        var type = typeof(T);
        var typeConverter = TypeDescriptor.GetConverter(type);
        if (!typeConverter.CanConvertFrom(typeof(string)))
        {
            throw new NotSupportedException(
                string.Format(
                    "Type '{0}' does not support conversion from string.",
                    type));
        }

        if (ParseModel<T>(input, typeConverter, GetCultureInfo(provider), throwOnError) is { } model)
        {
            if (!ValidateModel(model, throwOnError))
                return null;

            return model;
        }
        else
        {
            return null;
        }
    }

    static CultureInfo? GetCultureInfo(IFormatProvider? provider)
    {
        switch (provider)
        {
            case null:
                return null;

            case CultureInfo cultureInfo:
                return cultureInfo;

            case NumberFormatInfo numberFormatInfo:
                if (numberFormatInfo == NumberFormatInfo.InvariantInfo)
                    return CultureInfo.InvariantCulture;
                else if (numberFormatInfo == NumberFormatInfo.CurrentInfo)
                    return CultureInfo.CurrentCulture;
                break;
        }

        throw new ArgumentException(
            "The specified format provider does not provide culture information.",
            nameof(provider));
    }

    static IntervalModel<T>? ParseModel<T>(ReadOnlySpan<char> s, TypeConverter typeConverter, CultureInfo? culture, bool throwOnError)
    {
        switch (s)
        {
            // Empty interval
            case "∅" or "{}":
                {
                    var boundary = IntervalBoundary.Empty<T>();
                    return new(boundary, boundary);
                }

            // A set form of the interval
            case ['{', .. var token, '}']:
                {
                    token = token.Trim();
                    if (token.IsEmpty)
                    {
                        // Empty set => empty interval.
                        var boundary = IntervalBoundary.Empty<T>();
                        return new(boundary, boundary);
                    }
                    else
                    {
                        // A set of one => degenerate interval.
                        var value = ParseValue<T>(token, typeConverter, culture, throwOnError);
                        if (!value.HasValue)
                            return null;
                        var boundary = IntervalBoundary.Inclusive(value.Value);
                        return new(boundary, boundary);
                    }
                }

            // Generic interval form
            case [('[' or '(') and var lbt, .. var token, (']' or ')') and var rbt]:
                return ParseIntervalForm<T>(lbt, ref token, rbt, typeConverter, culture, throwOnError);

            // European regional form
            case [('[' or ']') and var lbt, .. var token, (']' or '[') and var rbt]:
                return ParseIntervalForm<T>(lbt, ref token, rbt, typeConverter, culture, throwOnError);

            default:
                return null;
        }
    }

    static IntervalModel<T>? ParseIntervalForm<T>(char lbt, ref ReadOnlySpan<char> token, char rbt, TypeConverter typeConverter, CultureInfo? culture, bool throwOnError)
    {
        // Trim the token to reduce the total number of characters to work on.
        token = token.Trim();

        // Separator
        int j = token.IndexOf(';');
        if (j == -1)
        {
            j = token.IndexOf(',');
            if (j == -1)
                return throwOnError ? throw new FormatException("Interval separator not found.") : null;
        }

        // Left boundary
        var leftToken = token[..j].TrimEnd();
        var leftKind = ParseBoundaryKind(lbt, false);
        var left = ParseBoundary<T>(leftToken, leftKind, typeConverter, culture, throwOnError);
        if (!left.HasValue)
            return null;

        // Right boundary
        var rightToken = token[(j + 1)..].TrimStart();
        var rightKind = ParseBoundaryKind(rbt, true);
        var right = ParseBoundary<T>(rightToken, rightKind, typeConverter, culture, throwOnError);
        if (!right.HasValue)
            return null;

        return new(left.Value, right.Value);
    }

    static IntervalBoundaryKind ParseBoundaryKind(char c, bool end)
    {
        return
            (c, end) switch
            {
                // Generic interval form
                ('[', false) or (']', true) => IntervalBoundaryKind.Inclusive,
                ('(' or ')', _) => IntervalBoundaryKind.Exclusive,

                // European regional format
                (']', false) or ('[', true) => IntervalBoundaryKind.Exclusive,

                _ => throw new SwitchExpressionException()
            };
    }

    static IntervalBoundary<T>? ParseBoundary<T>(
        ReadOnlySpan<char> s,
        IntervalBoundaryKind kind,
        TypeConverter typeConverter,
        CultureInfo? culture,
        bool throwOnError)
    {
        if (s.Length is > 0 and <= 4 and var length)
        {
            Span<char> ns = stackalloc char[length];
            int j = s.ToLowerInvariant(ns);
            if (j != -1)
            {
                ns = ns[..j];
                switch (ns)
                {
                    case "∞" or "+∞":
                    case "inf" or "+inf":
                        if (!ValidateInfiniteBoundary(kind, throwOnError))
                            return null;
                        return IntervalBoundary.PositiveInfinity<T>();

                    case "-∞":
                    case "-inf":
                        if (!ValidateInfiniteBoundary(kind, throwOnError))
                            return null;
                        return IntervalBoundary.NegativeInfinity<T>();
                }

                static bool ValidateInfiniteBoundary(IntervalBoundaryKind kind, bool throwOnError)
                {
                    if (kind != IntervalBoundaryKind.Exclusive)
                        return throwOnError ? throw new FormatException("An infinite interval boundary must be exclusive.") : false;
                    return true;
                }
            }
        }

        var value = ParseValue<T>(s, typeConverter, culture, throwOnError);
        if (!value.HasValue)
            return null;

        return kind switch
        {
            IntervalBoundaryKind.Exclusive => IntervalBoundary.Exclusive(value.Value),
            IntervalBoundaryKind.Inclusive => IntervalBoundary.Inclusive(value.Value),
            _ => throw new SwitchExpressionException(kind)
        };
    }

    static Optional<T> ParseValue<T>(ReadOnlySpan<char> s, TypeConverter typeConverter, CultureInfo? culture, bool throwOnError)
    {
        try
        {
            return (T)typeConverter.ConvertFromString(null, culture, s.ToString())!;
        }
        catch (Exception e) when (!e.IsControlFlowException())
        {
            return throwOnError ? throw new FormatException("Invalid interval boundary value.", e) : default;
        }
    }

    static bool ValidateModel<T>(in IntervalModel<T> model, bool throwOnError)
    {
        var (message, _) = IntervalEngine.VerifyBoundaries(model.From, model.To, throwOnError, null, null);
        if (message != null)
            return throwOnError ? throw new FormatException(message) : false;

        return true;
    }
}
