// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography.Tests.SymmetricAlgorithms;

public abstract class SymmetricAlgorithmTestsBase
{
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

            algorithm.IV = RandomNumberGenerator.GetBytes(ivRank);
        }

        Assert.ThrowsExactly<ArgumentNullException>(() => algorithm.IV = null!);
    }

    protected abstract SymmetricAlgorithm CreateSymmetricAlgorithm();
}
