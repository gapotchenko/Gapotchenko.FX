// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Security.Cryptography;

#pragma warning disable CA1850 // Prefer static 'HashData' method over 'ComputeHash'

namespace Gapotchenko.FX.Security.Cryptography.Tests;

[TestClass]
public sealed class IncrementalHashTests
{
    [TestMethod]
    public void AppendData_Span()
    {
        byte[] data = new byte[8193];
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)i;

        byte[] expected;
        using (var algorithm = SHA256.Create())
            expected = algorithm.ComputeHash(data);

        byte[] actual;
        using (var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
        {
            hash.AppendData(data.AsSpan());
            actual = hash.GetHashAndReset();
        }

        Assert.AreSequenceEqual(expected, actual);
    }
}
