// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math;

using Gapotchenko.FX.Math.Utils;
using System.Runtime.CompilerServices;
using Math = System.Math;

partial class MathEx
{
#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY // 2025

    #region Min

    /// <inheritdoc cref="MathExtensions.Min{T}(T, T)"/>
    [Obsolete("Use System.Math.Min(val1, val2) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    public static T? Min<T>(T? val1, T? val2) where T : IComparable<T>
    {
        if (val1 is null)
            return val2;
        else if (val2 is null)
            return val1;
        else
            return Math.Min(val1, val2);
    }

    /// <inheritdoc cref="MathExtensions.Min{T}(T, T, IComparer{T}?)"/>
    [Obsolete("Use System.Math.Min(val1, val2, comparer) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    public static T? Min<T>(T? val1, T? val2, IComparer<T>? comparer = null)
    {
        if (val1 is null)
            return val2;
        else if (val2 is null)
            return val1;
        else
            return Math.Min(val1, val2, comparer)!;
    }

    /// <inheritdoc cref="MathExtensions.Min{T}(T, T, T)"/>
    [Obsolete("Use System.Math.Min(val1, val2, val3) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    [return: NotNullIfNotNull(nameof(val3))]
    public static T? Min<T>(T? val1, T? val2, T? val3) where T : IComparable<T> =>
        Min(Min(val1, val2), val3);

    /// <inheritdoc cref="MathExtensions.Min{T}(T, T, T, IComparer{T}?)"/>
    [Obsolete("Use System.Math.Min(val1, val2, val3, comparer) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    [return: NotNullIfNotNull(nameof(val3))]
    public static T? Min<T>(T? val1, T? val2, T? val3, IComparer<T>? comparer = null) =>
        Min(Min(val1, val2, comparer), val3, comparer);

    /// <inheritdoc cref="MathExtensions.Min{T}(T, T, T)"/>
    [Obsolete("Use System.Math.Min(val1, val2, val3) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int Min(int val1, int val2, int val3) => Math.Min(val1, val2, val3);

    /// <inheritdoc cref="MathExtensions.Min{T}(T, T)"/>
    [Obsolete("Use System.Math.Min(val1, val2) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateTime Min(DateTime val1, DateTime val2) => Math.Min(val1, val2);

    /// <inheritdoc cref="MathExtensions.Min{T}(T, T, T)"/>
    [Obsolete("Use System.Math.Min(val1, val2, val3) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateTime Min(DateTime val1, DateTime val2, DateTime val3) => Math.Min(val1, val2, val3);

    /// <inheritdoc cref="MathExtensions.Min{T}(T, T)"/>
    [Obsolete("Use System.Math.Min(val1, val2) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    public static Version? Min(Version? val1, Version? val2)
    {
        if (val1 is null)
            return val2;
        if (val2 is null)
            return val1;
        return val1 <= val2 ? val1 : val2;
    }

    /// <inheritdoc cref="MathExtensions.Min{T}(T, T, T)"/>
    [Obsolete("Use System.Math.Min(val1, val2, val3) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    [return: NotNullIfNotNull(nameof(val3))]
    public static Version? Min(Version? val1, Version? val2, Version? val3) => Min(Min(val1, val2), val3);

    #endregion

    #region Max

    /// <inheritdoc cref="MathExtensions.Max{T}(T, T)"/>
    [Obsolete("Use System.Math.Max(val1, val2) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    public static T? Max<T>(T? val1, T? val2) where T : IComparable<T>
    {
        if (val1 is null)
            return val2;
        else if (val2 is null)
            return val1;
        else
            return Math.Max(val1, val2);
    }

    /// <inheritdoc cref="MathExtensions.Max{T}(T, T, IComparer{T}?)"/>
    [Obsolete("Use System.Math.Max(val1, val2, comparer) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    public static T? Max<T>(T? val1, T? val2, IComparer<T>? comparer = null)
    {
        if (val1 is null)
            return val2;
        else if (val2 is null)
            return val1;
        else
            return Math.Max(val1, val2, comparer)!;
    }

    /// <inheritdoc cref="MathExtensions.Max{T}(T, T, T)"/>
    [Obsolete("Use System.Math.Max(val1, val2, val3) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    [return: NotNullIfNotNull(nameof(val3))]
    public static T? Max<T>(T? val1, T? val2, T? val3) where T : IComparable<T> =>
        Max(Max(val1, val2), val3);

    /// <inheritdoc cref="MathExtensions.Max{T}(T, T, T, IComparer{T}?)"/>
    [Obsolete("Use System.Math.Max(val1, val2, val3, comparer) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    [return: NotNullIfNotNull(nameof(val3))]
    public static T? Max<T>(T? val1, T? val2, T? val3, IComparer<T>? comparer = null) =>
        Max(Max(val1, val2, comparer), val3, comparer);

    /// <inheritdoc cref="MathExtensions.Max{T}(T, T, T)"/>
    [Obsolete("Use System.Math.Max(val1, val2, val3) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int Max(int val1, int val2, int val3) => Math.Max(val1, val2, val3);

    /// <inheritdoc cref="MathExtensions.Max{T}(T, T)"/>
    [Obsolete("Use System.Math.Max(val1, val2) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateTime Max(DateTime val1, DateTime val2) => Math.Max(val1, val2);

    /// <inheritdoc cref="MathExtensions.Max{T}(T, T, T)"/>
    [Obsolete("Use System.Math.Max(val1, val2, val3) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static DateTime Max(DateTime val1, DateTime val2, DateTime val3) => Math.Max(val1, val2, val3);

    /// <inheritdoc cref="MathExtensions.Max{T}(T, T)"/>
    [Obsolete("Use System.Math.Max(val1, val2) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    public static Version? Max(Version? val1, Version? val2)
    {
        if (val1 is null)
            return val2;
        if (val2 is null)
            return val1;
        return val1 >= val2 ? val1 : val2;
    }

    /// <inheritdoc cref="MathExtensions.Max{T}(T, T, T)"/>
    [Obsolete("Use System.Math.Max(val1, val2, val3) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(val1))]
    [return: NotNullIfNotNull(nameof(val2))]
    [return: NotNullIfNotNull(nameof(val3))]
    public static Version? Max(Version? val1, Version? val2, Version? val3) => Max(Max(val1, val2), val3);

    #endregion

    #region Clamp

    /// <inheritdoc cref="MathExtensions.Clamp{T}(T, T, T)"/>
    [Obsolete("Use System.Math.Clamp(value, min, max) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(value))]
    public static T? Clamp<T>(T? value, T? min, T? max) where T : IComparable<T>?
    {
        if (min is null && max is null)
            return value;

        if (min is not null && max is not null && min.CompareTo(max) > 0)
            ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value is null)
            return value;

        if (min is not null && value.CompareTo(min) < 0)
            return min;
        else if (max is not null && value.CompareTo(max) > 0)
            return max;

        return value;
    }

    /// <inheritdoc cref="MathExtensions.Clamp{T}(T, T, T, IComparer{T}?)"/>
    [Obsolete("Use System.Math.Clamp(value, min, max, comparer) method instead (may change semantics for null arguments).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [return: NotNullIfNotNull(nameof(value))]
    public static T? Clamp<T>(T? value, T? min, T? max, IComparer<T>? comparer)
    {
        if (min is null && max is null)
            return value;

        comparer ??= Comparer<T>.Default;

        if (min is not null && max is not null && comparer.Compare(min, max) > 0)
            ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value is null)
            return value;

        if (min is not null && comparer.Compare(value, min) < 0)
            return min;
        else if (max is not null && comparer.Compare(value, max) > 0)
            return max;

        return value;
    }

    /// <inheritdoc cref="MathPolyfills.Clamp(byte, byte, byte)"/>
    [Obsolete("Use System.Math.Clamp(System.Byte, System.Byte, System.Byte) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Clamp(byte value, byte min, byte max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(sbyte, sbyte, sbyte)"/>
    [Obsolete("Use System.Math.Clamp(System.SByte, System.SByte, System.SByte) method instead.")]
    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Clamp(sbyte value, sbyte min, sbyte max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(short, short, short)"/>
    [Obsolete("Use System.Math.Clamp(System.Int16, System.Int16, System.Int16) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Clamp(short value, short min, short max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(ushort, ushort, ushort)"/>
    [Obsolete("Use System.Math.Clamp(System.UInt16, System.UInt16, System.UInt16) method instead.")]
    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Clamp(ushort value, ushort min, ushort max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(int, int, int)"/>
    [Obsolete("Use System.Math.Clamp(System.Int32, System.Int32, System.Int32) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int min, int max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(uint, uint, uint)"/>
    [Obsolete("Use System.Math.Clamp(System.UInt32, System.UInt32, System.UInt32) method instead.")]
    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Clamp(uint value, uint min, uint max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(long, long, long)"/>
    [Obsolete("Use System.Math.Clamp(System.Int64, System.Int64, System.Int64) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Clamp(long value, long min, long max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(ulong, ulong, ulong)"/>
    [Obsolete("Use System.Math.Clamp(System.UInt64, System.UInt64, System.UInt64) method instead.")]
    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Clamp(ulong value, ulong min, ulong max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(decimal, decimal, decimal)"/>
    [Obsolete("Use System.Math.Clamp(System.Decimal, System.Decimal, System.Decimal) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Clamp(decimal value, decimal min, decimal max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(float, float, float)"/>
    [Obsolete("Use System.Math.Clamp(System.Single, System.Single, System.Single) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float min, float max) => Math.Clamp(value, min, max);

    /// <inheritdoc cref="MathPolyfills.Clamp(double, double, double)"/>
    [Obsolete("Use System.Math.Clamp(System.Double, System.Double, System.Double) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(double value, double min, double max) => Math.Clamp(value, min, max);

    #endregion

    #region BitIncrement/BitDecrement

    /// <inheritdoc cref="MathPolyfills.BitIncrement(double)"/>
    [Obsolete("Use Math.BitIncrement(double) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static double BitIncrement(double x) => Math.BitIncrement(x);

    /// <inheritdoc cref="MathPolyfills.BitDecrement(double)"/>
    [Obsolete("Use Math.BitDecrement(double) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static double BitDecrement(double x) => Math.BitDecrement(x);

    /// <inheritdoc cref="MathFPolyfills.BitIncrement(float)"/>
    [Obsolete("Use MathF.BitIncrement(float) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static float BitIncrement(float x) => MathF.BitIncrement(x);

    /// <inheritdoc cref="MathFPolyfills.BitDecrement(float)"/>
    [Obsolete("Use MathF.BitDecrement(float) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static float BitDecrement(float x) => MathF.BitDecrement(x);

    #endregion

    #region Swap

    /// <summary>
    /// Swaps two values.
    /// </summary>
    /// <typeparam name="T">The type of values to swap.</typeparam>
    /// <param name="val1">The first of two values to swap.</param>
    /// <param name="val2">The second of two values to swap.</param>
    [Obsolete("Use tuple syntax to swap values: (a, b) = (b, a).")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap<T>(ref T val1, ref T val2)
    {
        (val2, val1) = (val1, val2);
    }

    #endregion

#endif
}
