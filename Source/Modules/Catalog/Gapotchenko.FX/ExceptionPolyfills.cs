// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NET8_0_OR_GREATER
#define TFF_EXCEPTION_THROWIF
#endif

using Gapotchenko.FX.Properties;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#pragma warning disable CA1708 // Identifiers should differ by more than case

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill methods for classes provided by BCL and inherited from <see cref="Exception"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExceptionPolyfills
{
    /// <summary>
    /// Provides polyfill methods for <see cref="ArgumentNullException"/> class.
    /// </summary>
    extension(ArgumentNullException)
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The reference type argument to validate as non-null.</param>
        /// <param name="paramName">
        /// The name of the parameter with which <paramref name="argument"/> corresponds.
        /// If you omit this parameter, the name of <paramref name="argument"/> is used.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/>.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentNullException.ThrowIfNull(argument, paramName);
#else
            if (argument is null)
                throw new ArgumentNullException(paramName);
#endif
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The pointer argument to validate as non-null</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/>.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static unsafe void ThrowIfNull([NotNull] void* argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentNullException.ThrowIfNull(argument, paramName);
#else
            if (argument is null)
                throw new ArgumentNullException(paramName);
#endif
        }
    }

    /// <summary>
    /// Provides polyfill methods for <see cref="ArgumentOutOfRangeException"/> class.
    /// </summary>
    extension(ArgumentOutOfRangeException)
    {
        #region ThrowIfZero

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.
        /// </summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is zero.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfZero(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfZero(value, paramName);
#else
            if (value == 0)
                ThrowZero(value, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfZero(int, string?)"/>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfZero(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfZero(value, paramName);
#else
            if (value == 0)
                ThrowZero(value, paramName);
#endif
        }

#if !TFF_EXCEPTION_THROWIF
        [DoesNotReturn]
        static void ThrowZero(object value, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeNonZero, paramName, value));
#endif

        #endregion

        #region ThrowIfNegative

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.
        /// </summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
#else
            if (value < 0)
                ThrowNegative(value, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfNegative(int, string?)"/>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNegative(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
#else
            if (value < 0)
                ThrowNegative(value, paramName);
#endif
        }

#if !TFF_EXCEPTION_THROWIF
        [DoesNotReturn]
        static void ThrowNegative(object value, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeNonNegative, paramName, value));
#endif

        #endregion

        #region ThrowIfNegativeOrZero

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative or zero.
        /// </summary>
        /// <param name="value">The argument to validate as non-zero or non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative or zero.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNegativeOrZero(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
#else
            if (value <= 0)
                ThrowNegativeOrZero(value, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfNegativeOrZero(int, string?)"/>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNegativeOrZero(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
#else
            if (value <= 0)
                ThrowNegativeOrZero(value, paramName);
#endif
        }

#if !TFF_EXCEPTION_THROWIF
        [DoesNotReturn]
        static void ThrowNegativeOrZero(object value, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeNonNegativeNonZero, paramName, value));
#endif

        #endregion

        #region ThrowIfGreaterThan

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as less than or equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than <paramref name="other"/>.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfGreaterThan(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
#else
            if (value > other)
                ThrowGreater(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfGreaterThan(int, int, string?)"/>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfGreaterThan(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
#else
            if (value > other)
                ThrowGreater(value, other, paramName);
#endif
        }

#if !TFF_EXCEPTION_THROWIF
        [DoesNotReturn]
        static void ThrowGreater(object value, object other, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeLessOrEqual, paramName, value, other));
#endif

        #endregion

        #region ThrowIfGreaterThanOrEqual

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as less than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is greater than or equal to <paramref name="other"/>.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfGreaterThanOrEqual(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);
#else
            if (value >= other)
                ThrowGreaterEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfGreaterThanOrEqual(int, int, string?)"/>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfGreaterThanOrEqual(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);
#else
            if (value >= other)
                ThrowGreaterEqual(value, other, paramName);
#endif
        }

#if !TFF_EXCEPTION_THROWIF
        [DoesNotReturn]
        static void ThrowGreaterEqual(object value, object other, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeLess, paramName, value, other));
#endif

        #endregion

        #region ThrowIfLessThan

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as greater than or equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than <paramref name="other"/>.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfLessThan(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
#else
            if (value < other)
                ThrowLess(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfLessThan(int, int, string?)"/>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfLessThan(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
#else
            if (value < other)
                ThrowLess(value, other, paramName);
#endif
        }

#if !TFF_EXCEPTION_THROWIF
        [DoesNotReturn]
        static void ThrowLess(object value, object other, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeGreaterOrEqual, paramName, value, other));
#endif

        #endregion

        #region ThrowIfLessThanOrEqual

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as greater than to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is less than or equal to <paramref name="other"/>.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfLessThanOrEqual(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);
#else
            if (value <= other)
                ThrowLessEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfLessThanOrEqual(int, int, string?)"/>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfLessThanOrEqual(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);
#else
            if (value <= other)
                ThrowLessEqual(value, other, paramName);
#endif
        }

#if !TFF_EXCEPTION_THROWIF
        [DoesNotReturn]
        static void ThrowLessEqual(object value, object other, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeGreater, paramName, value, other));
#endif

        #endregion

        #region ThrowIfEqual

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is equal to <paramref name="other"/>.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfEqual(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfEqual(value, other, paramName);
#else
            if (value == other)
                ThrowEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfEqual(int, int, string?)"/>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfEqual(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfEqual(value, other, paramName);
#else
            if (value == other)
                ThrowEqual(value, other, paramName);
#endif
        }

#if !TFF_EXCEPTION_THROWIF
        [DoesNotReturn]
        static void ThrowEqual(object? value, object? other, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeNotEqual, paramName, value ?? "null", other ?? "null"));
#endif

        #endregion

        #region ThrowIfNotEqual

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="value">The argument to validate as not equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is equal to <paramref name="other"/>.</exception>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNotEqual(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);
#else
            if (value != other)
                ThrowNotEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfNotEqual(int, int, string?)"/>
#if TFF_EXCEPTION_THROWIF
        [StackTraceHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNotEqual(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if TFF_EXCEPTION_THROWIF
            ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);
#else
            if (value != other)
                ThrowNotEqual(value, other, paramName);
#endif
        }

#if !TFF_EXCEPTION_THROWIF
        [DoesNotReturn]
        static void ThrowNotEqual(object? value, object? other, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeEqual, paramName, value ?? "null", other ?? "null"));
#endif

        #endregion
    }
}
