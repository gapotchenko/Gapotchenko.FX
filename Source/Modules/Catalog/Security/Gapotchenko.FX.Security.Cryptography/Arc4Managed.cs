// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides a managed implementation of the Alleged Rivest Cipher 4 (ARC4) algorithm.
/// </summary>
[Obsolete("Use Arc4 type instead.")]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class Arc4Managed : Arc4
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Arc4Managed"/> class.
    /// </summary>
    [Obsolete("Use Arc4.Create() method instead.")]
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
