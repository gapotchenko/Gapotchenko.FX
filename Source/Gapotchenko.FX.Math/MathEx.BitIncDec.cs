// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2024

#if NETCOREAPP3_0_OR_GREATER
#define TFF_MATH_BITINCDEC
#endif

namespace Gapotchenko.FX.Math;

using Math = System.Math;

partial class MathEx
{
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
}
