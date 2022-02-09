using Gapotchenko.FX.Data.Checksum.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Data.Checksum.Tests
{
    [TestClass]
    public class Crc32Tests
    {
        static void Check19(IChecksumAlgorithm<uint> algorithm, uint check) => ChecksumTestBench.Check(algorithm, ChecksumTestBench.TV19, check);

        [TestMethod]
        public void Crc32_Q_Check() => Check19(Crc32.Attested.Q, 0x3010bf7f);

        [TestMethod]
        public void Crc32_Autosar_Check() => Check19(Crc32.Attested.Autosar, 0x1697d06a);
    }
}
