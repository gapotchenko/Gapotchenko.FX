using Gapotchenko.FX.Data.Encoding.Test.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class CrockfordBase32Tests
    {
        static void TestVector(byte[] raw, string encoded, DataEncodingOptions options = default)
        {
            string actualEncoded = CrockfordBase32.GetString(raw, options);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = CrockfordBase32.GetBytes(encoded, options);
            Assert.IsTrue(raw.SequenceEqual(actualDecoded));

            var instance = CrockfordBase32.Instance;
            Assert.AreEqual(CrockfordBase32.Efficiency, instance.Efficiency);

            TextDataEncodingTestBench.TestVector(instance, raw, encoded, options: options);
        }

        static void TestVector(string raw, string encoded) => TestVector(Encoding.UTF8.GetBytes(raw), encoded);

        static void TestVector(int raw, string encoded, DataEncodingOptions options = default)
        {
            var actualDecoded = CrockfordBase32.GetInt32(encoded, options);
            Assert.AreEqual(raw, actualDecoded);

            string actualEncoded = CrockfordBase32.GetString(raw, options);
            Assert.AreEqual(encoded, actualEncoded);
        }

        static void TestVector(long raw, string encoded, DataEncodingOptions options = default)
        {
            var actualDecoded = CrockfordBase32.GetInt64(encoded, options);
            Assert.AreEqual(raw, actualDecoded);

            string actualEncoded = CrockfordBase32.GetString(raw, options);
            Assert.AreEqual(encoded, actualEncoded);
        }

        static void TestVector(BigInteger raw, string encoded, DataEncodingOptions options = default)
        {
            var actualDecoded = CrockfordBase32.GetBigInteger(encoded, options);
            Assert.AreEqual(raw, actualDecoded);

            string actualEncoded = CrockfordBase32.GetString(raw, options);
            Assert.AreEqual(encoded, actualEncoded);
        }

        [TestMethod]
        public void CrockfordBase32_Empty() => TestVector("", "");

        [DataTestMethod]
        [DataRow("0Oo", "000")]
        [DataRow("1Ll", "111")]
        [DataRow("1Ii", "111")]
        [DataRow("Loi", "101")]
        public void CrockfordBase32_Canonicilization(string from, string to) =>
            Assert.AreEqual(
                to,
                CrockfordBase32.Instance.Canonicalize(from));

        [TestMethod]
        public void CrockfordBase32_Text_Main_TV1() => TestVector("f", "CR");

        [TestMethod]
        public void CrockfordBase32_Text_Main_TV2() => TestVector("fo", "CSQG");

        [TestMethod]
        public void CrockfordBase32_Text_Main_TV3() => TestVector("foo", "CSQPY");

        [TestMethod]
        public void CrockfordBase32_Text_Main_TV4() => TestVector("foob", "CSQPYRG");

        [TestMethod]
        public void CrockfordBase32_Text_Main_TV5() => TestVector("fooba", "CSQPYRK1");

        [TestMethod]
        public void CrockfordBase32_Text_Main_TV6() => TestVector("foobar", "CSQPYRK1E8");

        [TestMethod]
        public void CrockfordBase32_Text_Main_TV7() =>
            Assert.AreEqual(
                "foobar",
                Encoding.UTF8.GetString(CrockfordBase32.GetBytes("CsQP-YRkL-E8")));

        [TestMethod]
        public void CrockfordBase32_Int32_Main_TV1() => TestVector(1337, "19S");

        [TestMethod]
        public void CrockfordBase32_Int32_Main_TV2() => TestVector(1234, "16J");

        [TestMethod]
        public void CrockfordBase32_Int32_Main_TV3() => TestVector(5111, "4ZQ");

        [TestMethod]
        public void CrockfordBase32_Int32_Main_TV4() => TestVector(0, "0");

        [TestMethod]
        public void CrockfordBase32_Int32_Checksum_TV1() => TestVector(32, "10*", DataEncodingOptions.Checksum);

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void CrockfordBase32_Int32_Checksum_TV2() => TestVector(32, "10~", DataEncodingOptions.Checksum);

        [TestMethod]
        public void CrockfordBase32_Int32_Checksum_TV3() => TestVector(1234, "16JD", DataEncodingOptions.Checksum);

        [TestMethod]
        public void CrockfordBase32_Int32_Checksum_TV4() => TestVector(0, "00", DataEncodingOptions.Checksum);

        [TestMethod]
        public void CrockfordBase32_Int64_Main_TV1() => TestVector(1337L, "19S");

        [TestMethod]
        public void CrockfordBase32_Int64_Main_TV2() => TestVector(1234L, "16J");

        [TestMethod]
        public void CrockfordBase32_Int64_Main_TV3() => TestVector(5111L, "4ZQ");

        [TestMethod]
        public void CrockfordBase32_Int64_Checksum_TV1() => TestVector(32L, "10*", DataEncodingOptions.Checksum);

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void CrockfordBase32_Int64_Checksum_TV2() => TestVector(32L, "10~", DataEncodingOptions.Checksum);

        [TestMethod]
        public void CrockfordBase32_Int64_Checksum_TV3() => TestVector(1234L, "16JD", DataEncodingOptions.Checksum);

        [TestMethod]
        public void CrockfordBase32_Int64_Checksum_TV4() => TestVector(0L, "00", DataEncodingOptions.Checksum);

        [TestMethod]
        public void CrockfordBase32_BigInteger_Main_TV1() => TestVector(BigInteger.Zero, "0");

        [TestMethod]
        public void CrockfordBase32_BigInteger_Main_TV2() => TestVector(BigInteger.Parse("1234"), "16J");

        [TestMethod]
        public void CrockfordBase32_BigInteger_Main_TV3() => TestVector(BigInteger.Parse("3019140802085400304608040952"), "2E1BZQDAGC4G6TTENZR");

        [TestMethod]
        public void CrockfordBase32_BigInteger_Main_TV4() => TestVector(BigInteger.Parse("100000000000000000000"), "2PQHTY5NHH0000");

        [TestMethod]
        public void CrockfordBase32_BigInteger_Checksum_TV1() => TestVector(BigInteger.Zero, "00", DataEncodingOptions.Checksum);

        [TestMethod]
        public void CrockfordBase32_BigInteger_Checksum_TV2() => TestVector(BigInteger.Parse("1234"), "16JD", DataEncodingOptions.Checksum);

        [TestMethod]
        public void CrockfordBase32_RT_Random() => TextDataEncodingTestBench.RandomRoundTrip(CrockfordBase32.Instance, 16, 100000);

        [TestMethod]
        public void CrockfordBase32_RT_RandomWithPadding() => TextDataEncodingTestBench.RandomRoundTrip(CrockfordBase32.Instance, 16, 100000, DataEncodingOptions.Padding);
    }
}
