using Gapotchenko.FX.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace Gapotchenko.FX.Data.Checksum.Tests
{
    [TestClass]
    public class Crc16Tests
    {
        static readonly byte[] CheckData = Encoding.ASCII.GetBytes("123456789");

        [TestMethod]
        public void Check(Crc16 crc, ushort check)
        {
            Assert.AreEqual(check, crc.ComputeChecksum(CheckData), "Checksum computation failed.");

            var iterator = crc.CreateIterator();
            iterator.ComputeBlock(CheckData.AsSpan(0, 5));
            iterator.ComputeBlock(CheckData.AsSpan(5, 4));
            Assert.AreEqual(check, iterator.ComputeFinal(), "Iterator checksum computation failed.");

            var ms = new MemoryStream(CheckData, false);
            Assert.AreEqual(check, crc.ComputeChecksum(ms), "Stream checksum computation failed.");

            ms.Position = 0;
            var checksum = TaskBridge.Execute(() => crc.ComputeChecksumAsync(ms));
            Assert.AreEqual(check, checksum, "Asynchronous stream checksum computation failed.");
        }

        [TestMethod]
        public void Crc16_Standard_Check() => Check(Crc16.Standard, 0xbb3d);

        [TestMethod]
        public void Crc16_Ccitt_Check() => Check(Crc16.Ccitt, 0x2189);

        [TestMethod]
        public void Crc16_IsoIec14443_3_A_Check() => Check(Crc16.IsoIec14443_3_A, 0xbf05);

        [TestMethod]
        public void Crc16_IsoIec14443_3_B_Check() => Check(Crc16.IsoIec14443_3_B, 0x906e);

        [TestMethod]
        public void Crc16_Maxim_Check() => Check(Crc16.Maxim, 0x44c2);

        [TestMethod]
        public void Crc16_Nrsc5_Check() => Check(Crc16.Nrsc5, 0xa066);

        [TestMethod]
        public void Crc16_SpiFujitsu_Check() => Check(Crc16.SpiFujitsu, 0xe5cc);
    }
}
