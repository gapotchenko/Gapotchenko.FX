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
        public void CrockfordBase32_TV1() => TestVector("", "");

        [TestMethod]
        public void CrockfordBase32_TV2() => TestVector("f", "CR");

        [TestMethod]
        public void CrockfordBase32_TV3() => TestVector("fo", "CSQG");

        [TestMethod]
        public void CrockfordBase32_TV4() => TestVector("foo", "CSQPY");

        [TestMethod]
        public void CrockfordBase32_TV5() => TestVector("foob", "CSQPYRG");

        [TestMethod]
        public void CrockfordBase32_TV6() => TestVector("fooba", "CSQPYRK1");

        [TestMethod]
        public void CrockfordBase32_TV7() => TestVector("foobar", "CSQPYRK1E8");

        [TestMethod]
        public void CrockfordBase32_TV8() =>
            Assert.AreEqual(
                "foobar",
                Encoding.UTF8.GetString(CrockfordBase32.GetBytes("CSQP-YRK1-E8")));
    }
}
