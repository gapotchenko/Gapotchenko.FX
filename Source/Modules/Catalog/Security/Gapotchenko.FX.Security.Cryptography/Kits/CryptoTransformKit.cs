// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Kits;

/// <summary>
/// Provides common building blocks for cryptographic transform implementations.
/// </summary>
/// <remarks>
/// This type is intended for implementers of <see cref="ICryptoTransform"/> interface.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static partial class CryptoTransformKit
{
    // This type is partial.
    // For the rest of the implementation, please take a look at the neighboring source files.

    /// <summary>
    /// Determines whether writing output buffer from start to end would overwrite the input buffer.
    /// </summary>
    /// <param name="inputBuffer">The input buffer.</param>
    /// <param name="inputOffset">The offset into <paramref name="inputBuffer"/> at which input data begins.</param>
    /// <param name="inputCount">The number of bytes of input data.</param>
    /// <param name="outputBuffer">The output buffer.</param>
    /// <param name="outputOffset">The offset into <paramref name="outputBuffer"/> at which output data begins.</param>
    /// <returns>
    /// <see langword="true"/> when writing output can overwrite input bytes;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool HasForwardOverlap(
        byte[] inputBuffer, int inputOffset,
        int inputCount, byte[] outputBuffer, int outputOffset)
    {
        return
            ReferenceEquals(inputBuffer, outputBuffer) &&
            outputOffset > inputOffset &&
            outputOffset < inputOffset + inputCount;
    }
}
