// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETCOREAPP3_0_OR_GREATER
#define TFF_MATH_BITINCDEC
#endif

using Gapotchenko.FX.Math.Utils;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Math;

using Math = System.Math;

/// <summary>
/// Provides polyfill extension methods for the <see cref="Math"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class MathPolyfills
{
    /// <summary>
    /// Provides polyfill extension methods for <see cref="Math"/> class.
    /// </summary>
    extension(Math)
    {
        #region BitIncrement/BitDecrement

        /// <summary>
        /// Increments a value to the smallest value that compares greater than a given value.
        /// </summary>
        /// <param name="x">The value to be bitwise incremented.</param>
        /// <returns>
        /// The smallest value that compares greater than <paramref name="x"/>.
        /// <br/>-or-<br/>
        /// <see cref="double.PositiveInfinity"/> if <paramref name="x"/> equals <see cref="double.PositiveInfinity"/>.
        /// <br/>-or-<br/>
        /// <see cref="double.NaN"/> if <paramref name="x" /> equals <see cref="double.NaN"/>.
        /// </returns>
#if TFF_MATH_BITINCDEC
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static double BitIncrement(double x)
        {
#if TFF_MATH_BITINCDEC
            return Math.BitIncrement(x);
#else
            long bits = BitConverter.DoubleToInt64Bits(x);

            if (((bits >> 32) & 0x7FF00000) >= 0x7FF00000)
            {
                // NaN returns NaN
                // -Infinity returns double.MinValue
                // +Infinity returns +Infinity
                return (bits == unchecked((long)(0xFFF00000_00000000))) ? double.MinValue : x;
            }

            if (bits == unchecked((long)(0x80000000_00000000)))
            {
                // -0.0 returns double.Epsilon
                return double.Epsilon;
            }

            // Negative values need to be decremented
            // Positive values need to be incremented

            bits += (bits < 0) ? -1 : +1;
            return BitConverter.Int64BitsToDouble(bits);
#endif
        }

        /// <summary>
        /// Returns the largest value that compares less than a specified value.
        /// </summary>
        /// <param name="x">The value to decrement.</param>
        /// <returns>
        /// The largest value that compares less than <paramref name="x"/>.
        /// <br/>-or-<br/>
        /// <see cref="double.NegativeInfinity"/> if <paramref name="x"/> equals <see cref="double.NegativeInfinity"/>.
        /// <br/>-or-<br/>
        /// <see cref="double.NaN"/> if <paramref name="x"/> equals <see cref="double.NaN"/>.
        /// </returns>
#if TFF_MATH_BITINCDEC
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static double BitDecrement(double x)
        {
#if TFF_MATH_BITINCDEC
            return Math.BitDecrement(x);
#else
            long bits = BitConverter.DoubleToInt64Bits(x);

            if (((bits >> 32) & 0x7FF00000) >= 0x7FF00000)
            {
                // NaN returns NaN
                // -Infinity returns -Infinity
                // +Infinity returns double.MaxValue
                return (bits == 0x7FF00000_00000000) ? double.MaxValue : x;
            }

            if (bits == 0x00000000_00000000)
            {
                // +0.0 returns -double.Epsilon
                return -double.Epsilon;
            }

            // Negative values need to be incremented
            // Positive values need to be decremented

            bits += (bits < 0) ? +1 : -1;
            return BitConverter.Int64BitsToDouble(bits);
#endif
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Returns a 8-bit unsigned integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Clamp(byte value, byte min, byte max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a 8-bit signed integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a 16-bit signed integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short Clamp(short value, short min, short max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a 16-bit unsigned integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Clamp(ushort value, ushort min, ushort max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a 32-bit signed integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Clamp(uint value, uint min, uint max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a 64-bit signed integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Clamp(long value, long min, long max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a 64-bit unsigned integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [CLSCompliant(false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Clamp(ulong value, ulong min, ulong max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a <see cref="decimal"/> <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal Clamp(decimal value, decimal min, decimal max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a <see cref="float"/> <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        /// <summary>
        /// Returns a <see cref="double"/> <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound of the range.</param>
        /// <param name="max">The upper bound of the range.</param>
        /// <returns>
        /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
        /// <br/>-or-<br/>
        /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
        /// <br/>-or-<br/>
        /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
        /// </returns>
#if TFF_MATH_CLAMP
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max)
        {
#if TFF_MATH_CLAMP
            return Math.Clamp(value, min, max);
#else
            if (min > max)
                ThrowHelper.ThrowMinCannotBeGreaterThanMaxException(min, max);

            if (value < min)
                return min;
            else if (value > max)
                return max;

            return value;
#endif
        }

        #endregion
    }
}
