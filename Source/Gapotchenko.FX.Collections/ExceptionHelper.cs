using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections;

static class ExceptionHelper
{
    // Allow nulls for reference types and Nullable<U>, but not for value types.
    // Aggressively inline so the JIT evaluates the if in place and either drops
    // the call altogether or leaves the body as is.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidateNullArgumentLegality<T>(object? value, string parameterName)
    {
        // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
        if (!(default(T) == null) && value == null)
            throw new ArgumentNullException(parameterName);
    }

    public static Exception CreateInvalidArgumentTypeException(object? actualValue, Type expectedType, string parameterName) =>
        new ArgumentException(
            string.Format(
                "The value '{0}' is not of type '{1}' and cannot be used in this generic collection.",
                actualValue,
                expectedType),
            parameterName);
}
