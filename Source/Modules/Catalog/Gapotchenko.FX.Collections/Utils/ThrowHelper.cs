using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections.Utils;

[StackTraceHidden]
static class ThrowHelper
{

#if !NET8_0_OR_GREATER

    public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
            ThrowNull(paramName);
    }

    public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value < 0)
            ThrowNegative(value, paramName);
    }

    public static void ThrowIfGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) > 0)
            ThrowGreater(value, other, paramName);
    }

    public static void ThrowIfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) < 0)
            ThrowLess(value, other, paramName);
    }

    [DoesNotReturn]
    static void ThrowNull(string? paramName) =>
           throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    static void ThrowNegative(object value, string? paramName) =>
        throw new ArgumentOutOfRangeException(
            paramName,
            value,
            string.Format(
                "{0} ('{1}') must be a non-negative value.",
                paramName,
                value));

    [DoesNotReturn]
    static void ThrowGreater(object value, object other, string? paramName) =>
        throw new ArgumentOutOfRangeException(
            paramName,
            value,
            string.Format(
                "{0} ('{1}') must be less than or equal to '{2}'.",
                paramName,
                value,
                other));

    [DoesNotReturn]
    static void ThrowLess(object value, object other, string? paramName) =>
        throw new ArgumentOutOfRangeException(
            paramName,
            value,
            string.Format(
                "{0} ('{1}') must be greater than or equal to '{2}'.",
                paramName,
                value,
                other));

#endif

}
