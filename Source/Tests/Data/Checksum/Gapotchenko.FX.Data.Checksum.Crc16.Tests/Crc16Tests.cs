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
        /// <summary>
        /// The test vector that corresponds to the bytes of ASCII string "123456789".
        /// </summary>
        static readonly byte[] TV19 = Encoding.ASCII.GetBytes("123456789");

        static void Check<T>(IChecksumAlgorithm<T> algorithm, byte[] data, ushort check)
            where T : struct
        {
            Assert.AreEqual(check, algorithm.ComputeChecksum(data), "Checksum computation failed.");

            var iterator = algorithm.CreateIterator();
            iterator.ComputeBlock(data.AsSpan(0, 5));
            iterator.ComputeBlock(data.AsSpan(5, 4));
            Assert.AreEqual(check, iterator.ComputeFinal(), "Iterator checksum computation failed.");

            iterator.ComputeBlock(data.AsSpan(0, 2));
            iterator.ComputeBlock(data.AsSpan(2, 7));
            Assert.AreEqual(check, iterator.ComputeFinal(), "Reused iterator checksum computation failed.");

            iterator.ComputeBlock(data.AsSpan(0, 5));
            iterator.Reset();
            iterator.ComputeBlock(data.AsSpan(0, 3));
            iterator.ComputeBlock(data.AsSpan(3, 6));
            Assert.AreEqual(check, iterator.ComputeFinal(), "Reseted iterator checksum computation failed.");

            var ms = new MemoryStream(data, false);
            Assert.AreEqual(check, algorithm.ComputeChecksum(ms), "Stream checksum computation failed.");

            ms.Position = 0;
            var checksum = TaskBridge.Execute(() => algorithm.ComputeChecksumAsync(ms));
            Assert.AreEqual(check, checksum, "Asynchronous stream checksum computation failed.");
        }

        /// <summary>
        /// Checks a specified checksum algorithm for correctness working with "123456789" ASCII test vector.
        /// </summary>
        /// <typeparam name="T">The type of the checksum value.</typeparam>
        /// <param name="algorithm">The checksum algorithm to check.</param>
        /// <param name="check">The expected checksum.</param>
        static void Check19<T>(IChecksumAlgorithm<T> algorithm, ushort check)
            where T : struct =>
            Check(algorithm, TV19, check);

        [TestMethod]
        public void Crc16_Standard_Check() => Check19(Crc16.Standard, 0xbb3d);

        [TestMethod]
        public void Crc16_Ccitt_Check() => Check19(Crc16.Ccitt, 0x2189);

        [TestMethod]
        public void Crc16_IsoIec14443_3_A_Check() => Check19(Crc16.IsoIec14443_3_A, 0xbf05);

        [TestMethod]
        public void Crc16_IsoIec14443_3_B_Check() => Check19(Crc16.IsoIec14443_3_B, 0x906e);

        [TestMethod]
        public void Crc16_Maxim_Check() => Check19(Crc16.Maxim, 0x44c2);

        [TestMethod]
        public void Crc16_Nrsc5_Check() => Check19(Crc16.Nrsc5, 0xa066);

        [TestMethod]
        public void Crc16_SpiFujitsu_Check() => Check19(Crc16.SpiFujitsu, 0xe5cc);
    }
}
