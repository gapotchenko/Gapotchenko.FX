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

    /// <summary>
    /// Creates a symmetric encryptor object with the specified key and initialization vector.
    /// </summary>
    /// <param name="rgbKey">The key.</param>
    /// <param name="rgvIV">The initialization vector.</param>
    /// <returns>A symmetric encryptor object.</returns>
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgvIV) => new Arc4ManagedTransform(rgbKey);

    /// <summary>
    /// Creates a symmetric decryptor object with the specified key and initialization vector.
    /// </summary>
    /// <param name="rgbKey">The key.</param>
    /// <param name="rgvIV">The initialization vector.</param>
    /// <returns>A symmetric decryptor object.</returns>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgvIV) => new Arc4ManagedTransform(rgbKey);

    /// <summary>
    /// Generates a random key to use for the algorithm.
    /// </summary>
    public override void GenerateKey() => KeyValue = Utils.GenerateRandomBytes(KeySizeValue / 8);
}
