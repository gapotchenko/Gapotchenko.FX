﻿using Gapotchenko.FX.Threading.Tasks;
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
        public void Crc16_Standard_Hash()
        {
            var algorithm = Crc16.Standard;
            var ha = algorithm.CreateHashAlgorithm();
            var hash = ha.ComputeHash(TV19);
            Assert.AreEqual(2, hash.Length);
            Assert.AreEqual(0xbb3d, LittleEndianBitConverter.ToUInt16(hash));
        }

        void Crc16_Standard_Hash(IBitConverter bitConverter)
        {
            var algorithm = Crc16.Standard;
            var ha = algorithm.CreateHashAlgorithm(bitConverter);
            var hash = ha.ComputeHash(TV19);
            Assert.AreEqual(2, hash.Length);
            Assert.AreEqual(0xbb3d, bitConverter.ToUInt16(hash));
        }

        [TestMethod]
        public void Crc16_Standard_Hash_LE() => Crc16_Standard_Hash(LittleEndianBitConverter.Instance);

        [TestMethod]
        public void Crc16_Standard_Hash_BE() => Crc16_Standard_Hash(BigEndianBitConverter.Instance);

        [TestMethod]
        public void Crc16_Ccitt_Check() => Check19(Crc16.Attested.Ccitt, 0x2189);

        [TestMethod]
        public void Crc16_IsoIec14443_3_A_Check() => Check19(Crc16.Attested.IsoIec14443_3_A, 0xbf05);

        [TestMethod]
        public void Crc16_IsoIec14443_3_B_Check() => Check19(Crc16.Attested.IsoIec14443_3_B, 0x906e);

        [TestMethod]
        public void Crc16_Maxim_Check() => Check19(Crc16.Attested.Maxim, 0x44c2);

        [TestMethod]
        public void Crc16_Nrsc5_Check() => Check19(Crc16.Attested.Nrsc5, 0xa066);

        [TestMethod]
        public void Crc16_SpiFujitsu_Check() => Check19(Crc16.Attested.SpiFujitsu, 0xe5cc);

        [TestMethod]
        public void Crc16_Umts_Check() => Check19(Crc16.Attested.Umts, 0xfee8);

        [TestMethod]
        public void Crc16_Usb_Check() => Check19(Crc16.Attested.Usb, 0xb4c8);

        [TestMethod]
        public void Crc16_XModem_Check() => Check19(Crc16.Attested.XModem, 0x31c3);

        [TestMethod]
        public void Crc16_Profibus_Check() => Check19(Crc16.Attested.Profibus, 0xa819);

        [TestMethod]
        public void Crc16_Modbus_Check() => Check19(Crc16.Attested.Modbus, 0x4b37);

        [TestMethod]
        public void Crc16_OpenSafetyA_Check() => Check19(Crc16.Attested.OpenSafetyA, 0x5d38);

        [TestMethod]
        public void Crc16_OpenSafetyB_Check() => Check19(Crc16.Attested.OpenSafetyB, 0x20fe);

        [TestMethod]
        public void Crc16_TMS37157_Check() => Check19(Crc16.Attested.TMS37157, 0x26b1);

        [TestMethod]
        public void Crc16_MCRF4XX_Check() => Check19(Crc16.Attested.MCRF4XX, 0x6f91);
    }
}
