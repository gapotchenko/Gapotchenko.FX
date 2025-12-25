// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math;

using Math = System.Math;

partial class MathEx
{
#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY

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

#endif
}
