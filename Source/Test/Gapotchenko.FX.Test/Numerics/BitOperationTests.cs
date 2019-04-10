using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Gapotchenko.FX.Test.Numerics
{
    [TestClass]
    public class BitOperationTests
    {
        [TestMethod]
        public void BitOps_Log2()
        {
            Assert.AreEqual(0, BitOperations.Log2(0));
            Assert.AreEqual(0, BitOperations.Log2(1));
            Assert.AreEqual(1, BitOperations.Log2(2));
            Assert.AreEqual(5, BitOperations.Log2(32));
            Assert.AreEqual(6, BitOperations.Log2(127));
            Assert.AreEqual(7, BitOperations.Log2(128));
            Assert.AreEqual(7, BitOperations.Log2(255));
            Assert.AreEqual(8, BitOperations.Log2(256));
            Assert.AreEqual(31, BitOperations.Log2(uint.MaxValue));
        }

        [TestMethod]
        public void BitOps_PopCount()
        {
            Assert.AreEqual(0, BitOperations.PopCount(0));
            Assert.AreEqual(1, BitOperations.PopCount(1));
            Assert.AreEqual(32, BitOperations.PopCount(~0U));

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
}
