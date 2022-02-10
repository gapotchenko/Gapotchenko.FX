using Gapotchenko.FX.Data.Checksum.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Gapotchenko.FX.Data.Checksum.Tests
{
    [TestClass]
    public class Crc8Tests
    {
        static void Check19(IChecksumAlgorithm<byte> algorithm, byte check)
        {
            Assert.AreEqual(8, algorithm.ChecksumSize);
            ChecksumTestBench.Check(algorithm, ChecksumTestBench.TV19, check);
        }

        [TestMethod]
        public void Crc8_Standard_Check() => Check19(Crc8.Standard, 0xf4);
    }
}
