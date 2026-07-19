using System.Numerics;

namespace Gapotchenko.FX.Numerics.Tests;

[TestClass]
public class BitOperationTests
{
    [TestMethod]
    // Contract
    [DataRow(0x0U, 0)]
    // Sanity checks
    [DataRow(0x1U, 0)]
    [DataRow(0x2U, 1)]
    [DataRow(uint.MaxValue, 31)]
    // Verification
    [DataRow(0x20U, 5)]
    [DataRow(0x7fU, 6)]
    [DataRow(0x80U, 7)]
    [DataRow(0xffU, 7)]
    [DataRow(0x100U, 8)]
    public void BitOperations_Log2_UInt32(uint value, int log)
    {
        Assert.AreEqual(log, BitOperations.Log2(value));
    }

    [TestMethod]
    // Contract
    [DataRow(0x0UL, 0)]
    // Sanity checks
    [DataRow(0x1UL, 0)]
    [DataRow(0x2UL, 1)]
    [DataRow(ulong.MaxValue, 63)]
    // Verification
    [DataRow(0x20UL, 5)]
    [DataRow(0x7fUL, 6)]
    [DataRow(0x80UL, 7)]
    [DataRow(0xffUL, 7)]
    [DataRow(0x100UL, 8)]
    [DataRow(0xffffffffUL, 31)]
    [DataRow(0x100000000UL, 32)]
    [DataRow(0x1ffffffffUL, 32)]
    [DataRow(0x200000000UL, 33)]
    public void BitOperations_Log2_UInt64(ulong value, int log)
    {
        Assert.AreEqual(log, BitOperations.Log2(value));
    }

    [TestMethod]
    // Sanity checks
    [DataRow(0x0U, 32)]
    [DataRow(0x1U, 31)]
    [DataRow(0x2U, 30)]
    [DataRow(uint.MaxValue, 0)]
    // Verification
    [DataRow(0x20U, 26)]
    [DataRow(0x7fU, 25)]
    [DataRow(0x80U, 24)]
    [DataRow(0xffU, 24)]
    [DataRow(0x100U, 23)]
    [DataRow(0x7fffffffU, 1)]
    [DataRow(0x80000000U, 0)]
    public void BitOperations_LeadingZeroCount_UInt32(uint value, int count)
    {
        Assert.AreEqual(count, BitOperations.LeadingZeroCount(value));
    }

    [TestMethod]
    // Sanity checks
    [DataRow(0x0UL, 64)]
    [DataRow(0x1UL, 63)]
    [DataRow(0x2UL, 62)]
    [DataRow(ulong.MaxValue, 0)]
    // Verification
    [DataRow(0x20UL, 58)]
    [DataRow(0x7fUL, 57)]
    [DataRow(0x80UL, 56)]
    [DataRow(0xffUL, 56)]
    [DataRow(0x100UL, 55)]
    [DataRow(0xffffffffUL, 32)]
    [DataRow(0x100000000UL, 31)]
    [DataRow(0x7fffffffffffffffUL, 1)]
    [DataRow(0x8000000000000000UL, 0)]
    public void BitOperations_LeadingZeroCount_UInt64(ulong value, int count)
    {
        Assert.AreEqual(count, BitOperations.LeadingZeroCount(value));
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
