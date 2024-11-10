// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

using Gapotchenko.FX.Collections.Properties;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections.Utils;

[StackTraceHidden]
static class ThrowHelper
{
    /// <summary>Throws an exception for a key not being found in the dictionary.</summary>
    [DoesNotReturn]
    public static void ThrowKeyNotFound(object? key) => throw (ExceptionHelper.CreateKeyNotFound(key));

    /// <summary>Throws an exception for trying to insert a duplicate key into the dictionary.</summary>
    [DoesNotReturn]
    public static void ThrowDuplicateKey(object? key, [CallerArgumentExpression(nameof(key))] string? parameterName = null) =>
    throw new ArgumentException(string.Format(Resources.Argument_AddingDuplicate, key), parameterName);

    /// <summary>Throws an exception when erroneous concurrent use of a collection is detected.</summary>
    [DoesNotReturn]
    public static void ThrowConcurrentOperation() => throw new InvalidOperationException(Resources.InvalidOperation_ConcurrentOperationsNotSupported);

    /// <summary>Throws an exception for an index being out of range.</summary>
    [DoesNotReturn]
    public static void ThrowIndexArgumentOutOfRange(string? parameterName = "index") => throw new ArgumentOutOfRangeException(parameterName);

    /// <summary>Throws an exception for a version check failing during enumeration.</summary>
    [DoesNotReturn]
    internal static void ThrowVersionCheckFailed() =>
        throw new InvalidOperationException(Resources.InvalidOperation_EnumFailedVersion);

    [DoesNotReturn]
    public static void ThrowArrayPlusOffTooSmall() => throw new ArgumentException(Resources.Arg_ArrayPlusOffTooSmall);

    public static void ThrowIfArrayIsMultiDimensional(Array array, [CallerArgumentExpression(nameof(array))] string? parameterName = null)
    {
        if (array.Rank != 1)
            throw new ArgumentException(Resources.Arg_RankMultiDimNotSupported, parameterName);
    }

    public static void ThrowIfArrayLowerBoundIsNotZero(Array array, [CallerArgumentExpression(nameof(array))] string? parameterName = null)
    {
        if (array.GetLowerBound(0) != 0)
            throw new ArgumentException(Resources.Arg_NonZeroLowerBound, parameterName);
    }
}
