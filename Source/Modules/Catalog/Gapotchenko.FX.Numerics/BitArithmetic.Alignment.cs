// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Numerics;

partial class BitArithmetic
{
    /// <summary>
    /// Aligns the specified value upward to a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The positive power-of-two alignment.</param>
    /// <returns>
    /// The smallest multiple of <paramref name="alignment"/> that is greater than or equal to <paramref name="value"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a positive power of two.
    /// </exception>
    public static int AlignUp(int value, int alignment)
    {
#if NET
        return AlignUp<int>(value, alignment);
#else
        ValidateAlignment(alignment);

        int result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return result + alignment;
#endif
    }

    /// <summary>
    /// Aligns the specified value upward to a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The power-of-two alignment.</param>
    /// <returns>
    /// The smallest multiple of <paramref name="alignment"/> that is greater than or equal to <paramref name="value"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a power of two.
    /// </exception>
    [CLSCompliant(false)]
    public static uint AlignUp(uint value, uint alignment)
    {
#if NET
        return AlignUp<uint>(value, alignment);
#else
        ValidateAlignment(alignment);

        uint result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return result + alignment;
#endif
    }

    /// <inheritdoc cref="AlignUp(int, int)"/>
    public static long AlignUp(long value, long alignment)
    {
#if NET
        return AlignUp<long>(value, alignment);
#else
        ValidateAlignment(alignment);

        long result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return result + alignment;
#endif
    }

    /// <inheritdoc cref="AlignUp(uint, uint)"/>
    [CLSCompliant(false)]
    public static ulong AlignUp(ulong value, ulong alignment)
    {
#if NET
        return AlignUp<ulong>(value, alignment);
#else
        ValidateAlignment(alignment);

        ulong result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return result + alignment;
#endif
    }

    /// <inheritdoc cref="AlignUp(int, int)"/>
    public static nint AlignUp(nint value, nint alignment)
    {
#if NET
        return AlignUp<nint>(value, alignment);
#else
        ValidateAlignment(alignment);

        nint result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return result + alignment;
#endif
    }

    /// <inheritdoc cref="AlignUp(uint, uint)"/>
    [CLSCompliant(false)]
    public static nuint AlignUp(nuint value, nuint alignment)
    {
#if NET
        return AlignUp<nuint>(value, alignment);
#else
        ValidateAlignment(alignment);

        nuint result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return result + alignment;
#endif
    }

#if NET

    /// <inheritdoc cref="AlignUp(int, int)"/>
    public static T AlignUp<T>(T value, T alignment)
        where T : IBinaryInteger<T>
    {
        ValidateAlignment(alignment);

        var result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return result + alignment;
    }

#endif

    // ------------------------------------------------------------------------

    /// <summary>
    /// Attempts to align the specified value upward to a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The positive power-of-two alignment.</param>
    /// <returns>
    /// The smallest multiple of <paramref name="alignment"/> that is greater than or equal to <paramref name="value"/>,
    /// or <see langword="null"/> if the result is not representable by the underlying type indicating an overflow condition.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a positive power of two.
    /// </exception>
    public static int? TryAlignUp(int value, int alignment)
    {
#if NET
        return TryAlignUp<int>(value, alignment);
#else
        ValidateAlignment(alignment);

        int result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else if (result > int.MaxValue - alignment)
            return null;
        else
            return result + alignment;
#endif
    }

    /// <summary>
    /// Attempts to align the specified value upward to a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The power-of-two alignment.</param>
    /// <returns>
    /// The smallest multiple of <paramref name="alignment"/> that is greater than or equal to <paramref name="value"/>,
    /// or <see langword="null"/> if the result is not representable by the underlying type indicating an overflow condition.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a power of two.
    /// </exception>
    [CLSCompliant(false)]
    public static uint? TryAlignUp(uint value, uint alignment)
    {
#if NET
        return TryAlignUp<uint>(value, alignment);
#else
        ValidateAlignment(alignment);

        uint result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else if (result > uint.MaxValue - alignment)
            return null;
        else
            return result + alignment;
#endif
    }

    /// <inheritdoc cref="TryAlignUp(int, int)"/>
    public static long? TryAlignUp(long value, long alignment)
    {
#if NET
        return TryAlignUp<long>(value, alignment);
#else
        ValidateAlignment(alignment);

        long result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else if (result > long.MaxValue - alignment)
            return null;
        else
            return result + alignment;
#endif
    }

    /// <inheritdoc cref="TryAlignUp(uint, uint)"/>
    [CLSCompliant(false)]
    public static ulong? TryAlignUp(ulong value, ulong alignment)
    {
#if NET
        return TryAlignUp<ulong>(value, alignment);
#else
        ValidateAlignment(alignment);

        ulong result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else if (result > ulong.MaxValue - alignment)
            return null;
        else
            return result + alignment;
#endif
    }

    /// <inheritdoc cref="TryAlignUp(int, int)"/>
    public static nint? TryAlignUp(nint value, nint alignment)
    {
#if NET
        return TryAlignUp<nint>(value, alignment);
#else
        ValidateAlignment(alignment);

        nint result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else if (result > nint.MaxValue - alignment)
            return null;
        else
            return result + alignment;
#endif
    }

    /// <inheritdoc cref="TryAlignUp(uint, uint)"/>
    [CLSCompliant(false)]
    public static nuint? TryAlignUp(nuint value, nuint alignment)
    {
#if NET
        return TryAlignUp<nuint>(value, alignment);
#else
        ValidateAlignment(alignment);

        nuint result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else if (result > nuint.MaxValue - alignment)
            return null;
        else
            return result + alignment;
#endif
    }

#if NET

    /// <inheritdoc cref="TryAlignUp(int, int)"/>
    public static T? TryAlignUp<T>(T value, T alignment)
        where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
    {
        ValidateAlignment(alignment);

        var result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else if (result > T.MaxValue - alignment)
            return null;
        else
            return result + alignment;
    }

#endif

    // ------------------------------------------------------------------------

    /// <summary>
    /// Aligns the specified value upward to a multiple of the specified alignment and checks for overflow.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The positive power-of-two alignment.</param>
    /// <returns>
    /// The smallest multiple of <paramref name="alignment"/> that is greater than or equal to <paramref name="value"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a positive power of two.
    /// </exception>
    /// <exception cref="OverflowException">
    /// The result is not representable by the underlying type indicating an overflow condition.
    /// </exception>
    public static int AlignUpChecked(int value, int alignment)
    {
#if NET
        return AlignUpChecked<int>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignUpCheckedCore(value, alignment);
#endif
    }

    /// <summary>
    /// Aligns the specified value upward to a multiple of the specified alignment and checks for overflow.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The power-of-two alignment.</param>
    /// <returns>
    /// The smallest multiple of <paramref name="alignment"/> that is greater than or equal to <paramref name="value"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a power of two.
    /// </exception>
    /// <exception cref="OverflowException">
    /// The result is not representable by the underlying type indicating an overflow condition.
    /// </exception>
    [CLSCompliant(false)]
    public static uint AlignUpChecked(uint value, uint alignment)
    {
#if NET
        return AlignUpChecked<uint>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignUpCheckedCore(value, alignment);
#endif
    }

    /// <inheritdoc cref="AlignUpChecked(int, int)"/>
    public static long AlignUpChecked(long value, long alignment)
    {
#if NET
        return AlignUpChecked<long>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignUpCheckedCore(value, alignment);
#endif
    }

    /// <inheritdoc cref="AlignUpChecked(uint, uint)"/>
    [CLSCompliant(false)]
    public static ulong AlignUpChecked(ulong value, ulong alignment)
    {
#if NET
        return AlignUpChecked<ulong>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignUpCheckedCore(value, alignment);
#endif
    }

    /// <inheritdoc cref="AlignUpChecked(int, int)"/>
    public static nint AlignUpChecked(nint value, nint alignment)
    {
#if NET
        return AlignUpChecked<nint>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignUpCheckedCore(value, alignment);
#endif
    }

    /// <inheritdoc cref="AlignUpChecked(uint, uint)"/>
    [CLSCompliant(false)]
    public static nuint AlignUpChecked(nuint value, nuint alignment)
    {
#if NET
        return AlignUpChecked<nuint>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignUpCheckedCore(value, alignment);
#endif
    }

#if NET

    /// <inheritdoc cref="AlignUpChecked(int, int)"/>
    public static T AlignUpChecked<T>(T value, T alignment)
        where T : IBinaryInteger<T>
    {
        ValidateAlignment(alignment);

        return AlignUpCheckedCore(value, alignment);
    }

#endif

    // ------------------------------------------------------------------------

    /// <summary>
    /// Aligns the specified value downward to a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The positive power-of-two alignment.</param>
    /// <returns>
    /// The largest multiple of <paramref name="alignment"/> that is less than or equal to <paramref name="value"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a positive power of two.
    /// </exception>
    public static int AlignDown(int value, int alignment)
    {
#if NET
        return AlignDown<int>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment);
#endif
    }

    /// <summary>
    /// Aligns the specified value downward to a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The power-of-two alignment.</param>
    /// <returns>
    /// The largest multiple of <paramref name="alignment"/> that is less than or equal to <paramref name="value"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a power of two.
    /// </exception>
    [CLSCompliant(false)]
    public static uint AlignDown(uint value, uint alignment)
    {
#if NET
        return AlignDown<uint>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment);
#endif
    }

    /// <inheritdoc cref="AlignDown(int, int)"/>
    public static long AlignDown(long value, long alignment)
    {
#if NET
        return AlignDown<long>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment);
#endif
    }

    /// <inheritdoc cref="AlignDown(uint, uint)"/>
    [CLSCompliant(false)]
    public static ulong AlignDown(ulong value, ulong alignment)
    {
#if NET
        return AlignDown<ulong>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment);
#endif
    }

    /// <inheritdoc cref="AlignDown(int, int)"/>
    public static nint AlignDown(nint value, nint alignment)
    {
#if NET
        return AlignDown<nint>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment);
#endif
    }

    /// <inheritdoc cref="AlignDown(uint, uint)"/>
    [CLSCompliant(false)]
    public static nuint AlignDown(nuint value, nuint alignment)
    {
#if NET
        return AlignDown<nuint>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment);
#endif
    }

#if NET

    /// <inheritdoc cref="AlignDown(int, int)"/>
    public static T AlignDown<T>(T value, T alignment)
        where T : IBinaryInteger<T>
    {
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment);
    }

#endif

    // ------------------------------------------------------------------------

    /// <summary>
    /// Determines whether the specified value is a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to examine.</param>
    /// <param name="alignment">The positive power-of-two alignment.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> is aligned to a multiple of <paramref name="alignment"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a positive power of two.
    /// </exception>
    public static bool IsAligned(int value, int alignment)
    {
#if NET
        return IsAligned<int>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment) == value;
#endif
    }

    /// <summary>
    /// Determines whether the specified value is a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to examine.</param>
    /// <param name="alignment">The power-of-two alignment.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> is aligned to a multiple of <paramref name="alignment"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a power of two.
    /// </exception>
    [CLSCompliant(false)]
    public static bool IsAligned(uint value, uint alignment)
    {
#if NET
        return IsAligned<uint>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment) == value;
#endif
    }

    /// <inheritdoc cref="IsAligned(int, int)"/>
    public static bool IsAligned(long value, long alignment)
    {
#if NET
        return IsAligned<long>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment) == value;
#endif
    }

    /// <inheritdoc cref="IsAligned(uint, uint)"/>
    [CLSCompliant(false)]
    public static bool IsAligned(ulong value, ulong alignment)
    {
#if NET
        return IsAligned<ulong>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment) == value;
#endif
    }

    /// <inheritdoc cref="IsAligned(int, int)"/>
    public static bool IsAligned(nint value, nint alignment)
    {
#if NET
        return IsAligned<nint>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment) == value;
#endif
    }

    /// <inheritdoc cref="IsAligned(uint, uint)"/>
    [CLSCompliant(false)]
    public static bool IsAligned(nuint value, nuint alignment)
    {
#if NET
        return IsAligned<nuint>(value, alignment);
#else
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment) == value;
#endif
    }

#if NET

    /// <inheritdoc cref="IsAligned(int, int)"/>
    public static bool IsAligned<T>(T value, T alignment)
        where T : IBinaryInteger<T>
    {
        ValidateAlignment(alignment);

        return AlignDownCore(value, alignment) == value;
    }

#endif

    // ------------------------------------------------------------------------

    /// <summary>
    /// Gets the padding required to align the specified value upward to a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The positive power-of-two alignment.</param>
    /// <returns>
    /// The alignment padding, from zero inclusive to <paramref name="alignment"/> exclusive.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a positive power of two.
    /// </exception>
    public static int GetAlignmentPadding(int value, int alignment)
    {
#if NET
        return GetAlignmentPadding<int>(value, alignment);
#else
        ValidateAlignment(alignment);

        int alignmentMask = alignment - 1;
        int remainder = value & alignmentMask;
        return (alignment - remainder) & alignmentMask;
#endif
    }

    /// <summary>
    /// Gets the padding required to align the specified value upward to a multiple of the specified alignment.
    /// </summary>
    /// <param name="value">The value to align.</param>
    /// <param name="alignment">The power-of-two alignment.</param>
    /// <returns>
    /// The alignment padding, from zero inclusive to <paramref name="alignment"/> exclusive.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="alignment"/> is not a power of two.
    /// </exception>
    [CLSCompliant(false)]
    public static uint GetAlignmentPadding(uint value, uint alignment)
    {
#if NET
        return GetAlignmentPadding<uint>(value, alignment);
#else
        ValidateAlignment(alignment);

        uint alignmentMask = alignment - 1;
        uint remainder = value & alignmentMask;
        return (alignment - remainder) & alignmentMask;
#endif
    }

    /// <inheritdoc cref="GetAlignmentPadding(int, int)"/>
    public static long GetAlignmentPadding(long value, long alignment)
    {
#if NET
        return GetAlignmentPadding<long>(value, alignment);
#else
        ValidateAlignment(alignment);

        long alignmentMask = alignment - 1;
        long remainder = value & alignmentMask;
        return (alignment - remainder) & alignmentMask;
#endif
    }

    /// <inheritdoc cref="GetAlignmentPadding(uint, uint)"/>
    [CLSCompliant(false)]
    public static ulong GetAlignmentPadding(ulong value, ulong alignment)
    {
#if NET
        return GetAlignmentPadding<ulong>(value, alignment);
#else
        ValidateAlignment(alignment);

        ulong alignmentMask = alignment - 1;
        ulong remainder = value & alignmentMask;
        return (alignment - remainder) & alignmentMask;
#endif
    }

    /// <inheritdoc cref="GetAlignmentPadding(int, int)"/>
    public static nint GetAlignmentPadding(nint value, nint alignment)
    {
#if NET
        return GetAlignmentPadding<nint>(value, alignment);
#else
        ValidateAlignment(alignment);

        nint alignmentMask = alignment - 1;
        nint remainder = value & alignmentMask;
        return (alignment - remainder) & alignmentMask;
#endif
    }

    /// <inheritdoc cref="GetAlignmentPadding(uint, uint)"/>
    [CLSCompliant(false)]
    public static nuint GetAlignmentPadding(nuint value, nuint alignment)
    {
#if NET
        return GetAlignmentPadding<nuint>(value, alignment);
#else
        ValidateAlignment(alignment);

        nuint alignmentMask = alignment - 1;
        nuint remainder = value & alignmentMask;
        return (alignment - remainder) & alignmentMask;
#endif
    }

#if NET

    /// <inheritdoc cref="GetAlignmentPadding(int, int)"/>
    public static T GetAlignmentPadding<T>(T value, T alignment)
        where T : IBinaryInteger<T>
    {
        ValidateAlignment(alignment);

        var alignmentMask = alignment - T.One;
        var remainder = value & alignmentMask;
        return (alignment - remainder) & alignmentMask;
    }

#endif

    // ------------------------------------------------------------------------

#if NET

    static T AlignUpCheckedCore<T>(T value, T alignment)
        where T : IBinaryInteger<T>
    {
        var result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return checked(result + alignment);
    }

    static T AlignDownCore<T>(T value, T alignment) where T : IBinaryInteger<T> => value & ~(alignment - T.One);

#else

    static int AlignUpCheckedCore(int value, int alignment)
    {
        int result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return checked(result + alignment);
    }

    static uint AlignUpCheckedCore(uint value, uint alignment)
    {
        uint result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return checked(result + alignment);
    }

    static long AlignUpCheckedCore(long value, long alignment)
    {
        long result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return checked(result + alignment);
    }

    static ulong AlignUpCheckedCore(ulong value, ulong alignment)
    {
        ulong result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return checked(result + alignment);
    }

    static nint AlignUpCheckedCore(nint value, nint alignment)
    {
        nint result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return checked(result + alignment);
    }

    static nuint AlignUpCheckedCore(nuint value, nuint alignment)
    {
        nuint result = AlignDownCore(value, alignment);
        if (result == value)
            return result;
        else
            return checked(result + alignment);
    }

    static int AlignDownCore(int value, int alignment) => value & ~(alignment - 1);

    static uint AlignDownCore(uint value, uint alignment) => value & ~(alignment - 1);

    static long AlignDownCore(long value, long alignment) => value & ~(alignment - 1);

    static ulong AlignDownCore(ulong value, ulong alignment) => value & ~(alignment - 1);

    static nint AlignDownCore(nint value, nint alignment) => value & ~(alignment - 1);

    static nuint AlignDownCore(nuint value, nuint alignment) => value & ~(alignment - 1);

#endif

    // ------------------------------------------------------------------------

#if NET

    [StackTraceHidden]
    static void ValidateAlignment<T>(
        T alignment,
        [CallerArgumentExpression(nameof(alignment))] string? paramName = null)
        where T : IBinaryInteger<T>
    {
        if (alignment <= T.Zero || (alignment & (alignment - T.One)) != T.Zero)
            ThrowInvalidAlignment(paramName);
    }

#else

    [StackTraceHidden]
    static void ValidateAlignment(
        int alignment,
        [CallerArgumentExpression(nameof(alignment))] string? paramName = null)
    {
        if (alignment <= 0 || (alignment & (alignment - 1)) != 0)
            ThrowInvalidAlignment(paramName);
    }

    [StackTraceHidden]
    static void ValidateAlignment(
        uint alignment,
        [CallerArgumentExpression(nameof(alignment))] string? paramName = null)
    {
        if (alignment == 0 || (alignment & (alignment - 1)) != 0)
            ThrowInvalidAlignment(paramName);
    }

    [StackTraceHidden]
    static void ValidateAlignment(
        long alignment,
        [CallerArgumentExpression(nameof(alignment))] string? paramName = null)
    {
        if (alignment <= 0 || (alignment & (alignment - 1)) != 0)
            ThrowInvalidAlignment(paramName);
    }

    [StackTraceHidden]
    static void ValidateAlignment(
        ulong alignment,
        [CallerArgumentExpression(nameof(alignment))] string? paramName = null)
    {
        if (alignment == 0 || (alignment & (alignment - 1)) != 0)
            ThrowInvalidAlignment(paramName);
    }

    [StackTraceHidden]
    static void ValidateAlignment(
        nint alignment,
        [CallerArgumentExpression(nameof(alignment))] string? paramName = null)
    {
        if (alignment <= 0 || (alignment & (alignment - 1)) != 0)
            ThrowInvalidAlignment(paramName);
    }

    [StackTraceHidden]
    static void ValidateAlignment(
        nuint alignment,
        [CallerArgumentExpression(nameof(alignment))] string? paramName = null)
    {
        if (alignment == 0 || (alignment & (alignment - 1)) != 0)
            ThrowInvalidAlignment(paramName);
    }

#endif

    [StackTraceHidden]
    [DoesNotReturn]
    static void ThrowInvalidAlignment(string? paramName)
    {
        throw new ArgumentOutOfRangeException(
            paramName,
            "The alignment must be a positive power of two.");
    }
}
