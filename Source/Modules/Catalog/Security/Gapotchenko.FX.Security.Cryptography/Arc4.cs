using System.Diagnostics;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Represents the abstract base class from which all implementations of the Alleged Rivest Cipher 4 (ARC4) must inherit.
/// </summary>
public abstract class Arc4 : SymmetricAlgorithm
{
    const int BLOCK_SIZE_BITS = 8;

    static readonly KeySizes[] m_LegalBlockSizes = [new(BLOCK_SIZE_BITS, BLOCK_SIZE_BITS, 0)];

    static readonly KeySizes[] m_LegalKeySizes = [new(40, 2048, 8)];

    /// <summary>
    /// Initializes a new instance of the <see cref="Arc4"/> class.
    /// </summary>
    protected Arc4()
    {
        KeySizeValue = 128;
        BlockSizeValue = BLOCK_SIZE_BITS;
        FeedbackSizeValue = BLOCK_SIZE_BITS;
        LegalBlockSizesValue = m_LegalBlockSizes;
        LegalKeySizesValue = m_LegalKeySizes;

        ModeValue = CipherMode.ECB;
        PaddingValue = PaddingMode.None;

        IVValue = [];
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
                        "ARC4 algorithm does not support modes other than {0}.",
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
                        "ARC4 algorithm does not support paddings other than {0}.",
                        nameof(PaddingMode.None)));
            }
        }
    }

    /// <summary>
    /// Gets or sets the initialization vector.
    /// </summary>
    public override byte[] IV
    {
        get => base.IV;
        set
        {
            if (value?.Length > 0)
                ThrowDoesNotSupportIV();
            base.IV = value!;
        }
    }

    [DoesNotReturn, StackTraceHidden]
    static void ThrowDoesNotSupportIV() => throw new CryptographicException("ARC4 algorithm does not support initialization vector.");

    /// <summary>
    /// Generates a random initialization vector to use for the algorithm.
    /// </summary>
    public override void GenerateIV() => ThrowDoesNotSupportIV();

    /// <summary>
    /// Creates an instance of the default implementation of ARC4 algorithm.
    /// </summary>
    /// <returns>The instance of ARC4 algorithm.</returns>
    public static new Arc4 Create() => new Arc4Managed();
}
