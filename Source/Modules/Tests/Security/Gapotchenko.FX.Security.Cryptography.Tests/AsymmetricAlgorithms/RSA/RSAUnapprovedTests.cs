// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Tests.AsymmetricAlgorithms.RSA;

using System.Security.Cryptography;
using RSA = System.Security.Cryptography.RSA;

[TestClass]
public sealed class RSAUnapprovedTests
{
    #region Key Generation

    const int KeySize = 1024;
    const int KeyGenerationTimeout = 30 * 1000;

    [TestMethod]
    public void RSAUnapproved_DefaultKeySize()
    {
        using var exampleAlgorithm = CreateExampleAlgorithm();
        using var actualAlgorithm = CreateActualAlgorithm();

        Assert.AreEqual(exampleAlgorithm.KeySize, actualAlgorithm.KeySize);
    }

    [TestMethod]
    [Timeout(KeyGenerationTimeout)]
    public void RSAUnapproved_GenerateKey()
    {
        using var algorithm = CreateActualAlgorithm();
        algorithm.KeySize = KeySize;

        var parameters = algorithm.ExportParameters(true);

        Assert.IsNotNull(parameters.Modulus);
        Assert.HasCount(algorithm.KeySize / 8, parameters.Modulus);
        CollectionAssert.AreEqual(new byte[] { 1, 0, 1 }, parameters.Exponent);
        Assert.IsNotNull(parameters.D);
        Assert.IsNotNull(parameters.P);
        Assert.IsNotNull(parameters.Q);
        Assert.IsNotNull(parameters.DP);
        Assert.IsNotNull(parameters.DQ);
        Assert.IsNotNull(parameters.InverseQ);

        byte[] data = [1, 2, 3, 4, 5];
        byte[] encryptedData = algorithm.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        byte[] decryptedData = algorithm.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);

        CollectionAssert.AreEqual(data, decryptedData);
    }

    [TestMethod]
    [Timeout(KeyGenerationTimeout)]
    public void RSAUnapproved_ChangingKeySizeRegeneratesKey()
    {
        using var algorithm = CreateActualAlgorithm();

        // -----------------------------------------------------

        algorithm.KeySize = KeySize;
        byte[]? oldModulus = algorithm.ExportParameters(false).Modulus;

        Assert.IsNotNull(oldModulus);
        Assert.HasCount(algorithm.KeySize / 8, oldModulus);
        Assert.AreNotEqual(0, oldModulus[0]);

        // -----------------------------------------------------

        algorithm.KeySize = KeySize + 256;
        byte[]? newModulus = algorithm.ExportParameters(false).Modulus;

        Assert.IsNotNull(newModulus);
        Assert.HasCount(algorithm.KeySize / 8, newModulus);
        Assert.AreNotEqual(0, newModulus[0]);
    }

    #endregion

    #region Import/Export

    [TestMethod]
    public void RSAUnapproved_ImportExportParameters()
    {
        using var exampleAlgorithm = CreateExampleAlgorithm();
        exampleAlgorithm.KeySize = KeySize;

        using var actualAlgorithm = CreateActualAlgorithm();
        var parameters = exampleAlgorithm.ExportParameters(true);
        actualAlgorithm.ImportParameters(parameters);

        var exportedParameters = actualAlgorithm.ExportParameters(true);
        CollectionAssert.AreEqual(parameters.Modulus, exportedParameters.Modulus);
        CollectionAssert.AreEqual(parameters.Exponent, exportedParameters.Exponent);
        CollectionAssert.AreEqual(parameters.D, exportedParameters.D);
    }

    [TestMethod]
    public void RSAUnapproved_ImportParametersValidation()
    {
        using var exampleAlgorithm = CreateExampleAlgorithm();
        exampleAlgorithm.KeySize = KeySize;

        using var actualAlgorithm = CreateActualAlgorithm();
        actualAlgorithm.ImportParameters(exampleAlgorithm.ExportParameters(true));
        var expectedParameters = actualAlgorithm.ExportParameters(true);

        Action<RSAParameters>[] corruptions =
        [
            p => p.Modulus?[0] = 0,
            p => p.Exponent?[^1] &= 0xfe,
            p => p.D?[^1] ^= 1,
            p => p.P?[^1] ^= 1,
            p => p.Q?[^1] ^= 1,
            p => p.DP?[^1] ^= 1,
            p => p.DQ?[^1] ^= 1,
            p => p.InverseQ?[^1] ^= 1
        ];

        foreach (var corrupt in corruptions)
        {
            // Make invalid parameters by corrupting the current valid parameters.
            var invalidParameters = exampleAlgorithm.ExportParameters(true);
            corrupt(invalidParameters);

            // Try import invalid parameters.
            Assert.ThrowsExactly<CryptographicException>(() => actualAlgorithm.ImportParameters(invalidParameters));

            // Ensure that actual parameters stayed intact.
            var actualParameters = actualAlgorithm.ExportParameters(true);
            CollectionAssert.AreEqual(expectedParameters.Modulus, actualParameters.Modulus);
            CollectionAssert.AreEqual(expectedParameters.Exponent, actualParameters.Exponent);
            CollectionAssert.AreEqual(expectedParameters.D, actualParameters.D);
            CollectionAssert.AreEqual(expectedParameters.P, actualParameters.P);
            CollectionAssert.AreEqual(expectedParameters.Q, actualParameters.Q);
            CollectionAssert.AreEqual(expectedParameters.DP, actualParameters.DP);
            CollectionAssert.AreEqual(expectedParameters.DQ, actualParameters.DQ);
            CollectionAssert.AreEqual(expectedParameters.InverseQ, actualParameters.InverseQ);
        }
    }

    #endregion

    #region Encyryption/Decryption

    [TestMethod]
    public void RSAUnapproved_EncryptDecrypt_Pkcs1()
    {
        RSAUnapproved_EncryptDecrypt(RSAEncryptionPadding.Pkcs1);
    }

    [TestMethod]
    public void RSAUnapproved_EncryptDecrypt_OaepSha1()
    {
        RSAUnapproved_EncryptDecrypt(RSAEncryptionPadding.OaepSHA1);
    }

    static void RSAUnapproved_EncryptDecrypt(RSAEncryptionPadding padding)
    {
        using var exampleAlgorithm = CreateExampleAlgorithm();
        exampleAlgorithm.KeySize = KeySize;

        using var actualAlgorithm = CreateActualAlgorithm();
        actualAlgorithm.ImportParameters(exampleAlgorithm.ExportParameters(true));

        byte[] data = [1, 2, 3, 4, 5];

        // Actual -> actual
        byte[] encryptedData = actualAlgorithm.Encrypt(data, padding);
        byte[] decryptedData = actualAlgorithm.Decrypt(encryptedData, padding);
        CollectionAssert.AreEqual(data, decryptedData);

        // Actual -> example
        decryptedData = exampleAlgorithm.Decrypt(encryptedData, padding);
        CollectionAssert.AreEqual(data, decryptedData);

        // Example -> actual
        encryptedData = exampleAlgorithm.Encrypt(data, padding);
        decryptedData = actualAlgorithm.Decrypt(encryptedData, padding);
        CollectionAssert.AreEqual(data, decryptedData);
    }

    #endregion

    #region Signature

    [TestMethod]
    public void RSAUnapproved_SignVerifyHash_Pkcs1Sha256()
    {
        RSAUnapproved_SignVerifyHash(HashAlgorithmName.SHA256, SHA256.Create, RSASignaturePadding.Pkcs1);
    }

    [TestMethod]
    public void RSAUnapproved_SignVerifyHash_PssSha256()
    {
        RSAUnapproved_SignVerifyHash(HashAlgorithmName.SHA256, SHA256.Create, RSASignaturePadding.Pss);
    }

    static void RSAUnapproved_SignVerifyHash(
        HashAlgorithmName hashAlgorithmName,
        Func<HashAlgorithm> hashAlgorithmFactory,
        RSASignaturePadding signaturePadding)
    {
        using var exampleAlgorithm = CreateExampleAlgorithm();
        exampleAlgorithm.KeySize = KeySize;

        using var actualAlgorithm = CreateActualAlgorithm();
        actualAlgorithm.ImportParameters(exampleAlgorithm.ExportParameters(true));

        using var hashAlgorithm = hashAlgorithmFactory();
        byte[] hash = hashAlgorithm.ComputeHash([1, 2, 3, 4, 5]);

        // Actual -> actual
        byte[] signature = actualAlgorithm.SignHash(hash, hashAlgorithmName, signaturePadding);
        Assert.IsTrue(actualAlgorithm.VerifyHash(hash, signature, hashAlgorithmName, signaturePadding));

        // Actual -> example
        signature = actualAlgorithm.SignHash(hash, hashAlgorithmName, signaturePadding);
        try
        {
            Assert.IsTrue(exampleAlgorithm.VerifyHash(hash, signature, hashAlgorithmName, signaturePadding));
        }
        catch (CryptographicException)
        {
            // Example algorithm lacks the support of the specified padding mode.
            return;
        }

        // Example -> actual
        signature = exampleAlgorithm.SignHash(hash, hashAlgorithmName, signaturePadding);
        Assert.IsTrue(actualAlgorithm.VerifyHash(hash, signature, hashAlgorithmName, signaturePadding));
    }

    #endregion

    static RSA CreateExampleAlgorithm()
    {
        return RSA.Create();
    }

    static RSA CreateActualAlgorithm()
    {
        return RSAUnapproved.Create();
    }
}
