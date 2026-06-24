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
    private protected Arc4ManagedBase()
    {
    }

    /// <inheritdoc/>
    public override void GenerateKey()
    {
        KeyValue = RandomNumberGenerator.GetBytes(KeySizeValue / 8);
    }

    /// <inheritdoc/>
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV)
    {
        return CreateTransform(rgbKey, rgbIV);
    }

    /// <inheritdoc/>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV)
    {
        return CreateTransform(rgbKey, rgbIV);
    }

    ICryptoTransform CreateTransform(
        byte[] key,
        byte[]? iv,
        [CallerArgumentExpression(nameof(key))] string? keyParamName = null,
        [CallerArgumentExpression(nameof(key))] string? ivParamName = null)
    {
        ValidateKeyArgument(key, keyParamName);
        ValidateIVArgument(iv, ivParamName);

        return new Arc4ManagedTransform(key);
    }

    void ValidateKeyArgument(byte[] key, [CallerArgumentExpression(nameof(key))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(key, paramName);

        long keySize = key.Length * 8;
        if (keySize > int.MaxValue || !ValidKeySize((int)keySize))
        {
            throw new ArgumentException(
                string.Format(Resources.SpecifiedKeyIsNotValidSizeForXAlgorithm, AlgorithmName),
                paramName);
        }
    }

    static void ValidateIVArgument(byte[]? iv, [CallerArgumentExpression(nameof(iv))] string? paramName = null)
    {
        if (iv?.Length > 0)
        {
            throw new ArgumentException(
                string.Format(Resources.XAlgorithmDoesNotSupportIV, AlgorithmName),
                paramName);
        }
    }
}
