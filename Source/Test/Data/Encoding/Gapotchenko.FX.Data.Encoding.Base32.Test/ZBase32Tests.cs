using Gapotchenko.FX.Data.Encoding.Test.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class ZBase32Tests
    {
        static void TestVector(byte[] raw, string encoded)
        {
            string actualEncoded = ZBase32.GetString(raw);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = ZBase32.GetBytes(actualEncoded);
            Assert.IsTrue(raw.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = ZBase32.Instance;

            Assert.AreEqual(ZBase32.Efficiency, instance.Efficiency);

            // -----------------------------------------------------------------

            TextDataEncodingTestServices.TestVector(instance, raw, encoded, padded: false);
        }

        [TestMethod]
        public void ZBase32_Empty() => TestVector(new byte[0], "");

        [TestMethod]
        public void ZBase32_Bytes_TV1() => TestVector(new byte[] { 240, 191, 199 }, "6n9hq");

        [TestMethod]
        public void ZBase32_Bytes_TV2() => TestVector(new byte[] { 212, 122, 4 }, "4t7ye");

        [TestMethod]
        public void ZBase32_Bytes_TV3() => TestVector(new byte[] { 0xff }, "9h");

        [TestMethod]
        public void ZBase32_Bytes_TV4() => TestVector(new byte[] { 0xb5 }, "sw");

        [TestMethod]
        public void ZBase32_Bytes_TV5() => TestVector(new byte[] { 0x34, 0x5a }, "gtpy");

        [TestMethod]
        public void ZBase32_Bytes_TV6() => TestVector(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff }, "99999999");

        [TestMethod]
        public void ZBase32_Bytes_TV7() => TestVector(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }, "999999999h");

        [TestMethod]
        public void ZBase32_Bytes_TV8() => TestVector(
            new byte[]
            {
                0xc0, 0x73, 0x62, 0x4a, 0xaf, 0x39, 0x78, 0x51,
                0x4e, 0xf8, 0x44, 0x3b, 0xb2, 0xa8, 0x59, 0xc7,
                0x5f, 0xc3, 0xcc, 0x6a, 0xf2, 0x6d, 0x5a, 0xaa
            },
            "ab3sr1ix8fhfnuzaeo75fkn3a7xh8udk6jsiiko");
    }
}
