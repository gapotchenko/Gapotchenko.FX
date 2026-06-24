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

    public static ICryptoTransform Apply(
        ICryptoTransform cryptoTransform,
        CipherMode cipherMode,
        PaddingMode paddingMode,
        byte[]? iv,
        int feedbackSize)
    {
        // TODO: implement IV and missing modes/paddings.

        switch (cipherMode)
        {
            case CipherMode.ECB:
                break;
            default:
                throw new NotSupportedException(string.Format("{0} mode is not supported.", cipherMode));
        }

        switch (paddingMode)
        {
            case PaddingMode.None:
                break;
            default:
                throw new NotSupportedException(string.Format("{0} padding is not supported.", paddingMode));
        }

        return cryptoTransform;
    }
}
