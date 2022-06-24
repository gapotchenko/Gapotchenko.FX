using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Gapotchenko.FX;

/// <summary>
/// Provides extended functionality for base data type conversion.
/// </summary>
#if TFF_BITCONVERTER_SINGLETOINT32BITS
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public static class BitConverterEx
{
#if !TFF_BITCONVERTER_SINGLETOINT32BITS
    [StructLayout(LayoutKind.Explicit)]
    struct ReinterpretCastGround32
    {
        [FieldOffset(0)]
        public int Int32;

        [FieldOffset(0)]
        public float Single;
    }
#endif

    /// <summary>
    /// <para>
    /// Converts the specified single-precision floating point number to a 32-bit signed integer.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>A 32-bit signed integer whose value is equivalent to <paramref name="value"/>.</returns>
#if TFF_BITCONVERTER_SINGLETOINT32BITS
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int SingleToInt32Bits(float value)
    {
#if TFF_BITCONVERTER_SINGLETOINT32BITS
        return BitConverter.SingleToInt32Bits(value);
#else
        var rcg = new ReinterpretCastGround32();
        rcg.Single = value;
        return rcg.Int32;
#endif
    }

    /// <summary>
    /// <para>
    /// Converts the specified 32-bit signed integer to a single-precision floating point number.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="value">The number to convert.</param>
    /// <returns>A single-precision floating point number whose value is equivalent to <paramref name="value"/>.</returns>
#if TFF_BITCONVERTER_SINGLETOINT32BITS
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static float Int32BitsToSingle(int value)
    {
#if TFF_BITCONVERTER_SINGLETOINT32BITS
        return BitConverter.Int32BitsToSingle(value);
#else
        var rcg = new ReinterpretCastGround32();
        rcg.Int32 = value;
        return rcg.Single;
#endif
    }
}
