// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Security.Cryptography.Properties;
using System.Runtime.CompilerServices;

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
        ValidateKeyArgument(rgbKey);
        ValidateIVArgument(rgbIV);

        return CreateTransform(rgbKey);
    }

    /// <inheritdoc/>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV)
    {
        ValidateKeyArgument(rgbKey);
        ValidateIVArgument(rgbIV);

        return CreateTransform(rgbKey);
    }

    void ValidateKeyArgument(byte[] key, [CallerArgumentExpression(nameof(key))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(key, paramName);

        if (!ValidKeySize(key.Length * 8))
        {
            throw new ArgumentException(
                string.Format(Resources.SpecifiedKeyIsNotValidSizeForXAlgorithm, Name),
                paramName);
        }
    }

    static void ValidateIVArgument(byte[]? iv, [CallerArgumentExpression(nameof(iv))] string? paramName = null)
    {
        if (iv?.Length > 0)
        {
            throw new ArgumentException(
                string.Format(Resources.XAlgorithmDoesNotSupportIV, Name),
                paramName);
        }
    }

    ICryptoTransform CreateTransform(byte[] key)
    {
        EnforceAlgorithmPolicy();

        return new Arc4ManagedTransform(key);
    }

    private protected abstract void EnforceAlgorithmPolicy();
}
