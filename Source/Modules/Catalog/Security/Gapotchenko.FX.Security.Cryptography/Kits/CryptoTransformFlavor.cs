namespace Gapotchenko.FX.Security.Cryptography.Kits;

/// <summary>
/// Provides a way to add cipher modes / cipher padding / IV support to an existing <see cref="ICryptoTransform"/>
/// of a <see cref="SymmetricAlgorithm"/> that does not implement them natively.
/// </summary>
public sealed class CryptoTransformFlavor
{
    CryptoTransformFlavor()
    {
    }

    /// <summary>
    /// Applies a cipher mode, padding mode, and initialization vector to a block transform.
    /// </summary>
    /// <param name="transform">The underlying electronic codebook block transform.</param>
    /// <param name="encrypting"><see langword="true"/> to encrypt; <see langword="false"/> to decrypt.</param>
    /// <param name="cipherMode">The cipher mode to apply.</param>
    /// <param name="paddingMode">The padding mode to apply.</param>
    /// <param name="iv">The initialization vector.</param>
    /// <param name="feedbackSize">The feedback size, in bits.</param>
    /// <returns>The flavored crypto transform.</returns>
    public static ICryptoTransform Apply(
        ICryptoTransform transform,
        bool encrypting,
        CipherMode cipherMode,
        PaddingMode paddingMode,
        byte[]? iv,
        int feedbackSize)
    {
        ArgumentNullException.ThrowIfNull(transform);
        if (cipherMode == CipherMode.CBC)
            ArgumentNullException.ThrowIfNull(iv);

        _ = feedbackSize; // TODO: use later for cipher modes that need it

        return
            (cipherMode, paddingMode) switch
            {
                (CipherMode.ECB, PaddingMode.None) when iv is null => transform,
                (CipherMode.ECB or CipherMode.CBC, _) => new Transform(transform, cipherMode, paddingMode, iv, encrypting),
                _ => throw new NotSupportedException(string.Format("{0} cipher mode is not supported.", cipherMode))
            };
    }

    sealed class Transform : ICryptoTransform
    {
        public Transform(
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

            if (cipherMode == CipherMode.CBC)
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

        public bool CanReuseTransform => GetTransform().CanReuseTransform;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => BlockSize;

        public int OutputBlockSize => BlockSize;

        int BlockSize => m_BlockSize;

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
            ValidateInputParameters(inputBuffer, inputOffset, inputCount);

            ArgumentNullException.ThrowIfNull(outputBuffer);
            ArgumentOutOfRangeException.ThrowIfNegative(outputOffset);

            int blockSize = BlockSize;
            if (inputCount % blockSize != 0)
                throw new CryptographicException("Length of the data to transform is invalid.");

            int outputCount = GetTransformBlockOutputCount(inputCount);
            if (outputOffset > outputBuffer.Length - outputCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(outputBuffer),
                    "Specified output buffer is too small.");
            }

            return TransformBlockCore(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ValidateInputParameters(inputBuffer, inputOffset, inputCount);

            int blockSize = BlockSize;
            if (!m_Encrypting && inputCount % blockSize != 0)
                throw new CryptographicException("Length of the data to decrypt is invalid.");

            byte[] output = m_Encrypting ?
                TransformFinalBlockEncrypt(inputBuffer, inputOffset, inputCount) :
                TransformFinalBlockDecrypt(inputBuffer, inputOffset, inputCount);

            if (CanReuseTransform)
                Reset();

            return output;
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

        static void ValidateInputParameters(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ArgumentNullException.ThrowIfNull(inputBuffer);
            ArgumentOutOfRangeException.ThrowIfNegative(inputOffset);
            ArgumentOutOfRangeException.ThrowIfNegative(inputCount);

            if (inputOffset > inputBuffer.Length - inputCount)
            {
                throw new ArgumentException(
                    "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.",
                    nameof(inputBuffer));
            }
        }

        int GetTransformBlockOutputCount(int inputCount)
        {
            if (m_Encrypting || !PaddingRequiresFinalBlock)
                return inputCount;

            int blockSize = BlockSize;
            int blockCount = inputCount / blockSize;
            if (m_DeferredBlock != null)
                ++blockCount;

            return blockCount <= 1 ? 0 : (blockCount - 1) * blockSize;
        }

        int TransformBlockCore(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (!m_Encrypting && PaddingRequiresFinalBlock)
                return TransformBlockDecryptWithPadding(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);

            int blockSize = BlockSize;
            int inputEnd = inputOffset + inputCount;
            int initialOutputOffset = outputOffset;

            for (int i = inputOffset; i < inputEnd; i += blockSize)
            {
                if (m_Encrypting)
                    EncryptBlock(inputBuffer, i, outputBuffer, outputOffset);
                else
                    DecryptBlock(inputBuffer, i, outputBuffer, outputOffset);

                outputOffset += blockSize;
            }

            return outputOffset - initialOutputOffset;
        }

        int TransformBlockDecryptWithPadding(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            int blockSize = BlockSize;
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
            int blockSize = BlockSize;
            int padLength = GetPadLength(inputCount, blockSize);
            int paddedInputCount = inputCount + padLength;

            if (paddedInputCount == 0)
                return [];

            byte[] output = new byte[paddedInputCount];

            byte[] paddedInput = new byte[paddedInputCount];
            Buffer.BlockCopy(inputBuffer, inputOffset, paddedInput, 0, inputCount);
            PadBlock(paddedInput, inputCount, padLength);

            TransformBlockCore(paddedInput, 0, paddedInput.Length, output, 0);
            CryptographicOperations.ZeroMemory(paddedInput);

            return output;
        }

        byte[] TransformFinalBlockDecrypt(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            int blockSize = BlockSize;
            bool paddingRequiresFinalBlock = PaddingRequiresFinalBlock;

            int outputCount;
            if (paddingRequiresFinalBlock)
            {
                int blockCount = inputCount / blockSize;
                if (m_DeferredBlock != null)
                    ++blockCount;
                if (blockCount == 0)
                    throw new CryptographicException("Padding is invalid and cannot be removed.");

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

        int GetPadLength(int inputCount, int blockSize)
        {
            int remainder = inputCount % blockSize;

            return
                m_PaddingMode switch
                {
                    PaddingMode.None when remainder == 0 => 0,
                    PaddingMode.None => throw new CryptographicException("Length of the data to encrypt is invalid."),
                    PaddingMode.Zeros => remainder == 0 ? 0 : blockSize - remainder,
                    _ => blockSize - remainder
                };
        }

        void PadBlock(byte[] buffer, int inputCount, int padLength)
        {
            if (padLength == 0)
                return;

            switch (m_PaddingMode)
            {
                case PaddingMode.Zeros:
                    break;

                case PaddingMode.PKCS7:
                    Array.Fill(buffer, (byte)padLength, inputCount, padLength);
                    break;

                case PaddingMode.ANSIX923:
                    buffer[^1] = (byte)padLength;
                    break;

                case PaddingMode.ISO10126:
                    if (padLength > 1)
                        RandomNumberGenerator.Fill(buffer.AsSpan(inputCount, padLength - 1));
                    buffer[^1] = (byte)padLength;
                    break;
            }
        }

        int GetPaddingLength(byte[] output)
        {
            int blockSize = BlockSize;
            int padLength = output[^1];

            if (padLength <= 0 || padLength > blockSize || padLength > output.Length)
                throw new CryptographicException("Padding is invalid and cannot be removed.");

            switch (m_PaddingMode)
            {
                case PaddingMode.PKCS7:
                    for (int i = output.Length - padLength; i < output.Length; ++i)
                    {
                        if (output[i] != padLength)
                            throw new CryptographicException("Padding is invalid and cannot be removed.");
                    }
                    break;

                case PaddingMode.ANSIX923:
                    for (int i = output.Length - padLength; i < output.Length - 1; ++i)
                    {
                        if (output[i] != 0)
                            throw new CryptographicException("Padding is invalid and cannot be removed.");
                    }
                    break;
            }

            return padLength;
        }

        void EncryptBlock(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            if (m_Feedback is { } feedback)
            {
                int blockSize = BlockSize;
                byte[] block = new byte[blockSize];
                for (int i = 0; i < blockSize; ++i)
                    block[i] = (byte)(inputBuffer[inputOffset + i] ^ feedback[i]);

                TransformEcbBlock(block, 0, outputBuffer, outputOffset);
                Buffer.BlockCopy(outputBuffer, outputOffset, feedback, 0, blockSize);
                CryptographicOperations.ZeroMemory(block);
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
                int blockSize = BlockSize;
                byte[] block = new byte[blockSize];
                TransformEcbBlock(inputBuffer, inputOffset, block, 0);

                for (int i = 0; i < blockSize; ++i)
                    outputBuffer[outputOffset + i] = (byte)(block[i] ^ feedback[i]);

                Buffer.BlockCopy(inputBuffer, inputOffset, feedback, 0, blockSize);
                CryptographicOperations.ZeroMemory(block);
            }
            else
            {
                TransformEcbBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);
            }
        }

        void TransformEcbBlock(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            var transform = GetTransform();

            int bytesWritten = transform.TransformBlock(inputBuffer, inputOffset, BlockSize, outputBuffer, outputOffset);
            if (bytesWritten != BlockSize)
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

        readonly byte[]? m_IV;
        byte[]? m_Feedback;

        byte[]? m_DeferredBlock;
    }
}
