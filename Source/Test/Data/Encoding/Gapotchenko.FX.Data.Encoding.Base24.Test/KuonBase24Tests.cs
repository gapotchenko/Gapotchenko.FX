using Gapotchenko.FX.Data.Encoding.Test.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class KuonBase24Tests
    {
        static void TestVector(byte[] raw, string encoded)
        {
            // -----------------------------------------------------------------

            string actualEncoded = KuonBase24.Instance.GetString(raw);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = KuonBase24.Instance.GetBytes(actualEncoded);
            Assert.IsTrue(raw.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = KuonBase24.Instance;

            Assert.AreEqual(KuonBase24.Efficiency, instance.Efficiency);

            // -----------------------------------------------------------------

            TextDataEncodingTestBench.TestVector(instance, raw, encoded);
        }

        static void TestVector(string raw, string encoded) => TestVector(Encoding.UTF8.GetBytes(raw), encoded);

        [TestMethod]
        public void KuonBase24_Empty() => TestVector("", "");

        [TestMethod]
        public void KuonBase24_TV1() => TestVector(Base16.GetBytes("00000000"), "ZZZZZZZ");

        [TestMethod]
        public void KuonBase24_TV2() => TestVector(Base16.GetBytes("000000000000000000000000"), "ZZZZZZZZZZZZZZZZZZZZZ");

        [TestMethod]
        public void KuonBase24_TV3() => TestVector(Base16.GetBytes("00000001"), "ZZZZZZA");
    }
}
