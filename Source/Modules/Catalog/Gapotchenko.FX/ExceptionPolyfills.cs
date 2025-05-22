// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Properties;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#pragma warning disable CA1708 // Identifiers should differ by more than case

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill extension methods for BCL classes derived from <see cref="Exception"/>.
/// </summary>
[StackTraceHidden]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExceptionPolyfills
{
    /// <summary>
    /// Provides polyfill extension methods for <see cref="ArgumentException"/> class.
    /// </summary>
    extension(ArgumentException)
    {
        /// <summary>
        /// Throws an exception if <paramref name="argument"/> is <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argument">The string argument to validate as non-null and non-empty.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="argument"/> is empty.</exception>
#if NET7_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
#if !NETCOREAPP3_0_OR_GREATER
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
#endif
        {
#if NET7_0_OR_GREATER
            ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
#else
            if (string.IsNullOrEmpty(argument))
                Throw(argument, paramName);

            [DoesNotReturn]
            static void Throw(string? argument, string? paramName)
            {
                ArgumentNullException.ThrowIfNull(argument, paramName);
                throw new ArgumentException(Resources.Argument_EmptyString, paramName);
            }
#endif
        }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

        /// <summary>
        /// Throws an exception if <paramref name="argument"/> is <see langword="null"/> or empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="argument">The string argument to validate.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="argument"/> is empty or consists only of white-space characters.</exception>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
#if !NETCOREAPP3_0_OR_GREATER
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
#endif
        {
#if NET8_0_OR_GREATER
            ArgumentException.ThrowIfNullOrWhiteSpace(argument, paramName);
#else
            if (string.IsNullOrWhiteSpace(argument))
                Throw(argument, paramName);

            [DoesNotReturn]
            static void Throw(string? argument, string? paramName)
            {
                ArgumentNullException.ThrowIfNull(argument, paramName);
                throw new ArgumentException(Resources.Argument_EmptyOrWhiteSpaceString, paramName);
            }
#endif
        }
    }

    /// <summary>
    /// Provides polyfill extension methods for <see cref="ArgumentNullException"/> class.
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
#if NET6_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
#if NET6_0_OR_GREATER
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
#if NET7_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static unsafe void ThrowIfNull([NotNull] void* argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
#if NET7_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(argument, paramName);
#else
            if (argument is null)
                throw new ArgumentNullException(paramName);
#endif
        }
    }

    /// <summary>
    /// Provides polyfill extension methods for <see cref="ArgumentOutOfRangeException"/> class.
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
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfZero(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfZero(value, paramName);
#else
            if (value == 0)
                ThrowZero(value, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfZero(int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfZero(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfZero(value, paramName);
#else
            if (value == 0)
                ThrowZero(value, paramName);
#endif
        }

#if !NET8_0_OR_GREATER
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
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
#else
            if (value < 0)
                ThrowNegative(value, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfNegative(int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNegative(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
#else
            if (value < 0)
                ThrowNegative(value, paramName);
#endif
        }

#if !NET8_0_OR_GREATER
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
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNegativeOrZero(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
#else
            if (value <= 0)
                ThrowNegativeOrZero(value, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfNegativeOrZero(int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNegativeOrZero(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
#else
            if (value <= 0)
                ThrowNegativeOrZero(value, paramName);
#endif
        }

#if !NET8_0_OR_GREATER
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
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfGreaterThan(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
#else
            if (value > other)
                ThrowGreater(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfGreaterThan(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static void ThrowIfGreaterThan(uint value, uint other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
#else
            if (value > other)
                ThrowGreater(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfGreaterThan(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfGreaterThan(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, other, paramName);
#else
            if (value > other)
                ThrowGreater(value, other, paramName);
#endif
        }

#if !NET8_0_OR_GREATER
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
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfGreaterThanOrEqual(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);
#else
            if (value >= other)
                ThrowGreaterEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfGreaterThanOrEqual(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static void ThrowIfGreaterThanOrEqual(uint value, uint other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);
#else
            if (value >= other)
                ThrowGreaterEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfGreaterThanOrEqual(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfGreaterThanOrEqual(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, other, paramName);
#else
            if (value >= other)
                ThrowGreaterEqual(value, other, paramName);
#endif
        }

#if !NET8_0_OR_GREATER
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
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfLessThan(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
#else
            if (value < other)
                ThrowLess(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfLessThan(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static void ThrowIfLessThan(uint value, uint other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
#else
            if (value < other)
                ThrowLess(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfLessThan(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfLessThan(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThan(value, other, paramName);
#else
            if (value < other)
                ThrowLess(value, other, paramName);
#endif
        }

#if !NET8_0_OR_GREATER
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
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfLessThanOrEqual(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);
#else
            if (value <= other)
                ThrowLessEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfLessThanOrEqual(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [CLSCompliant(false)]
        public static void ThrowIfLessThanOrEqual(uint value, uint other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);
#else
            if (value <= other)
                ThrowLessEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfLessThanOrEqual(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfLessThanOrEqual(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, other, paramName);
#else
            if (value <= other)
                ThrowLessEqual(value, other, paramName);
#endif
        }

#if !NET8_0_OR_GREATER
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
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfEqual(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfEqual(value, other, paramName);
#else
            if (value == other)
                ThrowEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfEqual(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfEqual(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfEqual(value, other, paramName);
#else
            if (value == other)
                ThrowEqual(value, other, paramName);
#endif
        }

#if !NET8_0_OR_GREATER
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
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNotEqual(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);
#else
            if (value != other)
                ThrowNotEqual(value, other, paramName);
#endif
        }

        /// <inheritdoc cref="ThrowIfNotEqual(int, int, string?)"/>
#if NET8_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIfNotEqual(double value, double other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNotEqual(value, other, paramName);
#else
            if (value != other)
                ThrowNotEqual(value, other, paramName);
#endif
        }

#if !NET8_0_OR_GREATER
        [DoesNotReturn]
        static void ThrowNotEqual(object? value, object? other, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, string.Format(Resources.ArgumentOutOfRange_Generic_MustBeEqual, paramName, value ?? "null", other ?? "null"));
#endif

        #endregion
    }

    /// <summary>
    /// Provides polyfill extension methods for <see cref="ObjectDisposedException"/> class.
    /// </summary>
    extension(ObjectDisposedException)
    {
        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the specified <paramref name="condition"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="instance">The object whose type's full name should be included in any resulting <see cref="ObjectDisposedException"/>.</param>
        /// <exception cref="ObjectDisposedException">The <paramref name="condition"/> is <see langword="true"/>.</exception>
#if NET7_0_OR_GREATER
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, object instance)
        {
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(condition, instance);
#else
            if (condition)
                Throw(instance);

            [DoesNotReturn]
            static void Throw(object? instance) => throw new ObjectDisposedException(instance?.GetType().FullName);
#endif
        }

        /// <inheritdoc cref="ThrowIf(bool, object)"/>
        /// <param name="condition"><inheritdoc/></param>
        /// <param name="type">The type whose full name should be included in any resulting <see cref="ObjectDisposedException"/>.</param>
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, Type type)
        {
#if NET7_0_OR_GREATER
            ObjectDisposedException.ThrowIf(condition, type);
#else
            if (condition)
                Throw(type);

            [DoesNotReturn]
            static void Throw(Type? type) => throw new ObjectDisposedException(type?.FullName);
#endif
        }
    }
}
