using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections;

static class ExceptionHelper
{
    public static void ThrowIfArgumentIsNegative(int value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(value, parameterName);
#else
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                string.Format(
                    "'{0}' must be a non-negative value.",
                    parameterName));
        }
#endif
    }

    /// <summary>
    /// Ensures that <paramref name="index"/> is non-negative and less than <paramref name="count"/>. 
    /// </summary>
    public static void ValidateIndexArgumentRange(int index, int count, [CallerArgumentExpression(nameof(index))] string? parameterName = null)
    {
        if (index < 0 || index >= count)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
    }

    /// <summary>
    /// Ensures that <paramref name="index"/> is non-negative and not greater than <paramref name="count"/>. 
    /// </summary>
    public static void ValidateIndexArgumentBounds(int index, int count, [CallerArgumentExpression(nameof(index))] string? parameterName = null)
    {
        if (index < 0 || index > count)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Index must be within the bounds of the collection.");
        }
    }

    // Allow nulls for reference types and Nullable<U>, but not for value types.
    // Aggressively inline so the JIT evaluates the if in place and either drops
    // the call altogether or leaves the body as is.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidateNullArgumentLegality<T>(
        object? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
        if (!(default(T) == null) && value == null)
            throw new ArgumentNullException(parameterName);
    }

    public static Exception CreateInvalidArgumentTypeException(
        object? actualValue,
        Type expectedType,
        [CallerArgumentExpression(nameof(actualValue))] string? parameterName = null) =>
        new ArgumentException(
            string.Format(
                "The value '{0}' is not of type '{1}' and cannot be used in this generic collection.",
                actualValue,
                expectedType),
            parameterName);
}
