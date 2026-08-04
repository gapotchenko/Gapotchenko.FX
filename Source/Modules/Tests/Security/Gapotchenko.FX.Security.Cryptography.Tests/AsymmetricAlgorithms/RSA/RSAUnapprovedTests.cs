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
    const int KeySize = 1024;

    [TestMethod]
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
    public void RSAUnapproved_EncryptDecrypt_Pkcs1()
    {
        using var exampleAlgorithm = CreateExampleAlgorithm();
        exampleAlgorithm.KeySize = KeySize;

        using var actualAlgorithm = CreateActualAlgorithm();
        actualAlgorithm.ImportParameters(exampleAlgorithm.ExportParameters(true));

        byte[] data = [1, 2, 3, 4, 5];
        byte[] encryptedData = actualAlgorithm.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        byte[] decryptedData = actualAlgorithm.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);

        CollectionAssert.AreEqual(data, decryptedData);
    }

    [TestMethod]
    public void RSAUnapproved_EncryptDecrypt_OaepSha1()
    {
        using var exampleAlgorithm = CreateExampleAlgorithm();
        exampleAlgorithm.KeySize = KeySize;

        using var actualAlgorithm = CreateActualAlgorithm();
        actualAlgorithm.ImportParameters(exampleAlgorithm.ExportParameters(true));

        byte[] data = [1, 2, 3, 4, 5];
        byte[] encryptedData = actualAlgorithm.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
        byte[] decryptedData = actualAlgorithm.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA1);

        CollectionAssert.AreEqual(data, decryptedData);
    }

    [TestMethod]
    public void RSAUnapproved_SignVerifyHash_Pkcs1Sha256()
    {
        using var exampleAlgorithm = CreateExampleAlgorithm();
        exampleAlgorithm.KeySize = KeySize;

        using var actualAlgorithm = CreateActualAlgorithm();
        actualAlgorithm.ImportParameters(exampleAlgorithm.ExportParameters(true));

        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash([1, 2, 3, 4, 5]);
        byte[] signature = actualAlgorithm.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        Assert.IsTrue(actualAlgorithm.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
    }

    [TestMethod]
    public void RSAUnapproved_SignVerifyHash_PssSha256()
    {
        using var exampleAlgorithm = CreateExampleAlgorithm();
        exampleAlgorithm.KeySize = KeySize;

        using var actualAlgorithm = CreateActualAlgorithm();
        actualAlgorithm.ImportParameters(exampleAlgorithm.ExportParameters(true));

        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash([1, 2, 3, 4, 5]);
        byte[] signature = actualAlgorithm.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

        Assert.IsTrue(actualAlgorithm.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
    }

    static RSA CreateExampleAlgorithm()
    {
        return RSA.Create();
    }

    static RSA CreateActualAlgorithm()
    {
        return RSAUnapproved.Create();
    }
}
