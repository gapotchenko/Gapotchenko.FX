using System;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography
{
    /// <summary>
    /// Provides a managed implementation of the Alleged Rivest Cipher 4 (ARC4) algorithm.
    /// </summary>
    public sealed class Arc4Managed : Arc4, ICryptoTransform
    {
        byte[]? m_Key;
        readonly byte[] m_State = new byte[256];
        byte m_X, m_Y;
        bool m_Disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Arc4Managed"/> class.
        /// </summary>
        public Arc4Managed()
        {
        }

        /// <summary>
        /// Finalizes the class instance.
        /// </summary>
        ~Arc4Managed() => Dispose(true);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                m_X = 0;
                m_Y = 0;

                if (m_Key != null)
                {
                    Array.Clear(m_Key, 0, m_Key.Length);
                    m_Key = null;
                }

                Array.Clear(m_State, 0, m_State.Length);

                GC.SuppressFinalize(this);
                m_Disposed = true;
            }
        }

        /// <inheritdoc/>
        public override byte[] Key
        {
            get
            {
                if (KeyValue == null)
                    GenerateKey();

                return (byte[])KeyValue!.Clone();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                var keySize = value.Length * 8;
                if (!ValidKeySize(keySize))
                    throw new CryptographicException("Specified key is not a valid size for this algorithm.");

                KeyValue = m_Key = (byte[])value.Clone();
                KeySizeValue = keySize;

                SetupKey(m_Key);
            }
        }

        /// <summary>
        /// Creates a symmetric encryptor object with the specified key and initialization vector.
        /// </summary>
        /// <param name="rgbKey">The key.</param>
        /// <param name="rgvIV">The initialization vector.</param>
        /// <returns>A symmetric encryptor object.</returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgvIV)
        {
            Key = rgbKey;
            return this;
        }

        /// <summary>
        /// Creates a symmetric decryptor object with the specified key and initialization vector.
        /// </summary>
        /// <param name="rgbKey">The key.</param>
        /// <param name="rgvIV">The initialization vector.</param>
        /// <returns>A symmetric decryptor object.</returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgvIV) => CreateEncryptor(rgbKey, rgvIV);

        /// <summary>
        /// Generates a random initialization vector to use for the algorithm.
        /// </summary>
        public override void GenerateIV() => throw new CryptographicException("ARC4 algorithm does not support initialization vectors.");

        /// <summary>
        /// Generates a random key to use for the algorithm.
        /// </summary>
        public override void GenerateKey()
        {
            var key = new byte[KeySizeValue / 8];

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            RandomNumberGenerator.Fill(key);
#else
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            rng.Dispose();
#endif

            KeyValue = key;
        }

        bool ICryptoTransform.CanReuseTransform => false;

        bool ICryptoTransform.CanTransformMultipleBlocks => true;

        int ICryptoTransform.InputBlockSize => 1;

        int ICryptoTransform.OutputBlockSize => 1;

        void SetupKey(byte[] key)
        {
            for (int counter = 0; counter < 256; ++counter)
                m_State[counter] = (byte)counter;

            m_X = 0;
            m_Y = 0;

            byte index1 = 0;
            byte index2 = 0;
            for (int i = 0; i < 256; ++i)
            {
                index2 = (byte)(key[index1] + m_State[i] + index2);
                SwapState(i, index2);
                index1 = (byte)((index1 + 1) % key.Length);
            }
        }

        void SwapState(int i, int j)
        {
            var state = m_State;

            var t = state[i];
            state[i] = state[j];
            state[j] = t;
        }

        static void CheckInputParameters(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null)
                throw new ArgumentNullException(nameof(inputBuffer));
            if (inputOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(inputOffset), "< 0");
            if (inputCount < 0)
                throw new ArgumentOutOfRangeException(nameof(inputCount), "< 0");

            if (inputOffset > inputBuffer.Length - inputCount)
                throw new ArgumentException(null, nameof(inputBuffer));
        }

        int ICryptoTransform.TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            CheckInputParameters(inputBuffer, inputOffset, inputCount);

            if (outputBuffer == null)
                throw new ArgumentNullException(nameof(outputBuffer));
            if (outputOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(outputOffset), "< 0");
            if (outputOffset > outputBuffer.Length - inputCount)
                throw new ArgumentException(null, nameof(outputBuffer));

            return TransformBlockCore(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
        }

        int TransformBlockCore(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            for (int i = 0; i < inputCount; ++i)
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ GetKeyStreamElement());
            return inputCount;
        }

        byte GetKeyStreamElement()
        {
            ++m_X;
            m_Y = (byte)(m_State[m_X] + m_Y);
            SwapState(m_X, m_Y);
            var keyIndex = (byte)(m_State[m_X] + m_State[m_Y]);
            return m_State[keyIndex];
        }

        byte[] ICryptoTransform.TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            CheckInputParameters(inputBuffer, inputOffset, inputCount);

            var output = new byte[inputCount];
            TransformBlockCore(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }
    }
}
