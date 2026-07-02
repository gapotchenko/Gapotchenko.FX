// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Security.Cryptography.Kits;
using Gapotchenko.FX.Security.Cryptography.Properties;

namespace Gapotchenko.FX.Security.Cryptography;

abstract class ManagedBlockTransform(int blockSize) : ICryptoTransform
{
    public virtual void Dispose()
    {
        m_Disposed = true;
    }

    public int InputBlockSize => blockSize;

    public int OutputBlockSize => blockSize;

    public bool CanTransformMultipleBlocks => true;

    public bool CanReuseTransform => true;

    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
    {
        CryptoTransformKit.ValidateInputArguments(inputBuffer, inputOffset, inputCount);
        CryptoTransformKit.ValidateOutputArguments(outputBuffer, outputOffset, inputCount);

        ValidateDataLength(inputCount);

        EnsureNotDisposed();

        for (int i = 0; i < inputCount; i += blockSize)
        {
            TransformBlockCore(
                inputBuffer.AsSpan(inputOffset + i, blockSize),
                outputBuffer.AsSpan(outputOffset + i, blockSize));
        }

        return inputCount;
    }

    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
        CryptoTransformKit.ValidateInputArguments(inputBuffer, inputOffset, inputCount);

        ValidateDataLength(inputCount);

        EnsureNotDisposed();

        if (inputCount == 0)
            return [];

        byte[] output = new byte[inputCount];

        for (int i = 0; i < inputCount; i += blockSize)
        {
            TransformBlockCore(
                inputBuffer.AsSpan(inputOffset + i, blockSize),
                output.AsSpan(i, blockSize));
        }

        return output;
    }

    void ValidateDataLength(int length)
    {
        if ((uint)length % blockSize != 0)
            throw new CryptographicException(Resources.InvalidLengthOfDataToTransform);
    }

    void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(m_Disposed, this);
    }

    bool m_Disposed;

    protected abstract void TransformBlockCore(ReadOnlySpan<byte> input, Span<byte> output);
}
