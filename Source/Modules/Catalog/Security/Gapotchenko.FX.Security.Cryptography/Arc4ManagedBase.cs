// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography;

#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY // 2026
/// <summary>
/// This is an infrastructure type that should never be used by user code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public
#endif
abstract class Arc4ManagedBase : Arc4
{
    internal Arc4ManagedBase()
    {
    }

    /// <inheritdoc/>
    public override void GenerateKey()
    {
        KeyValue = Utils.GenerateRandomBytes(KeySizeValue / 8);
    }

    /// <inheritdoc/>
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV)
    {
        ArgumentNullException.ThrowIfNull(rgbKey);

        return CreateTransform(rgbKey);
    }

    /// <inheritdoc/>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV)
    {
        ArgumentNullException.ThrowIfNull(rgbKey);

        return CreateTransform(rgbKey);
    }

    ICryptoTransform CreateTransform(byte[] key)
    {
        EnforceAlgorithmPolicy();

        return new Arc4ManagedTransform(key);
    }

    private protected abstract void EnforceAlgorithmPolicy();
}
