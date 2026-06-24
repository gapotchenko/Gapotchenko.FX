// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Security.Cryptography.Kits;

partial class CryptoTransformKit
{
    /// <summary>
    /// Validates input buffer arguments passed to an <see cref="ICryptoTransform"/> operation.
    /// </summary>
    /// <param name="inputBuffer">The input buffer to validate.</param>
    /// <param name="inputOffset">The offset into <paramref name="inputBuffer"/> at which input data begins.</param>
    /// <param name="inputCount">The number of bytes of input data.</param>
    /// <param name="inputBufferParamName">The name of the <paramref name="inputBuffer"/> parameter.</param>
    /// <param name="inputOffsetParamName">The name of the <paramref name="inputOffset"/> parameter.</param>
    /// <param name="inputCountParamName">The name of the <paramref name="inputCount"/> parameter.</param>
    /// <exception cref="ArgumentNullException"><paramref name="inputBuffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="inputOffset"/> or <paramref name="inputCount"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="inputOffset"/> and <paramref name="inputCount"/> do not identify a valid range in
    /// <paramref name="inputBuffer"/>.
    /// </exception>
    public static void ValidateInputArguments(
        byte[] inputBuffer,
        int inputOffset,
        int inputCount,
        [CallerArgumentExpression(nameof(inputBuffer))] string? inputBufferParamName = null,
        [CallerArgumentExpression(nameof(inputOffset))] string? inputOffsetParamName = null,
        [CallerArgumentExpression(nameof(inputCount))] string? inputCountParamName = null)
    {
        ArgumentNullException.ThrowIfNull(inputBuffer, inputBufferParamName);
        ArgumentOutOfRangeException.ThrowIfNegative(inputOffset, inputOffsetParamName);
        ArgumentOutOfRangeException.ThrowIfNegative(inputCount, inputCountParamName);

        if (inputOffset > inputBuffer.Length - inputCount)
        {
            throw new ArgumentException(
                "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.",
                inputBufferParamName);
        }
    }
}
