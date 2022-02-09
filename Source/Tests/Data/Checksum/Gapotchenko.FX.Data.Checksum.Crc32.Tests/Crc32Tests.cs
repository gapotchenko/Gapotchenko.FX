using Gapotchenko.FX.Data.Checksum.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Data.Checksum.Tests
{
    [TestClass]
    public class Crc32Tests
    {
        static void Check19(IChecksumAlgorithm<uint> algorithm, uint check) => ChecksumTestBench.Check(algorithm, ChecksumTestBench.TV19, check);

        [TestMethod]
        public void Crc32_Standard_Check() => Check19(Crc32.Standard, 0xcbf43926);

        [TestMethod]
        public void Crc32_Standard_Hash()
        {
            var algorithm = Crc32.Standard;
            var ha = algorithm.CreateHashAlgorithm();
            var hash = ha.ComputeHash(ChecksumTestBench.TV19);
            Assert.AreEqual(4, hash.Length);
            Assert.AreEqual(0xcbf43926, LittleEndianBitConverter.ToUInt32(hash));
        }

        void Crc32_Standard_Hash(IBitConverter bitConverter)
        {
            var algorithm = Crc32.Standard;
            var ha = algorithm.CreateHashAlgorithm(bitConverter);
            var hash = ha.ComputeHash(ChecksumTestBench.TV19);
            Assert.AreEqual(4, hash.Length);
            Assert.AreEqual(0xcbf43926, bitConverter.ToUInt32(hash));
        }

        [TestMethod]
        public void Crc32_Standard_Hash_LE() => Crc32_Standard_Hash(LittleEndianBitConverter.Instance);

        [TestMethod]
        public void Crc32_Standard_Hash_BE() => Crc32_Standard_Hash(BigEndianBitConverter.Instance);

        [TestMethod]
        public void Crc32_C_Check() => Check19(Crc32.Attested.C, 0xe3069283);

        [TestMethod]
        public void Crc32_D_Check() => Check19(Crc32.Attested.D, 0x87315576);

        [TestMethod]
        public void Crc32_Autosar_Check() => Check19(Crc32.Attested.Autosar, 0x1697d06a);

        [TestMethod]
        public void Crc32_Posix_Check() => Check19(Crc32.Attested.Posix, 0x765e7680);

        [TestMethod]
        public void Crc32_DectB_Check() => Check19(Crc32.Attested.DectB, 0xfc891918);
    }
}
