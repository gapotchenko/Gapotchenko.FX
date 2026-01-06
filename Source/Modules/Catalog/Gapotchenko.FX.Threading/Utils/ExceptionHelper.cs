// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Gapotchenko.FX.Threading.Utils;

[StackTraceHidden]
static class ExceptionHelper
{
    public static void ThrowIfThisArgumentIsNull([NotNull] object? value)
    {
#pragma warning disable CA2201 // Do not raise reserved exception types
        if (value is null)
            throw new NullReferenceException();
#pragma warning restore CA2201
    }

    public static void ValidateTimeoutArgument(
        int value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value < Timeout.Infinite)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "The value needs to be either -1 (signifying an infinite timeout), 0, or a positive integer.");
        }
    }

    public static void ValidateTimeoutArgument(
        TimeSpan value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (!IsValidTimeout(value))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "The value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0, or a positive integer less than or equal to the maximum allowed timer duration.");
        }
    }

    public static bool IsValidTimeout(TimeSpan value)
    {
        long milliseconds = (long)value.TotalMilliseconds;
        return milliseconds >= -1 && milliseconds < uint.MaxValue;
    }

#if !(NETCOREAPP2_2_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#pragma warning disable CS8763 // A method marked [DoesNotReturn] should not return.
#endif
    [DoesNotReturn]
    public static void Rethrow(Exception exception)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        ExceptionDispatchInfo.Throw(exception);
#else
        ExceptionDispatchInfo.Capture(exception).Throw();
#endif
    }
#pragma warning restore CS8763 // A method marked [DoesNotReturn] should not return.
}
