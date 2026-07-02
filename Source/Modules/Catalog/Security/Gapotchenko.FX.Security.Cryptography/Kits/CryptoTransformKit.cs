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

    public static bool HasForwardOverlap(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
    {
        return
            ReferenceEquals(inputBuffer, outputBuffer) &&
            outputOffset > inputOffset &&
            outputOffset < inputOffset + inputCount;
    }
}
