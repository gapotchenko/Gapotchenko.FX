// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Runtime.CompilerServices;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill extension methods for the <see cref="BitConverter"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class BitConverterPolyfills
{
    extension(BitConverter)
    {
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
            return Unsafe.BitCast<float, int>(value);
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
            return Unsafe.BitCast<int, float>(value);
#endif
        }
    }
}
