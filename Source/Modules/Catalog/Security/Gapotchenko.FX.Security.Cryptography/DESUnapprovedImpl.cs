// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms

using Gapotchenko.FX.Security.Cryptography.Kits;
using Gapotchenko.FX.Security.Cryptography.Properties;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Security.Cryptography;

sealed class DESUnapprovedImpl : DES
{
    /// <inheritdoc/>
    public override void GenerateKey()
    {
        byte[] key = new byte[8];

        // Never hand back a weak or semi-weak key.
        do
        {
            RandomNumberGenerator.Fill(key);
        }
        while (IsWeakKey(key) || IsSemiWeakKey(key));

        KeyValue = key;
    }

    /// <inheritdoc/>
    public override void GenerateIV()
    {
        IVValue = RandomNumberGenerator.GetBytes(8);
    }

    /// <inheritdoc/>
    public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV)
    {
        return CreateTransform(rgbKey, rgbIV, false);
    }

    /// <inheritdoc/>
    public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV)
    {
        return CreateTransform(rgbKey, rgbIV, true);
    }

    ICryptoTransform CreateTransform(
        byte[] key,
        byte[]? iv,
        bool encrypting,
        [CallerArgumentExpression(nameof(key))] string? keyParamName = null,
        [CallerArgumentExpression(nameof(key))] string? ivParamName = null)
    {
        ValidateKeyArgument(key, keyParamName);
        ValidateIVArgument(iv, ivParamName);

        return CreateTransformCore(key, iv, encrypting);
    }

    void ValidateKeyArgument(byte[] key, [CallerArgumentExpression(nameof(key))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(key, paramName);

        long keySize = key.Length * 8L;
        if (keySize > int.MaxValue || !ValidKeySize((int)keySize))
        {
            throw new ArgumentException(
                string.Format(Resources.SpecifiedKeyIsNotValidSizeForXAlgorithm, AlgorithmName),
                paramName);
        }

        if (IsWeakKey(key))
            throw new CryptographicException(string.Format(Resources.SpecifiedKeyIsKnownWeakForXAlgorithm, AlgorithmName));
        if (IsSemiWeakKey(key))
            throw new CryptographicException(string.Format(Resources.SpecifiedKeyIsKnownSemiWeakForXAlgorithm, AlgorithmName));
    }

    void ValidateIVArgument(byte[]? iv, [CallerArgumentExpression(nameof(iv))] string? paramName = null)
    {
        if (iv != null)
        {
            long ivSize = iv.Length * 8L;
            if (ivSize != BlockSize)
                throw new ArgumentException(string.Format(Resources.IVDoesNotMatchXAlgorithmBlockSize, AlgorithmName), paramName);
        }
    }

    /// <summary>
    /// The display algorithm name.
    /// </summary>
    const string AlgorithmName = "DES";

    ICryptoTransform CreateTransformCore(
        byte[] key,
        byte[]? iv,
        bool encrypting)
    {
        return CryptoTransformKit.AdaptBlockTransform(
            this,
            new DESManagedTransform(key, encrypting),
            encrypting,
            iv);
    }
}
