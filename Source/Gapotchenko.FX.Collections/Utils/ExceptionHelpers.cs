// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections.Utils;

static class ExceptionHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfArgumentIsNull(object? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfArrayArgumentIsMultiDimensional(Array array, [CallerArgumentExpression(nameof(array))] string? parameterName = null)
    {
        if (array.Rank != 1)
            throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", parameterName);
    }

    /// <summary>
    /// Ensures that <paramref name="index"/> is non-negative and less than <paramref name="size"/>. 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    /// <summary>
    /// Ensures that <paramref name="index"/> is non-negative and not greater than <paramref name="size"/>. 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidateIndexArgumentBounds(int index, int size, [CallerArgumentExpression(nameof(index))] string? parameterName = null)
    {
        Debug.Assert(size >= 0);

        if ((uint)index > (uint)size)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Index must be within the bounds of the collection.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ValidateIndexAndCountArgumentsRange(
        int index, int count, int size,
        [CallerArgumentExpression(nameof(index))] string? indexParameterName = null,
        [CallerArgumentExpression(nameof(count))] string? countParameterName = null)
    {
        ThrowIfArgumentIsNegative(index, indexParameterName);
        ThrowIfArgumentIsNegative(count, countParameterName);

        if (count > size - index) // overflow-safe equivalent of "index + count > size"
            throw new ArgumentException("Count is greater than the number of elements from index to the end of the collection.");
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

    public static Exception CreateIncompatibleArrayTypeException() =>
        new ArgumentException("Target array type is not compatible with the type of items in the collection.");

    public static Exception CreateEnumeratedCollectionWasModifiedException() =>
        new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

    public static Exception CreateEmptyCollectionException() =>
        new InvalidOperationException("The collection is empty.");
}
