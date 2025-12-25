using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides a managed implementation of the Alleged Rivest Cipher 4 (ARC4) algorithm.
/// </summary>
public sealed class Arc4Managed : Arc4
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Arc4Managed"/> class.
    /// </summary>
    public Arc4Managed()
    {
    }

    /// <inheritdoc/>
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV) => new Arc4ManagedTransform(rgbKey);

    /// <inheritdoc/>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV) => new Arc4ManagedTransform(rgbKey);

    /// <inheritdoc/>
    public override void GenerateKey() => KeyValue = Utils.GenerateRandomBytes(KeySizeValue / 8);
}
