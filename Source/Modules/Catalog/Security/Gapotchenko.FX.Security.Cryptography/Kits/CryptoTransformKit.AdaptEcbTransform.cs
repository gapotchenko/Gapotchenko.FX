// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Security.Cryptography.Properties;
using System.Buffers;
using System.Diagnostics;

namespace Gapotchenko.FX.Security.Cryptography.Kits;

partial class CryptoTransformKit
{
    /// <summary>
    /// Adapts the specified ECB transform to the mode and padding behavior of the specified symmetric algorithm.
    /// </summary>
    /// <remarks>
    /// This method wraps an underlying ECB transform and adds cipher mode chaining, final-block padding,
    /// and initialization vector handling when those features are not implemented by the transform itself.
    /// </remarks>
    /// <param name="algorithm">
    /// The symmetric algorithm whose mode, padding, feedback size, and policy constraints are used to adapt
    /// <paramref name="transform"/>.
    /// </param>
    /// <param name="transform">
    /// The underlying ECB transform.
    /// </param>
    /// <param name="encrypting">
    /// <see langword="true"/> to create an encrypting transform; <see langword="false"/> to create a decrypting transform.
    /// </param>
    /// <param name="iv">
    /// The initialization vector, or <see langword="null"/> when the algorithm's cipher mode does not require one.
    /// </param>
    /// <returns>
    /// An <see cref="ICryptoTransform"/> that combines the underlying ECB transform with the algorithm's mode and padding behavior.
    /// </returns>
    public static ICryptoTransform AdaptEcbTransform(
        SymmetricAlgorithm algorithm,
        ICryptoTransform transform,
        bool encrypting,
        byte[]? iv)
    {
        ArgumentNullException.ThrowIfNull(algorithm);
        ArgumentNullException.ThrowIfNull(transform);

        var cipherMode = algorithm.Mode;
        if (cipherMode is CipherMode.CBC)
            ArgumentNullException.ThrowIfNull(iv);

        var paddingMode = algorithm.Padding;

        return
            (cipherMode, paddingMode) switch
            {
                (CipherMode.ECB, PaddingMode.None) when iv is null => transform,
                (CipherMode.ECB or CipherMode.CBC, _) => new AdaptedEcbTransform(transform, cipherMode, paddingMode, iv, encrypting),
                _ => throw new NotSupportedException(string.Format("{0} cipher mode is not supported.", cipherMode))
            };
    }

    sealed class AdaptedEcbTransform : ICryptoTransform
    {
        public AdaptedEcbTransform(
            ICryptoTransform transform,
            CipherMode cipherMode,
            PaddingMode paddingMode,
            byte[]? iv,
            bool encrypting)
        {
            switch (paddingMode)
            {
                case PaddingMode.None:
                case PaddingMode.Zeros:
                case PaddingMode.PKCS7:
                case PaddingMode.ANSIX923:
                case PaddingMode.ISO10126:
                    break;

                default:
                    throw new NotSupportedException(string.Format("{0} padding is not supported.", paddingMode));
            }

            m_Transform = transform;
            m_Encrypting = encrypting;
            m_CipherMode = cipherMode;
            m_PaddingMode = paddingMode;

            int blockSize = transform.InputBlockSize;
            if (blockSize <= 0 || transform.OutputBlockSize != blockSize)
                throw new ArgumentException("The crypto transform must have equal positive input and output block sizes.", nameof(transform));
            m_BlockSize = blockSize;

            m_CanReuseTransform = transform.CanReuseTransform;

            if (cipherMode is CipherMode.CBC)
            {
                ArgumentNullException.ThrowIfNull(iv);
                if (iv.Length != blockSize)
                    throw new ArgumentException("The initialization vector (IV) does not match the block size.", nameof(iv));

                m_IV = (byte[])iv.Clone();
            }
            else
            {
                m_IV = null;
            }

            m_Feedback = (byte[]?)m_IV?.Clone();
        }

        public bool CanReuseTransform => m_CanReuseTransform;

        // The adapter accepts multi-block input even when the underlying transform
        // only processes one block per TransformBlock call.
        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => m_BlockSize;

        public int OutputBlockSize => m_BlockSize;

        public void Dispose()
        {
            if (m_Transform is { } transform)
            {
                m_Transform = null;
                transform.Dispose();
            }

            if (m_Feedback is { } feedback)
            {
                m_Feedback = null;
                CryptographicOperations.ZeroMemory(feedback);
            }

            CryptographicOperations.ZeroMemory(m_IV);

            if (m_DeferredBlock is { } deferredBlock)
            {
                m_DeferredBlock = null;
                CryptographicOperations.ZeroMemory(deferredBlock);
            }
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            ValidateInputArguments(inputBuffer, inputOffset, inputCount);

            ValidateDataLength(inputCount);
            int outputCount = GetTransformBlockOutputCount(inputCount);
            ValidateOutputArguments(outputBuffer, outputOffset, outputCount);

            return TransformBlockCore(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ValidateInputArguments(inputBuffer, inputOffset, inputCount);

            if (!m_Encrypting)
                ValidateDataLength(inputCount);

            byte[] output = m_Encrypting ?
                TransformFinalBlockEncrypt(inputBuffer, inputOffset, inputCount) :
                TransformFinalBlockDecrypt(inputBuffer, inputOffset, inputCount);

            if (m_CanReuseTransform)
                Reset();

            return output;
        }

        void ValidateDataLength(int length)
        {
            if ((uint)length % m_BlockSize != 0)
                throw new CryptographicException(Resources.InvalidLengthOfDataToTransform);
        }

        void Reset()
        {
            if (m_IV is { } iv && m_Feedback is { } feedback)
                Buffer.BlockCopy(iv, 0, feedback, 0, feedback.Length);

            if (m_DeferredBlock is { } deferredBlock)
            {
                m_DeferredBlock = null;
                CryptographicOperations.ZeroMemory(deferredBlock);
            }
        }

        int GetTransformBlockOutputCount(int inputCount)
        {
            if (m_Encrypting || !PaddingRequiresFinalBlock)
                return inputCount;

            int blockSize = m_BlockSize;
            int blockCount = inputCount / blockSize;
            if (m_DeferredBlock != null)
                ++blockCount;

            return blockCount <= 1 ? 0 : (blockCount - 1) * blockSize;
        }

        int TransformBlockCore(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (!m_Encrypting && PaddingRequiresFinalBlock)
                return TransformBlockDecryptWithPadding(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);

            int inputEnd = inputOffset + inputCount;
            int initialOutputOffset = outputOffset;

            for (int i = inputOffset; i < inputEnd; i += m_BlockSize)
            {
                if (m_Encrypting)
                    EncryptBlock(inputBuffer, i, outputBuffer, outputOffset);
                else
                    DecryptBlock(inputBuffer, i, outputBuffer, outputOffset);

                outputOffset += m_BlockSize;
            }

            return outputOffset - initialOutputOffset;
        }

        int TransformBlockDecryptWithPadding(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            int blockSize = m_BlockSize;
            int inputEnd = inputOffset + inputCount;
            int initialOutputOffset = outputOffset;

            if (m_DeferredBlock is { } deferredBlock)
            {
                if (inputCount == 0)
                    return 0;

                DecryptBlock(deferredBlock, 0, outputBuffer, outputOffset);
                outputOffset += blockSize;

                m_DeferredBlock = null;
                CryptographicOperations.ZeroMemory(deferredBlock);
            }

            while (inputOffset < inputEnd - blockSize)
            {
                DecryptBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);
                inputOffset += blockSize;
                outputOffset += blockSize;
            }

            if (inputOffset < inputEnd)
            {
                m_DeferredBlock = new byte[blockSize];
                Buffer.BlockCopy(inputBuffer, inputOffset, m_DeferredBlock, 0, blockSize);
            }

            return outputOffset - initialOutputOffset;
        }

        byte[] TransformFinalBlockEncrypt(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            int padLength = GetPadLength(inputCount);
            int paddedInputCount = inputCount + padLength;

            if (paddedInputCount == 0)
                return [];
            byte[] output = new byte[paddedInputCount];

            if (padLength == 0)
            {
                TransformBlockCore(inputBuffer, inputOffset, inputCount, output, 0);
            }
            else
            {
                var arrayPool = ArrayPool<byte>.Shared;
                byte[] paddedInput = arrayPool.Rent(paddedInputCount);
                try
                {
                    Buffer.BlockCopy(inputBuffer, inputOffset, paddedInput, 0, inputCount);
                    WritePadding(paddedInput.AsSpan(inputCount, padLength));

                    TransformBlockCore(paddedInput, 0, paddedInputCount, output, 0);
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(paddedInput.AsSpan(0, paddedInputCount));
                    arrayPool.Return(paddedInput);
                }
            }

            return output;
        }

        byte[] TransformFinalBlockDecrypt(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            int blockSize = m_BlockSize;
            bool paddingRequiresFinalBlock = PaddingRequiresFinalBlock;

            int outputCount;
            if (paddingRequiresFinalBlock)
            {
                int blockCount = inputCount / blockSize;
                if (m_DeferredBlock != null)
                    ++blockCount;
                if (blockCount == 0)
                    throw new CryptographicException(Resources.InvalidPadding);

                outputCount = blockCount * blockSize;
            }
            else
            {
                outputCount = inputCount;
            }

            byte[] output = new byte[outputCount];
            int outputOffset = 0;

            if (m_DeferredBlock is { } deferredBlock)
            {
                DecryptBlock(deferredBlock, 0, output, outputOffset);
                outputOffset += blockSize;

                m_DeferredBlock = null;
                CryptographicOperations.ZeroMemory(deferredBlock);
            }

            int inputEnd = inputOffset + inputCount;
            for (int i = inputOffset; i < inputEnd; i += blockSize)
            {
                DecryptBlock(inputBuffer, i, output, outputOffset);
                outputOffset += blockSize;
            }

            if (paddingRequiresFinalBlock)
            {
                int padLength = GetPaddingLength(output);
                Array.Resize(ref output, output.Length - padLength);
            }

            return output;
        }

        int GetPadLength(int inputCount)
        {
            int blockSize = m_BlockSize;
            int remainder = inputCount % blockSize;

            return
                m_PaddingMode switch
                {
                    PaddingMode.None when remainder == 0 => 0,
                    PaddingMode.None => throw new CryptographicException(Resources.InvalidLengthOfDataToTransform),
                    PaddingMode.Zeros => remainder == 0 ? 0 : blockSize - remainder,
                    _ => blockSize - remainder
                };
        }

        void WritePadding(Span<byte> buffer)
        {
            int padLength = buffer.Length;
            if (padLength == 0)
                return;

            switch (m_PaddingMode)
            {
                case PaddingMode.Zeros:
                    buffer.Clear();
                    break;

                case PaddingMode.PKCS7:
                    buffer.Fill((byte)padLength);
                    break;

                case PaddingMode.ANSIX923:
                    if (padLength > 1)
                        buffer[..(padLength - 1)].Clear();
                    buffer[^1] = (byte)padLength;
                    break;

                case PaddingMode.ISO10126:
                    if (padLength > 1)
                        RandomNumberGenerator.Fill(buffer[..(padLength - 1)]);
                    buffer[^1] = (byte)padLength;
                    break;
            }
        }

        int GetPaddingLength(byte[] output)
        {
            int blockSize = m_BlockSize;
            int padLength = output[^1];

            if (padLength <= 0 || padLength > blockSize || padLength > output.Length)
                throw new CryptographicException(Resources.InvalidPadding);

            switch (m_PaddingMode)
            {
                case PaddingMode.PKCS7:
                    for (int i = output.Length - padLength; i < output.Length; ++i)
                    {
                        if (output[i] != padLength)
                            throw new CryptographicException(Resources.InvalidPadding);
                    }
                    break;

                case PaddingMode.ANSIX923:
                    for (int i = output.Length - padLength; i < output.Length - 1; ++i)
                    {
                        if (output[i] != 0)
                            throw new CryptographicException(Resources.InvalidPadding);
                    }
                    break;
            }

            return padLength;
        }

        void EncryptBlock(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            if (m_Feedback is { } feedback)
            {
                int blockSize = m_BlockSize;

                var arrayPool = ArrayPool<byte>.Shared;
                byte[] block = arrayPool.Rent(blockSize);
                try
                {
                    for (int i = 0; i < blockSize; ++i)
                        block[i] = (byte)(inputBuffer[inputOffset + i] ^ feedback[i]);

                    TransformEcbBlock(block, 0, outputBuffer, outputOffset);
                    Buffer.BlockCopy(outputBuffer, outputOffset, feedback, 0, blockSize);
                }
                finally
                {
                    CryptographicOperations.ZeroMemory(block.AsSpan(0, blockSize));
                    arrayPool.Return(block);
                }
            }
            else
            {
                TransformEcbBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);
            }
        }

        void DecryptBlock(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            if (m_Feedback is { } feedback)
            {
                int blockSize = m_BlockSize;

                var arrayPool = ArrayPool<byte>.Shared;
                byte[]? cipherBlock = null;
                byte[] block = arrayPool.Rent(blockSize);
                try
                {
                    byte[] feedbackSource = inputBuffer;
                    int feedbackSourceOffset = inputOffset;
                    if (ReferenceEquals(inputBuffer, outputBuffer))
                    {
                        cipherBlock = arrayPool.Rent(blockSize);
                        Buffer.BlockCopy(inputBuffer, inputOffset, cipherBlock, 0, blockSize);
                        feedbackSource = cipherBlock;
                        feedbackSourceOffset = 0;
                    }

                    TransformEcbBlock(inputBuffer, inputOffset, block, 0);

                    for (int i = 0; i < blockSize; ++i)
                        outputBuffer[outputOffset + i] = (byte)(block[i] ^ feedback[i]);

                    Buffer.BlockCopy(feedbackSource, feedbackSourceOffset, feedback, 0, blockSize);
                }
                finally
                {
                    if (cipherBlock != null)
                    {
                        CryptographicOperations.ZeroMemory(cipherBlock.AsSpan(0, blockSize));
                        arrayPool.Return(cipherBlock);
                    }

                    CryptographicOperations.ZeroMemory(block.AsSpan(0, blockSize));
                    arrayPool.Return(block);
                }
            }
            else
            {
                TransformEcbBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);
            }
        }

        void TransformEcbBlock(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            var transform = GetTransform();

            int blockSize = m_BlockSize;
            int bytesWritten = transform.TransformBlock(inputBuffer, inputOffset, blockSize, outputBuffer, outputOffset);
            if (bytesWritten != blockSize)
                throw new CryptographicException("The underlying transform produced an invalid block size.");
        }

        bool PaddingRequiresFinalBlock =>
            !m_Encrypting &&
            m_PaddingMode is PaddingMode.PKCS7 or PaddingMode.ANSIX923 or PaddingMode.ISO10126;

        ICryptoTransform GetTransform()
        {
            var transform = m_Transform;
            ObjectDisposedException.ThrowIf(transform is null, this);
            return transform;
        }

        /// <summary>
        /// The underlying transform.
        /// </summary>
        ICryptoTransform? m_Transform;

        readonly bool m_Encrypting;
        readonly int m_BlockSize;
        readonly CipherMode m_CipherMode;
        readonly PaddingMode m_PaddingMode;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly bool m_CanReuseTransform;

        readonly byte[]? m_IV;
        byte[]? m_Feedback;

        byte[]? m_DeferredBlock;
    }
}
