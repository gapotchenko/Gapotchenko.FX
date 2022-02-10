using Gapotchenko.FX.Data.Checksum.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [TestMethod]
        public void Crc8_Standard_Hash()
        {
            var algorithm = Crc8.Standard;
            var ha = algorithm.CreateHashAlgorithm();
            var hash = ha.ComputeHash(ChecksumTestBench.TV19);
            Assert.AreEqual(1, hash.Length);
            Assert.AreEqual(0xf4, hash[0]);
        }
    }
}
