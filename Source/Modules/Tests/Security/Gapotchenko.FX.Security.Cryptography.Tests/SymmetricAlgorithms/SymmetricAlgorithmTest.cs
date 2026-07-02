// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms;

public abstract class SymmetricAlgorithmTest
{
    #region Keys

    [TestMethod]
    public void SymmetricAlgorithm_Key_LegalSizes()
    {
        using var algorithm = CreateSymmetricAlgorithm();

        var legalKeySizes = algorithm.LegalKeySizes;
        Assert.IsNotEmpty(legalKeySizes);

        foreach (var legalKeySize in legalKeySizes)
        {
            int minSize = legalKeySize.MinSize;
            Assert.IsPositive(minSize, "MinSize >= 0");

            int maxSize = legalKeySize.MaxSize;
            Assert.IsPositive(maxSize, "MaxSize >= 0");

            Assert.IsGreaterThanOrEqualTo(minSize, maxSize, "MinSize <= MaxSize");

            int skipSize = legalKeySize.SkipSize;
            Assert.IsGreaterThanOrEqualTo(0, skipSize, "SkipSize >= 0");

            if (skipSize == 0)
                Assert.AreEqual(minSize, maxSize, "MinSize = MaxSize when SkipSize = 0");
        }

        foreach (int legalKeySize in EnumerateLegalKeySizes(algorithm))
            Assert.IsTrue(algorithm.ValidKeySize(legalKeySize));
    }

    [TestMethod]
    public void SymmetricAlgorithm_Key_Sizes()
    {
        using var algorithm = CreateSymmetricAlgorithm();

        int defaultKeySize = algorithm.KeySize;
        algorithm.KeySize = defaultKeySize;
        Assert.AreEqual(defaultKeySize, algorithm.KeySize);

        bool defaultKeySizeIsLegal = false;
        foreach (int legalKeySize in EnumerateLegalKeySizes(algorithm))
        {
            algorithm.KeySize = legalKeySize;
            Assert.AreEqual(legalKeySize, algorithm.KeySize);

            defaultKeySizeIsLegal |= legalKeySize == defaultKeySize;
        }

        Assert.IsTrue(defaultKeySizeIsLegal);
    }

    static IEnumerable<int> EnumerateLegalKeySizes(SymmetricAlgorithm algorithm)
    {
        foreach (var i in algorithm.LegalKeySizes)
        {
            int minSize = i.MinSize;
            int maxSize = i.MaxSize;
            int skipSize = i.SkipSize;

            if (skipSize == 0)
            {
                yield return minSize;
            }
            else
            {
                for (int size = minSize; size <= maxSize; size += skipSize)
                    yield return size;
            }
        }
    }

    #endregion

    #region IV

    [TestMethod]
    public void SymmetricAlgorithm_IV()
    {
        using var algorithm = CreateSymmetricAlgorithm();

        byte[] iv0 = algorithm.IV;
        if (iv0 is [])
        {
            // The algorithm does not support IV.

            Assert.ThrowsExactly<CryptographicException>(
                algorithm.GenerateIV,
                "Symmetric algorithm not supporting IV should disallow its generation.");

            Assert.IsEmpty(
                algorithm.IV,
                "Symmetric algorithm not supporting IV should not actually generate it.");

            algorithm.IV = [];
            Assert.ThrowsExactly<CryptographicException>(() => algorithm.IV = [0]);
        }
        else
        {
            // The algorithm supports IV.

            int ivRank = iv0.Length;

            algorithm.GenerateIV();
            byte[] iv1 = algorithm.IV;
            Assert.HasCount(ivRank, iv1, "IV generation function should produce an IV of the same length.");
            CollectionAssert.AreNotEqual(iv0, iv1, "IV generation function should not produce the same IV.");

            var algorithm2 = CreateSymmetricAlgorithm();
            byte[] iv2 = algorithm.IV;
            Assert.HasCount(ivRank, iv2, "Symmetric algorithm should generate an IV of the same length after creation.");
            CollectionAssert.AreNotEqual(iv0, iv2, "Symmetric algorithm should generate unique IV after creation.");

            byte[] customIV = RandomNumberGenerator.GetBytes(ivRank);
            algorithm.IV = customIV;
            CollectionAssert.AreEqual(customIV, algorithm.IV, "Symmetric algorithm should preserve a previously set IV.");
        }

        Assert.ThrowsExactly<ArgumentNullException>(() => algorithm.IV = null!);
    }

    #endregion

    #region Cipher

    [TestMethod]
    public void SymmetricAlgorithm_Cipher_Arguments()
    {
        using var algorithm = CreateSymmetricAlgorithm();

        byte[] iv = algorithm.IV;

        if (ThrowsCryptographicExceptionOnInvalidCipherArguments)
        {
            Assert.ThrowsExactly<CryptographicException>(() => algorithm.CreateDecryptor(null!, iv));
            Assert.ThrowsExactly<CryptographicException>(() => algorithm.CreateEncryptor(null!, iv));
        }
        else
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => algorithm.CreateDecryptor(null!, iv));
            Assert.ThrowsExactly<ArgumentNullException>(() => algorithm.CreateEncryptor(null!, iv));
        }

        if (iv is [])
        {
            algorithm.CreateDecryptor(algorithm.Key, null).Dispose();
            algorithm.CreateEncryptor(algorithm.Key, null).Dispose();
        }
    }

    [TestMethod]
    public void SymmetricAlgorithm_Cipher_ReuseTransform()
    {
        using var algorithm = CreateSymmetricAlgorithm();

        using var encryptor = algorithm.CreateEncryptor();
        using var decryptor = algorithm.CreateDecryptor();

        byte[] plain1 = RandomNumberGenerator.GetBytes(RandomNumberGenerator.GetInt32(1, 64));
        byte[] plain2 = RandomNumberGenerator.GetBytes(RandomNumberGenerator.GetInt32(1, 64));
        byte[]? expectedCipher1 = null;
        byte[]? expectedCipher2 = null;

        for (int i = 0; i < 2; ++i)
        {
            byte[] cipher1 = encryptor.Transform(plain1);
            if (expectedCipher1 is null)
                expectedCipher1 = cipher1;
            else
                CollectionAssert.AreEqual(expectedCipher1, cipher1);

            byte[] actualPlain1 = decryptor.Transform(cipher1);
            CollectionAssert.AreEqual(plain1, actualPlain1);

            if (expectedCipher2 is null && (!encryptor.CanReuseTransform || !decryptor.CanReuseTransform))
                break;

            byte[] cipher2 = encryptor.Transform(plain2);
            if (expectedCipher2 is null)
                expectedCipher2 = cipher2;
            else
                CollectionAssert.AreEqual(expectedCipher2, cipher2);

            byte[] actualPlain2 = decryptor.Transform(cipher2);
            CollectionAssert.AreEqual(plain2, actualPlain2);
        }
    }

    [TestMethod]
    public void SymmetricAlgorithm_Cipher_CanEncryptMultipleBlocks()
    {
        using var algorithm = CreateSymmetricAlgorithm();
        algorithm.Mode = CipherMode.ECB;
        algorithm.Padding = PaddingMode.None;

        byte[] key = algorithm.Key;

        using var transform = algorithm.CreateEncryptor(key, null);
        if (!transform.CanTransformMultipleBlocks)
            return;

        int inputBlockSize = transform.InputBlockSize;
        int outputBlockSize = transform.OutputBlockSize;
        const int BlockCount = 3;

        byte[] plain = RandomNumberGenerator.GetBytes(inputBlockSize * BlockCount);
        byte[] actual = new byte[outputBlockSize * BlockCount];

        int actualCount = transform.TransformBlock(plain, 0, plain.Length, actual, 0);
        Assert.AreEqual(
            actual.Length,
            actualCount,
            "A transform that advertises multiple-block support should transform all complete input blocks.");

        byte[] expected = new byte[actual.Length];
        int expectedCount = 0;

        using (var referenceTransform = algorithm.CreateEncryptor(key, null))
        {
            for (int i = 0; i < plain.Length; i += inputBlockSize)
                expectedCount += referenceTransform.TransformBlock(plain, i, inputBlockSize, expected, expectedCount);
        }

        Assert.AreEqual(expectedCount, actualCount);
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void SymmetricAlgorithm_Cipher_CanDecryptMultipleBlocks()
    {
        using var algorithm = CreateSymmetricAlgorithm();
        algorithm.Mode = CipherMode.ECB;
        algorithm.Padding = PaddingMode.None;

        byte[] key = algorithm.Key;

        using var encryptor = algorithm.CreateEncryptor(key, null);
        using var transform = algorithm.CreateDecryptor(key, null);
        if (!transform.CanTransformMultipleBlocks)
            return;

        int inputBlockSize = transform.InputBlockSize;
        int outputBlockSize = transform.OutputBlockSize;
        const int BlockCount = 3;

        byte[] expected = RandomNumberGenerator.GetBytes(outputBlockSize * BlockCount);
        byte[] cipher = new byte[inputBlockSize * BlockCount];
        int cipherCount = 0;
        for (int i = 0; i < expected.Length; i += outputBlockSize)
            cipherCount += encryptor.TransformBlock(expected, i, outputBlockSize, cipher, cipherCount);
        Assert.AreEqual(cipher.Length, cipherCount);

        byte[] actual = new byte[expected.Length];
        int actualCount = transform.TransformBlock(cipher, 0, cipher.Length, actual, 0);
        Assert.AreEqual(
            actual.Length,
            actualCount,
            "A transform that advertises multiple-block support should transform all complete input blocks.");

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void SymmetricAlgorithm_Cipher_CanDecryptMultipleBlocksInPlace()
    {
        using var algorithm = CreateSymmetricAlgorithm();

        byte[] iv = algorithm.IV;
        if (iv is [])
            return;

        try
        {
            algorithm.Mode = CipherMode.CBC;
        }
        catch (CryptographicException)
        {
            return;
        }

        algorithm.Padding = PaddingMode.None;

        int blockSize = algorithm.BlockSize / 8;
        const int BlockCount = 3;

        byte[] expected = RandomNumberGenerator.GetBytes(blockSize * BlockCount);
        byte[] cipher;

        using (var encryptor = algorithm.CreateEncryptor(algorithm.Key, iv))
            cipher = encryptor.TransformFinalBlock(expected, 0, expected.Length);

        byte[] actual = new byte[cipher.Length];
        using (var decryptor = algorithm.CreateDecryptor(algorithm.Key, iv))
        {
            int actualCount = decryptor.TransformBlock(cipher, 0, cipher.Length, actual, 0);
            Assert.AreEqual(actual.Length, actualCount);
        }

        byte[] actualInPlace = (byte[])cipher.Clone();
        using (var decryptor = algorithm.CreateDecryptor(algorithm.Key, iv))
        {
            int actualCount = decryptor.TransformBlock(actualInPlace, 0, actualInPlace.Length, actualInPlace, 0);
            Assert.AreEqual(actualInPlace.Length, actualCount);
        }

        CollectionAssert.AreEqual(expected, actual);
        CollectionAssert.AreEqual(expected, actualInPlace);
    }

    [TestMethod]
    [Ignore("Algorithms provided by .NET BCL demonstrate this behavior as well.")]
    public void SymmetricAlgorithm_Cipher_CanDecryptPaddedBlocksInPlace()
    {
        using var algorithm = CreateSymmetricAlgorithm();

        byte[] iv = algorithm.IV;
        if (iv is [])
            return;

        try
        {
            algorithm.Mode = CipherMode.CBC;
        }
        catch (CryptographicException)
        {
            return;
        }

        algorithm.Padding = PaddingMode.PKCS7;

        byte[] key = algorithm.Key;
        int blockSize = algorithm.BlockSize / 8;
        byte[] expected = RandomNumberGenerator.GetBytes(blockSize * 2);
        byte[] cipher;

        using (var encryptor = algorithm.CreateEncryptor(key, iv))
            cipher = encryptor.TransformFinalBlock(expected, 0, expected.Length);

        byte[] actual = new byte[expected.Length];
        using (var decryptor = algorithm.CreateDecryptor(key, iv))
        {
            int actualCount = decryptor.TransformBlock(cipher, 0, blockSize, actual, 0);
            Assert.AreEqual(0, actualCount);

            actualCount += decryptor.TransformBlock(cipher, blockSize, cipher.Length - blockSize, actual, 0);
            byte[] finalBlock = decryptor.TransformFinalBlock([], 0, 0);
            Buffer.BlockCopy(finalBlock, 0, actual, actualCount, finalBlock.Length);
            actualCount += finalBlock.Length;

            Assert.AreEqual(expected.Length, actualCount);
        }

        byte[] actualInPlace = (byte[])cipher.Clone();
        int actualInPlaceCount;
        using (var decryptor = algorithm.CreateDecryptor(key, iv))
        {
            actualInPlaceCount = decryptor.TransformBlock(actualInPlace, 0, blockSize, actualInPlace, 0);
            Assert.AreEqual(0, actualInPlaceCount);

            actualInPlaceCount += decryptor.TransformBlock(actualInPlace, blockSize, actualInPlace.Length - blockSize, actualInPlace, blockSize);
            byte[] finalBlock = decryptor.TransformFinalBlock([], 0, 0);
            Buffer.BlockCopy(finalBlock, 0, actualInPlace, actualInPlaceCount, finalBlock.Length);
            actualInPlaceCount += finalBlock.Length;
        }

        byte[] actualInPlacePlain = new byte[actualInPlaceCount];
        Buffer.BlockCopy(actualInPlace, 0, actualInPlacePlain, 0, actualInPlacePlain.Length);

        CollectionAssert.AreEqual(expected, actual);
        CollectionAssert.AreEqual(expected, actualInPlacePlain);
    }

    [TestMethod]
    public void SymmetricAlgorithm_Cipher_CanTransformMultipleBlocksWithForwardOverlap()
    {
        using var algorithm = CreateSymmetricAlgorithm();

        byte[] key = algorithm.Key;
        byte[]? iv = null;
        byte[] algorithmIV = algorithm.IV;
        if (algorithmIV is not [])
        {
            try
            {
                algorithm.Mode = CipherMode.CBC;
            }
            catch (CryptographicException)
            {
                return;
            }

            iv = algorithmIV;
        }

        algorithm.Padding = PaddingMode.None;

        const int BlockCount = 3;

        byte[] plain;
        byte[] expectedCipher;

        using (var encryptor = algorithm.CreateEncryptor(key, iv))
        {
            if (!encryptor.CanTransformMultipleBlocks)
                return;

            plain = RandomNumberGenerator.GetBytes(encryptor.InputBlockSize * BlockCount);

            expectedCipher = TransformBlock(encryptor, plain);
        }

        byte[] actualCipher;
        using (var encryptor = algorithm.CreateEncryptor(key, iv))
            actualCipher = TransformBlockWithForwardOverlap(encryptor, plain);

        CollectionAssert.AreEqual(expectedCipher, actualCipher);

        byte[] expectedPlain;
        using (var decryptor = algorithm.CreateDecryptor(key, iv))
            expectedPlain = TransformBlock(decryptor, expectedCipher);

        byte[] actualPlain;
        using (var decryptor = algorithm.CreateDecryptor(key, iv))
            actualPlain = TransformBlockWithForwardOverlap(decryptor, expectedCipher);

        CollectionAssert.AreEqual(plain, expectedPlain);
        CollectionAssert.AreEqual(plain, actualPlain);

        static byte[] TransformBlock(ICryptoTransform transform, byte[] input)
        {
            int outputCount = input.Length / transform.InputBlockSize * transform.OutputBlockSize;
            byte[] output = new byte[outputCount];
            int actualCount = transform.TransformBlock(input, 0, input.Length, output, 0);
            Assert.AreEqual(output.Length, actualCount);
            return output;
        }
    }

    [TestMethod]
    public void SymmetricAlgorithm_Cipher_CanTransformEcbMultipleBlocksWithForwardOverlap()
    {
        using var algorithm = CreateSymmetricAlgorithm();

        try
        {
            algorithm.Mode = CipherMode.ECB;
        }
        catch (CryptographicException)
        {
            return;
        }

        algorithm.Padding = PaddingMode.None;

        byte[] key = algorithm.Key;
        using var encryptor = algorithm.CreateEncryptor(key, null);
        if (!encryptor.CanTransformMultipleBlocks)
            return;

        const int BlockCount = 3;
        byte[] plain = RandomNumberGenerator.GetBytes(encryptor.InputBlockSize * BlockCount);

        byte[] expectedCipher = encryptor.TransformFinalBlock(plain, 0, plain.Length);

        byte[] actualCipher;
        using (var transform = algorithm.CreateEncryptor(key, null))
            actualCipher = TransformBlockWithForwardOverlap(transform, plain);

        CollectionAssert.AreEqual(expectedCipher, actualCipher);
    }

    static byte[] TransformBlockWithForwardOverlap(ICryptoTransform transform, byte[] input)
    {
        int outputCount = input.Length / transform.InputBlockSize * transform.OutputBlockSize;
        int outputOffset = transform.OutputBlockSize;
        byte[] buffer = new byte[input.Length + outputOffset + outputCount];
        Buffer.BlockCopy(input, 0, buffer, 0, input.Length);

        int actualCount = transform.TransformBlock(buffer, 0, input.Length, buffer, outputOffset);
        Assert.AreEqual(outputCount, actualCount);

        byte[] output = new byte[outputCount];
        Buffer.BlockCopy(buffer, outputOffset, output, 0, output.Length);
        return output;
    }

    #endregion

    // ------------------------------------------------------------------------

    protected abstract SymmetricAlgorithm CreateSymmetricAlgorithm();

    protected virtual bool ThrowsCryptographicExceptionOnInvalidCipherArguments => false;
}
