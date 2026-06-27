// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Data.Encoding;
using System.Security.Cryptography;
using System.Text;

namespace Gapotchenko.FX.Security.Cryptography.Tests.HashAlgorithms;

public abstract class HashAlgorithmTest
{
    [TestMethod]
    public void HashAlgorithm_ReuseTransform()
    {
        using var algorithm = CreateHashAlgorithm();
        if (!algorithm.CanReuseTransform)
            return;

        byte[] data = RandomNumberGenerator.GetBytes(32);

        byte[] hash1 = algorithm.ComputeHash(data);
        byte[] hash2 = algorithm.ComputeHash(data);

        CollectionAssert.AreEqual(hash1, hash2);
    }

    // ------------------------------------------------------------------------

    protected void Verify(string inputText, string expectedHex)
    {
        byte[] input = Encoding.UTF8.GetBytes(inputText);
        byte[] expected = Base16.GetBytes(expectedHex);

        Verify(input, expected);
    }

    protected void Verify(byte[] input, byte[] expected)
    {
        Verify_Array(input, expected);

#if NETCOREAPP
        Verify_Span(input, expected);
#endif

        var inputStream = new MemoryStream(input, false);
        Verify_ComputeHashStream(inputStream, expected);

        inputStream.Position = 0;
        Verify_ICryptoTransformStream(inputStream, expected);
    }

    void Verify_Array(byte[] input, byte[] expected)
    {
        byte[] actual;

        using var hash = CreateHashAlgorithm();
        Assert.IsGreaterThan(0, hash.HashSize);
        actual = hash.ComputeHash(input, 0, input.Length);

        CollectionAssert.AreEqual(expected, actual);

        CollectionAssert.AreEqual(expected, hash.Hash);
    }

#if NETCOREAPP

    void Verify_Span(byte[] input, byte[] expected)
    {
        using var hash = CreateHashAlgorithm();

        // Too small
        byte[] actual = new byte[expected.Length - 1];
        Assert.IsFalse(hash.TryComputeHash(input, actual, out int bytesWritten));
        Assert.AreEqual(0, bytesWritten);

        // Just right
        actual = new byte[expected.Length];
        Assert.IsTrue(hash.TryComputeHash(input, actual, out bytesWritten));
        Assert.AreEqual(expected.Length, bytesWritten);
        CollectionAssert.AreEqual(expected, actual);

        // Bigger than needed
        actual = new byte[expected.Length + 1];
        actual[^1] = 42;
        Assert.IsTrue(hash.TryComputeHash(input, actual, out bytesWritten));
        Assert.AreEqual(expected.Length, bytesWritten);
        CollectionAssert.AreEqual(expected, actual.AsSpan(0, expected.Length).ToArray());
        Assert.AreEqual(42, actual[^1]);
    }

#endif

    void Verify_ComputeHashStream(Stream input, byte[] expected)
    {
        using var hash = CreateHashAlgorithm();
        Assert.IsGreaterThan(0, hash.HashSize);
        byte[] actual = hash.ComputeHash(input);
        CollectionAssert.AreEqual(expected, actual);
    }

    void Verify_ICryptoTransformStream(Stream input, byte[] expected)
    {
        using var hash = CreateHashAlgorithm();
        using var cryptoStream = new CryptoStream(input, hash, CryptoStreamMode.Read);

        byte[] buffer = new byte[357]; // a different buffer size than HashAlgorithm which uses 4096
        while (cryptoStream.Read(buffer, 0, buffer.Length) > 0)
        {
            // CryptoStream will build up the hash.
        }

        CollectionAssert.AreEqual(expected, hash.Hash, "Crypto transform operations should produce a valid hash.");
    }

    // ------------------------------------------------------------------------

    protected abstract HashAlgorithm CreateHashAlgorithm();
}
