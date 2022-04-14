using Gapotchenko.FX.Data.Encoding.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gapotchenko.FX.Data.Encoding.Tests
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class ZBase32Tests
    {
        static void TestVector(byte[] raw, string encoded, int bitCount = 0)
        {
            if (bitCount == 0)
                bitCount = raw.Length * 8;
            else if (bitCount > raw.Length * 8)
                throw new ArgumentException("Bit count is greater than data size.", nameof(bitCount));

            bool byteBounds = bitCount % 8 == 0;

            var options = DataEncodingOptions.None;
            if (!byteBounds)
                options |= DataEncodingOptions.Compress;

            string actualEncoded = ZBase32.GetString(raw, options);
            Assert.AreEqual(encoded, actualEncoded, "Encoding error.");

            var actualDecoded = ZBase32.GetBytes(encoded, options);
            if (!raw.SequenceEqual(actualDecoded))
            {
                Assert.AreEqual(
                    Base16.GetString(raw, DataEncodingOptions.Indent),
                    Base16.GetString(actualDecoded, DataEncodingOptions.Indent),
                    "Decoding error.");
            }

            // -----------------------------------------------------------------

            var instance = ZBase32.Instance;

            Assert.AreEqual(ZBase32.Efficiency, instance.Efficiency);

            // -----------------------------------------------------------------

            TextDataEncodingTestBench.TestVector(instance, raw, encoded, options);
        }

        static void TestVector(string raw, string encoded) => TestVector(Encoding.UTF8.GetBytes(raw), encoded);

        [TestMethod]
        public void ZBase32_Empty() => TestVector(Array.Empty<byte>(), "");

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

        [TestMethod]
        public void ZBase32_Bytes_TV9() => TestVector(new byte[] { 0x10, 0x11, 0x10 }, "nyety");

        [TestMethod]
        public void ZBase32_Bits_TV1() => TestVector(new byte[] { 0 }, "y", 1);

        [TestMethod]
        public void ZBase32_Bits_TV2() => TestVector(new byte[] { 128 }, "o", 1);

        [TestMethod]
        public void ZBase32_Bits_TV3() => TestVector(new byte[] { 64 }, "e", 2);

        [TestMethod]
        public void ZBase32_Bits_TV4() => TestVector(new byte[] { 192 }, "a", 2);

        [TestMethod]
        public void ZBase32_Bits_TV5() => TestVector(new byte[] { 0, 0 }, "yy", 10);

        [TestMethod]
        public void ZBase32_Bits_TV6() => TestVector(new byte[] { 128, 128 }, "on", 10);

        [TestMethod]
        public void ZBase32_Bits_TV7() => TestVector(new byte[] { 139, 136, 128 }, "tqre", 20);

        [TestMethod]
        public void ZBase32_Bits_TV8() => TestVector(new byte[] { 245, 87, 189, 12 }, "6im54d", 30);

        [TestMethod]
        public void ZBase32_Bits_TV9() => TestVector(new byte[] { 0xff, 0xe0 }, "99o", 11);

        [TestMethod]
        public void ZBase32_Bits_TV10() => TestVector(new byte[] { 0x10, 0x11, 0x10 }, "nyet", 20);

        [TestMethod]
        public void ZBase32_Bits_TV11() => TestVector(new byte[] { 0, 0, 0 }, "yyyy", 18);

        [TestMethod]
        public void ZBase32_Bits_TV12() => TestVector(new byte[] { 0, 0, 0, 0 }, "yyyyy", 25);

        [TestMethod]
        public void ZBase32_Strings_TV1() => TestVector("hello, world\n", "pb1sa5dxfoo8q551pt1yw");

        [TestMethod]
        public void ZBase32_Strings_TV2() => TestVector("\x0001binary!!!1\x0000", "yftg15ubqjh1nejbgryy");

        [TestMethod]
        public void ZBase32_RT1() => TextDataEncodingTestBench.RoundTrip(ZBase32.Instance, Base16.GetBytes("90 60 00"), DataEncodingOptions.Compress);

        [TestMethod]
        public void ZBase32_RT2() => TextDataEncodingTestBench.RoundTrip(ZBase32.Instance, Base16.GetBytes("64 BF"), DataEncodingOptions.Compress);

        [TestMethod]
        public void ZBase32_RT3() => TextDataEncodingTestBench.RoundTrip(ZBase32.Instance, Base16.GetBytes("EB 00"), DataEncodingOptions.Compress);

        [TestMethod]
        public void ZBase32_RT4() => TextDataEncodingTestBench.RoundTrip(ZBase32.Instance, Base16.GetBytes("01 67 00"), DataEncodingOptions.Compress);

        [DataTestMethod]
        [DataRow(DataEncodingOptions.None)]
        [DataRow(DataEncodingOptions.Compress)]
        [DataRow(DataEncodingOptions.Padding)]
        [DataRow(DataEncodingOptions.Padding | DataEncodingOptions.Compress)]
        public void ZBase32_RT_Random(DataEncodingOptions options) => TextDataEncodingTestBench.RandomRoundTrip(ZBase32.Instance, 16, 100000, options);

        [DataTestMethod]
        // S1
        [DataRow(TextDataEncodingTemplates.S1, DataEncodingOptions.None)]
        [DataRow(TextDataEncodingTemplates.S1, DataEncodingOptions.Compress)]
        [DataRow(TextDataEncodingTemplates.S1, DataEncodingOptions.Padding)]
        [DataRow(TextDataEncodingTemplates.S1, DataEncodingOptions.Padding | DataEncodingOptions.Compress)]
        // S2
        [DataRow(TextDataEncodingTemplates.S2, DataEncodingOptions.None)]
        [DataRow(TextDataEncodingTemplates.S2, DataEncodingOptions.Compress)]
        [DataRow(TextDataEncodingTemplates.S2, DataEncodingOptions.Padding)]
        [DataRow(TextDataEncodingTemplates.S2, DataEncodingOptions.Padding | DataEncodingOptions.Compress)]
        // S3
        [DataRow(TextDataEncodingTemplates.S3, DataEncodingOptions.None)]
        [DataRow(TextDataEncodingTemplates.S3, DataEncodingOptions.Compress)]
        [DataRow(TextDataEncodingTemplates.S3, DataEncodingOptions.Padding)]
        [DataRow(TextDataEncodingTemplates.S3, DataEncodingOptions.Padding | DataEncodingOptions.Compress)]
        public void ZBase32_RT_S(string s, DataEncodingOptions options) => TextDataEncodingTestBench.RoundTrip(ZBase32.Instance, s, options);
    }
}
