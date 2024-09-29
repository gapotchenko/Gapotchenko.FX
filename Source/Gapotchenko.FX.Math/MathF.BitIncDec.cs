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

partial class MathF
{
    /// <summary>
    /// Increments a value to the smallest value that compares greater than a given value.
    /// </summary>
    /// <param name="x">The value to be bitwise incremented.</param>
    /// <returns>
    /// The smallest value that compares greater than <paramref name="x"/>.
    /// <br/>-or-<br/>
    /// <see cref="float.PositiveInfinity"/> if <paramref name="x"/> equals <see cref="float.PositiveInfinity"/>.
    /// <br/>-or-<br/>
    /// <see cref="float.NaN"/> if <paramref name="x" /> equals <see cref="float.NaN"/>.
    /// </returns>
#if TFF_MATH_BITINCDEC
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static float BitIncrement(float x)
    {
#if TFF_MATH_BITINCDEC
        return System.MathF.BitIncrement(x);
#else
        int bits = BitConverterEx.SingleToInt32Bits(x);

        if ((bits & 0x7F800000) >= 0x7F800000)
        {
            // NaN returns NaN
            // -Infinity returns float.MinValue
            // +Infinity returns +Infinity
            return (bits == unchecked((int)(0xFF800000))) ? float.MinValue : x;
        }

        if (bits == unchecked((int)(0x80000000)))
        {
            // -0.0 returns float.Epsilon
            return float.Epsilon;
        }

        // Negative values need to be decremented
        // Positive values need to be incremented

        bits += (bits < 0) ? -1 : +1;
        return BitConverterEx.Int32BitsToSingle(bits);
#endif
    }

    /// <summary>
    /// Returns the largest value that compares less than a specified value.
    /// </summary>
    /// <param name="x">The value to decrement.</param>
    /// <returns>
    /// The largest value that compares less than <paramref name="x"/>.
    /// <br/>-or-<br/>
    /// <see cref="float.NegativeInfinity"/> if <paramref name="x"/> equals <see cref="float.NegativeInfinity"/>.
    /// <br/>-or-<br/>
    /// <see cref="float.NaN"/> if <paramref name="x"/> equals <see cref="float.NaN"/>.
    /// </returns>
#if TFF_MATH_BITINCDEC
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static float BitDecrement(float x)
    {
#if TFF_MATH_BITINCDEC
        return System.MathF.BitDecrement(x);
#else
        int bits = BitConverterEx.SingleToInt32Bits(x);

        if ((bits & 0x7F800000) >= 0x7F800000)
        {
            // NaN returns NaN
            // -Infinity returns -Infinity
            // +Infinity returns float.MaxValue
            return (bits == 0x7F800000) ? float.MaxValue : x;
        }

        if (bits == 0x00000000)
        {
            // +0.0 returns -float.Epsilon
            return -float.Epsilon;
        }

        // Negative values need to be incremented
        // Positive values need to be decremented

        bits += (bits < 0) ? +1 : -1;
        return BitConverterEx.Int32BitsToSingle(bits);
#endif
    }
}
