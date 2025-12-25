// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX;

partial class BitConverterEx
{
    /// <inheritdoc cref="BitConverterPolyfills.SingleToInt32Bits" />
    [Obsolete("Use BitConverter.SingleToInt32Bits method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int SingleToInt32Bits(float value) => BitConverter.SingleToInt32Bits(value);

    /// <inheritdoc cref="BitConverterPolyfills.Int32BitsToSingle" />
    [Obsolete("Use BitConverter.Int32BitsToSingle method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static float Int32BitsToSingle(int value) => BitConverter.Int32BitsToSingle(value);
}
