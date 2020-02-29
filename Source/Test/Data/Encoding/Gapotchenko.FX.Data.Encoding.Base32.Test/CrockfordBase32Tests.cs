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
    public class CrockfordBase32Tests
    {
        static void TestVector(byte[] raw, string encoded, DataEncodingOptions options = default)
        {
            string actualEncoded = CrockfordBase32.GetString(raw, options);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = CrockfordBase32.GetBytes(encoded, options);
            Assert.IsTrue(raw.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = CrockfordBase32.Instance;

            Assert.AreEqual(CrockfordBase32.Efficiency, instance.Efficiency);

            // -----------------------------------------------------------------

            TextDataEncodingTestServices.TestVector(instance, raw, encoded, options: options);
        }

        static void TestVector(string raw, string encoded) => TestVector(Encoding.UTF8.GetBytes(raw), encoded);

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
        public void CrockfordBase32_Main_TV1() => TestVector("f", "CR");

        [TestMethod]
        public void CrockfordBase32_Main_TV2() => TestVector("fo", "CSQG");

        [TestMethod]
        public void CrockfordBase32_Main_TV3() => TestVector("foo", "CSQPY");

        [TestMethod]
        public void CrockfordBase32_Main_TV4() => TestVector("foob", "CSQPYRG");

        [TestMethod]
        public void CrockfordBase32_Main_TV5() => TestVector("fooba", "CSQPYRK1");

        [TestMethod]
        public void CrockfordBase32_Main_TV6() => TestVector("foobar", "CSQPYRK1E8");
    }
}
