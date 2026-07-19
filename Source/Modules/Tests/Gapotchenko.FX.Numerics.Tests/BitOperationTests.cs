using System.Numerics;

namespace Gapotchenko.FX.Numerics.Tests;

[TestClass]
public class BitOperationTests
{
    [TestMethod]
    // Convention.
    [DataRow(0U, 0)]
    // Sanity checks.
    [DataRow(1U, 0)]
    [DataRow(2U, 1)]
    [DataRow(uint.MaxValue, 31)]
    // Verification.
    [DataRow(32U, 5)]
    [DataRow(127U, 6)]
    [DataRow(128U, 7)]
    [DataRow(255U, 7)]
    [DataRow(256U, 8)]
    public void BitOperations_Log2_UInt32(uint value, int log)
    {
        Assert.AreEqual(log, BitOperations.Log2(value));
    }

    [TestMethod]
    public void BitOperations_PopCount()
    {
        // Sanity checks.
        Assert.AreEqual(0, BitOperations.PopCount(0));
        Assert.AreEqual(1, BitOperations.PopCount(1));
        Assert.AreEqual(32, BitOperations.PopCount(~0U));

        // Algorithmic verification for linear bit masks.
        for (int j = 1; j < 32; ++j)
        {
            // Get a mask for the j-th bit.
            uint mask = 1U << j;

            // The j-th bit is single.
            Assert.AreEqual(1, BitOperations.PopCount(mask));

            // A mask - 1 signifies all the bits prior to j-th: 10000 - 1 = 01111
            Assert.AreEqual(j, BitOperations.PopCount(mask - 1));
        }
    }
}
