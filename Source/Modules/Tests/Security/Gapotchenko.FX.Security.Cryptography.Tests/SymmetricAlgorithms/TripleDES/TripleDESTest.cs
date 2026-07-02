// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
// Portions © The Legion of the Bouncy Castle Inc.
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Data.Encoding;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms.TripleDES;

using TripleDES = System.Security.Cryptography.TripleDES;

public abstract class TripleDESTest : SymmetricAlgorithmTest
{
    #region Keys

    [TestMethod]
    public void TripleDES_Key_Size()
    {
        using var algorithm = CreateAlgorithm();
        Assert.AreEqual(192, algorithm.KeySize);

        foreach (int keySize in new int[] { 128, 192 })
        {
            algorithm.KeySize = keySize;
            Assert.AreEqual(keySize, algorithm.KeySize);

            Assert.ThrowsExactly<CryptographicException>(() => algorithm.KeySize = keySize - 8);
            Assert.ThrowsExactly<CryptographicException>(() => algorithm.KeySize = keySize + 8);
        }
    }

    [TestMethod]
    [DynamicData(nameof(TripleDES_Key_Bad_Data))]
    public void TripleDES_Key_Weak(byte[] key)
    {
        Assert.IsTrue(IsWeakKey(key));

        using var algorithm = CreateAlgorithm();
        Assert.ThrowsExactly<CryptographicException>(() => algorithm.Key = key);
    }

    static IEnumerable<ValueTuple<byte[]>> TripleDES_Key_Bad_Data
    {
        get
        {
            foreach (byte[] key in m_WeakKeys)
            {
                yield return ValueTuple.Create(key);

                byte[] oddKey = RemoveDesParityBits(key);
                if (!oddKey.SequenceEqual(key))
                    yield return ValueTuple.Create(oddKey);
            }
        }
    }

    static byte[] RemoveDesParityBits(byte[] key)
    {
        int n = key.Length;
        byte[] oddKey = new byte[n];
        for (int i = 0; i < n; i++)
            oddKey[i] = (byte)(key[i] & 0xfe);
        return oddKey;
    }

    static readonly byte[][] m_WeakKeys =
    [
        Base16.GetBytes("00000000000000000000000000000000"),
        Base16.GetBytes("bbbbbbbbbbbbbbbb00000000000000000000000000000000"),
        Base16.GetBytes("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb0000000000000000")
    ];

    #endregion

    #region Blocks

    [TestMethod]
    public void TripleDES_Block_Size()
    {
        using var algorithm = CreateAlgorithm();
        Assert.AreEqual(64, algorithm.BlockSize);

        algorithm.BlockSize = 64;
        Assert.AreEqual(64, algorithm.BlockSize);

        Assert.ThrowsExactly<CryptographicException>(() => algorithm.BlockSize = 63);
        Assert.ThrowsExactly<CryptographicException>(() => algorithm.BlockSize = 65);
    }

    #endregion

    #region Cipher

    [TestMethod]
    [DataRow("0123456789abcdeffedcba9876543210", "4e6f77206973207468652074696d6520666f7220616c6c20", "d80a0d8b2bae5e4e6a0094171abcfc2775d2235a706e232c")]
    [DataRow("0123456789abcdeffedcba98765432100123456789abcdef", "4e6f77206973207468652074696d6520666f7220616c6c20", "d80a0d8b2bae5e4e6a0094171abcfc2775d2235a706e232c")]
    public void TripleDES_Cipher_TV(
        string key,
        string input,
        string output,
        CipherMode mode = CipherMode.ECB,
        PaddingMode padding = PaddingMode.None,
        string? iv = null)
    {
        byte[] keyData = Base16.GetBytes(key);
        byte[] expectedPlainText = Base16.GetBytes(input);
        byte[] expectedCipher = Base16.GetBytes(output);
        byte[]? ivData = iv is null ? null : Base16.GetBytes(iv);

        using var algorithm = CreateAlgorithm();
        algorithm.Mode = mode;
        algorithm.Padding = padding;
        algorithm.Key = keyData;
        if (ivData != null)
            algorithm.IV = ivData;

        byte[] actualCipher = algorithm.Encrypt(expectedPlainText);
        CollectionAssert.AreEqual(expectedCipher, actualCipher);

        byte[] actualPlainText = algorithm.Decrypt(actualCipher);
        CollectionAssert.AreEqual(expectedPlainText, actualPlainText);
    }

    #endregion

    // ------------------------------------------------------------------------

    protected sealed override SymmetricAlgorithm CreateSymmetricAlgorithm() => CreateAlgorithm();

    protected abstract TripleDES CreateAlgorithm();

    protected virtual bool IsWeakKey(byte[] key)
    {
        return TripleDES.IsWeakKey(key);
    }
}
