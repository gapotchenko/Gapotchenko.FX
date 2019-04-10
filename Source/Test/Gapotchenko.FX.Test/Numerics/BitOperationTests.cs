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

            for (int i = 1; i < 32; ++i)
                Assert.AreEqual(i, BitOperations.PopCount((1U << i) - 1));
        }
    }
}
