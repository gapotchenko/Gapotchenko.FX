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

    /// <summary>
    /// Validates output buffer arguments passed to an <see cref="ICryptoTransform"/> operation.
    /// </summary>
    /// <param name="outputBuffer">The output buffer to validate.</param>
    /// <param name="outputOffset">The offset into <paramref name="outputBuffer"/> at which output data begins.</param>
    /// <param name="requiredOutputCount">The number of bytes of output data required.</param>
    /// <param name="outputBufferParamName">The name of the <paramref name="outputBuffer"/> parameter.</param>
    /// <param name="outputOffsetParamName">The name of the <paramref name="outputOffset"/> parameter.</param>
    /// <exception cref="ArgumentNullException"><paramref name="outputBuffer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="outputOffset"/> or <paramref name="requiredOutputCount"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="outputBuffer"/> does not have enough space for <paramref name="requiredOutputCount"/> bytes
    /// starting at <paramref name="outputOffset"/>.
    /// </exception>
    public static void ValidateOutputArguments(
        byte[] outputBuffer,
        int outputOffset,
        int requiredOutputCount,
        [CallerArgumentExpression(nameof(outputBuffer))] string? outputBufferParamName = null,
        [CallerArgumentExpression(nameof(outputOffset))] string? outputOffsetParamName = null)
    {
        ArgumentNullException.ThrowIfNull(outputBuffer, outputBufferParamName);
        ArgumentOutOfRangeException.ThrowIfNegative(outputOffset, outputOffsetParamName);
        ArgumentOutOfRangeException.ThrowIfNegative(requiredOutputCount);

        if (outputOffset > outputBuffer.Length - requiredOutputCount)
        {
            throw new ArgumentOutOfRangeException(
                outputBufferParamName,
                "Specified output buffer is too small.");
        }
    }
}
