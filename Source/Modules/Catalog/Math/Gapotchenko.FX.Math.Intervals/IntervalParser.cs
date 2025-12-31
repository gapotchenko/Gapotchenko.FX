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
            case ['{', .. var x, '}']:
                {
                    x = x.Trim();
                    if (x.IsEmpty)
                    {
                        // Empty set => empty interval.
                        var boundary = IntervalBoundary.Empty<T>();
                        return new(boundary, boundary);
                    }
                    else
                    {
                        // A set of one => degenerate interval.
                        var value = ParseValue<T>(x, typeConverter, culture, throwOnError);
                        if (!value.HasValue)
                            return null;
                        var boundary = IntervalBoundary.Inclusive(value.Value);
                        return new(boundary, boundary);
                    }
                }

            // Generic interval form
            case [('[' or '(') and var lt, .. var mt, (']' or ')') and var rt]:
                {
                    // Separator
                    int j = mt.IndexOf(';');
                    if (j == -1)
                    {
                        j = mt.IndexOf(',');
                        if (j == -1)
                        {
                            if (throwOnError)
                                throw new FormatException("Interval boundary separator not found.");
                            else
                                return null;
                        }
                    }

                    // Left boundary
                    var ft = mt[..j].Trim();
                    var fk = ParseBoundaryKind(lt);
                    var from = ParseBoundary<T>(ft, fk, typeConverter, culture, throwOnError);
                    if (!from.HasValue)
                        return null;

                    // Right boundary
                    var tt = mt[(j + 1)..].Trim();
                    var tk = ParseBoundaryKind(rt);
                    var to = ParseBoundary<T>(tt, tk, typeConverter, culture, throwOnError);
                    if (!to.HasValue)
                        return null;

                    return new(from.Value, to.Value);
                }

            default:
                return null;
        }
    }

    static IntervalBoundaryKind ParseBoundaryKind(char c)
    {
        return c switch
        {
            '[' or ']' => IntervalBoundaryKind.Inclusive,
            '(' or ')' => IntervalBoundaryKind.Exclusive,
            _ => throw new SwitchExpressionException(c)
        };
    }

    static IntervalBoundary<T>? ParseBoundary<T>(
        ReadOnlySpan<char> s,
        IntervalBoundaryKind kind,
        TypeConverter typeConverter,
        CultureInfo? culture,
        bool throwOnError)
    {
        if (s.Length is >= 1 and <= 4)
        {
            Span<char> ns = stackalloc char[4];
            int j = s.ToLowerInvariant(ns);
            if (j != -1)
            {
                ns = ns[..j];
                switch (ns)
                {
                    case "∞" or "+∞":
                    case "inf" or "+inf":
                        return IntervalBoundary.PositiveInfinity<T>();

                    case "-∞":
                    case "-inf":
                        return IntervalBoundary.NegativeInfinity<T>();
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
            if (throwOnError)
                throw new FormatException("Invalid interval boundary value.", e);
            else
                return default;
        }
    }

    static bool ValidateModel<T>(in IntervalModel<T> model, bool throwOnError)
    {
        var (message, _) = IntervalEngine.VerifyBoundaries(model.From, model.To, throwOnError, null, null);
        if (message != null)
        {
            if (throwOnError)
                throw new FormatException(message);
            else
                return false;
        }

        return true;
    }
}
