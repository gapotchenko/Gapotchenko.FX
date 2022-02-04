using System;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography
{
    /// <summary>
    /// Represents the abstract base class from which all implementations of the Alleged Rivest Cipher 4 (ARC4) must inherit.
    /// </summary>
    public abstract class Arc4 : SymmetricAlgorithm
    {
        const int BLOCK_SIZE_BITS = 8;

        static readonly KeySizes[] m_LegalBlockSizes = { new(BLOCK_SIZE_BITS, BLOCK_SIZE_BITS, 0) };

        static readonly KeySizes[] m_LegalKeySizes = { new(40, 2048, 8) };

        /// <summary>
        /// Initializes a new instance of the <see cref="Arc4"/> class.
        /// </summary>
        public Arc4()
        {
            KeySizeValue = 128;
            BlockSizeValue = BLOCK_SIZE_BITS;
            FeedbackSizeValue = BLOCK_SIZE_BITS;
            LegalBlockSizesValue = m_LegalBlockSizes;
            LegalKeySizesValue = m_LegalKeySizes;

            ModeValue = CipherMode.ECB;
            PaddingValue = PaddingMode.None;
        }

        /// <summary>
        /// Gets or sets the initialization vector.
        /// ARC4 does not use initialization vector and does not allow to set it.
        /// </summary>
        public override byte[] IV
        {
            get => Empty<byte>.Array;
            set => throw new CryptographicException("Initialization vector cannot be set for ARC4 algorithm.");
        }

        /// <inheritdoc/>
        public override CipherMode Mode
        {
            get => base.Mode;
            set
            {
                if (value != CipherMode.ECB)
                {
                    throw new CryptographicException(
                        string.Format(
                            "Stream cipher does not support modes other than {0}.",
                            nameof(CipherMode.ECB)));
                }
            }
        }

        /// <inheritdoc/>
        public override PaddingMode Padding
        {
            get => base.Padding;
            set
            {
                if (value != PaddingMode.None)
                {
                    throw new CryptographicException(
                        string.Format(
                            "Stream cipher does not support paddings other than {0}.",
                            nameof(PaddingMode.None)));
                }
            }
        }

        /// <summary>
        /// Creates an instance of the default implementation of ARC4 algorithm.
        /// </summary>
        /// <returns>The instance of ARC4 algorithm.</returns>
        public new static Arc4 Create()
        {
            return new Arc4Managed();
        }
    }
}
