// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using Gapotchenko.FX.Security.Cryptography.Properties;
using System.Diagnostics;

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Represents the abstract base class from which all implementations of the Alleged Rivest Cipher 4 (ARC4) must inherit.
/// </summary>
public abstract class Arc4 : SymmetricAlgorithm
{
    /// <summary>
    /// Creates an instance of the default implementation of ARC4 algorithm.
    /// </summary>
    /// <returns>The instance of ARC4 algorithm.</returns>
    public static new Arc4 Create()
    {
        EnforcePolicies();

#pragma warning disable CS0618 // Type or member is obsolete
        return new Arc4Managed(false);
#pragma warning restore CS0618
    }

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

    const int BLOCK_SIZE_BITS = 8;
    static readonly KeySizes[] m_LegalBlockSizes = [new(BLOCK_SIZE_BITS, BLOCK_SIZE_BITS, 0)];
    static readonly KeySizes[] m_LegalKeySizes = [new(40, 2048, 8)];

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
                        Resources.XAlgorithmDoesNotSupportModesExceptY,
                        AlgorithmName,
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
                        Resources.XAlgorithmDoesNotSupportPaddingsExceptY,
                        AlgorithmName,
                        nameof(PaddingMode.None)));
            }
        }
    }

    /// <inheritdoc/>
    public override byte[] IV
    {
        get => [];
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.Length != 0)
                ThrowDoesNotSupportIV();
        }
    }

    /// <inheritdoc/>
    public override void GenerateIV()
    {
        ThrowDoesNotSupportIV();
    }

    [DoesNotReturn, StackTraceHidden]
    static void ThrowDoesNotSupportIV()
    {
        throw new CryptographicException(string.Format(Resources.XAlgorithmDoesNotSupportIV, AlgorithmName));
    }

    private protected static void EnforcePolicies()
    {
        if (CryptographyPolicy.AllowOnlyFipsAlgorithms)
            throw new CryptographicException(string.Format(Resources.XAlgorithmCannotBeUsedDueFips, AlgorithmName));
    }

    /// <summary>
    /// The algorithm display name.
    /// </summary>
    private protected const string AlgorithmName = "ARC4";
}
