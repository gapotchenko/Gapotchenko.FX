// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using Gapotchenko.FX.Collections.Properties;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#if NET8_0_OR_GREATER
using static System.ArgumentOutOfRangeException;
#else
using static Gapotchenko.FX.Collections.Utils.ThrowPolyfills;
#endif

namespace Gapotchenko.FX.Collections.Utils;

[StackTraceHidden]
static class ExceptionHelper
{
    public static void ThrowIfThisIsNull([NotNull] object? @this)
    {
        if (@this is null)
#pragma warning disable CA2201 // Do not raise reserved exception types
            throw new NullReferenceException();
#pragma warning restore CA2201 // Do not raise reserved exception types
    }

    /// <summary>
    /// Ensures that <paramref name="index"/> is non-negative and less than <paramref name="size"/>. 
    /// </summary>
    public static void ValidateIndexArgumentRange(int index, int size, [CallerArgumentExpression(nameof(index))] string? parameterName = null)
    {
        Debug.Assert(size >= 0);

        if ((uint)index >= (uint)size)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
    }

    [DoesNotReturn]
    public static void ThrowArgumentException_ArrayPlusOffTooSmall() => ThrowHelper.ThrowArrayPlusOffTooSmall();

    /// <summary>
    /// Ensures that <paramref name="index"/> is non-negative and not greater than <paramref name="size"/>. 
    /// </summary>
    public static void ValidateIndexArgumentBounds(
        int index,
        int size,
        [CallerArgumentExpression(nameof(index))] string? indexParameterName = null)
    {
        Debug.Assert(size >= 0);

        if ((uint)index > (uint)size)
        {
            throw new ArgumentOutOfRangeException(
                indexParameterName,
                "Index must be within the bounds of the collection.");
        }
    }

    public static void ValidateIndexAndCountArgumentsRange(
        int index,
        int count,
        int size,
        [CallerArgumentExpression(nameof(index))] string? indexParameterName = null,
        [CallerArgumentExpression(nameof(count))] string? countParameterName = null)
    {
        ThrowIfNegative(index, indexParameterName);
        ThrowIfNegative(count, countParameterName);

        if (count > size - index) // overflow-safe equivalent of "index + count > size"
            throw new ArgumentException("Count is greater than the number of elements from index to the end of the collection.");
    }

    // Allow nulls for reference types and Nullable<U>, but not for value types.
    // Aggressively inline so the JIT evaluates the if in place and either drops
    // the call altogether or leaves the body as is.
    public static void ValidateNullArgumentLegality<T>(
        object? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
        if (!(default(T) == null) && value == null)
            throw new ArgumentNullException(parameterName);
    }

    public static Exception CreateIncompatibleArrayTypeArgumentException(string? parameterName = null) =>
        new ArgumentException(
            Resources.Argument_IncompatibleArrayType,
            parameterName);

    public static InvalidOperationException CreateEmptyCollectionException() =>
        new("The collection is empty.");

    public static Exception CreateKeyNotFound(object? key) =>
        new KeyNotFoundException(string.Format(Resources.Arg_KeyNotFoundWithKey, key));

    public static InvalidOperationException CreateEnumerationEitherNotStarterOrFinishedException() =>
        new("Enumeration has either not started or has already finished.");

    public static Exception CreateWrongType(
        object? actualValue,
        Type expectedType,
        [CallerArgumentExpression(nameof(actualValue))] string? actualValueParameterName = null) =>
        new ArgumentException(
            string.Format(Resources.Arg_WrongType, actualValue, expectedType),
            actualValueParameterName);
}
