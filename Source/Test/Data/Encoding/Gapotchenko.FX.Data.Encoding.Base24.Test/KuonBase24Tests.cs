using Gapotchenko.FX.Data.Encoding.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Encoding.Tests
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class KuonBase24Tests
    {
        static void TestVector(ReadOnlySpan<byte> raw, string encoded)
        {
            string actualEncoded = KuonBase24.GetString(raw);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = KuonBase24.GetBytes(actualEncoded);
            Assert.IsTrue(raw.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = KuonBase24.Instance;

            Assert.AreEqual(KuonBase24.Efficiency, instance.Efficiency);
            Assert.IsFalse(instance.PrefersPadding);

            // -----------------------------------------------------------------

            TextDataEncodingTestBench.TestVector(instance, raw, encoded);
        }

        static void TestVector(string raw, string encoded) => TestVector(Encoding.UTF8.GetBytes(raw), encoded);

        static void RoundTrip(ReadOnlySpan<byte> raw) => TextDataEncodingTestBench.RoundTrip(KuonBase24.Instance, raw);

        [TestMethod]
        public void KuonBase24_Empty() => TestVector("", "");

        [TestMethod]
        public void KuonBase24_TV1() => TestVector(Base16.GetBytes("00000000"), "ZZZZZZZ");

        [TestMethod]
        public void KuonBase24_TV2() => TestVector(Base16.GetBytes("000000000000000000000000"), "ZZZZZZZZZZZZZZZZZZZZZ");

        [TestMethod]
        public void KuonBase24_TV3() => TestVector(Base16.GetBytes("00000001"), "ZZZZZZA");

        [TestMethod]
        public void KuonBase24_TV4() => TestVector(Base16.GetBytes("000000010000000100000001"), "ZZZZZZAZZZZZZAZZZZZZA");

        [TestMethod]
        public void KuonBase24_TV5() => TestVector(Base16.GetBytes("00000010"), "ZZZZZZP");

        [TestMethod]
        public void KuonBase24_TV6() => TestVector(Base16.GetBytes("00000030"), "ZZZZZCZ");

        [TestMethod]
        public void KuonBase24_TV7() => TestVector(Base16.GetBytes("88553311"), "5YEATXA");

        [TestMethod]
        public void KuonBase24_TV8() => TestVector(Base16.GetBytes("FFFFFFFF"), "X5GGBH7");

        [TestMethod]
        public void KuonBase24_TV9() => TestVector(Base16.GetBytes("FFFFFFFFFFFFFFFFFFFFFFFF"), "X5GGBH7X5GGBH7X5GGBH7");

        [TestMethod]
        public void KuonBase24_TV10() => TestVector(Base16.GetBytes("1234567887654321"), "A64KHWZ5WEPAGG");

        [TestMethod]
        public void KuonBase24_TV11() => TestVector(Base16.GetBytes("FF0001FF001101FF01023399"), "XGES63FZZ247C7ZC2ZA6G");

        [TestMethod]
        public void KuonBase24_TV12() => TestVector(Base16.GetBytes("25896984125478546598563251452658"), "2FC28KTA66WRST4XAHRRCF237S8Z");

        [TestMethod]
        public void KuonBase24_TV13() => TestVector(Base16.GetBytes("00000001"), "ZZZZZZA");

        [TestMethod]
        public void KuonBase24_TV14() => TestVector(Base16.GetBytes("00000002"), "ZZZZZZC");

        [TestMethod]
        public void KuonBase24_TV15() => TestVector(Base16.GetBytes("00000004"), "ZZZZZZB");

        [TestMethod]
        public void KuonBase24_TV16() => TestVector(Base16.GetBytes("00000008"), "ZZZZZZ4");

        [TestMethod]
        public void KuonBase24_TV17() => TestVector(Base16.GetBytes("00000010"), "ZZZZZZP");

        [TestMethod]
        public void KuonBase24_TV18() => TestVector(Base16.GetBytes("00000020"), "ZZZZZA4");

        [TestMethod]
        public void KuonBase24_TV19() => TestVector(Base16.GetBytes("00000040"), "ZZZZZCP");

        [TestMethod]
        public void KuonBase24_TV20() => TestVector(Base16.GetBytes("00000080"), "ZZZZZ34");

        [TestMethod]
        public void KuonBase24_TV21() => TestVector(Base16.GetBytes("00000100"), "ZZZZZHP");

        [TestMethod]
        public void KuonBase24_TV22() => TestVector(Base16.GetBytes("00000200"), "ZZZZZW4");

        [TestMethod]
        public void KuonBase24_TV23() => TestVector(Base16.GetBytes("00000400"), "ZZZZARP");

        [TestMethod]
        public void KuonBase24_TV24() => TestVector(Base16.GetBytes("00000800"), "ZZZZ2K4");

        [TestMethod]
        public void KuonBase24_TV25() => TestVector(Base16.GetBytes("00001000"), "ZZZZFCP");

        [TestMethod]
        public void KuonBase24_TV26() => TestVector(Base16.GetBytes("00002000"), "ZZZZ634");

        [TestMethod]
        public void KuonBase24_TV27() => TestVector(Base16.GetBytes("00004000"), "ZZZABHP");

        [TestMethod]
        public void KuonBase24_TV28() => TestVector(Base16.GetBytes("00008000"), "ZZZC4W4");

        [TestMethod]
        public void KuonBase24_TV29() => TestVector(Base16.GetBytes("00010000"), "ZZZB8RP");

        [TestMethod]
        public void KuonBase24_TV30() => TestVector(Base16.GetBytes("00020000"), "ZZZG5K4");

        [TestMethod]
        public void KuonBase24_TV31() => TestVector(Base16.GetBytes("00040000"), "ZZZRYCP");

        [TestMethod]
        public void KuonBase24_TV32() => TestVector(Base16.GetBytes("00080000"), "ZZAKX34");

        [TestMethod]
        public void KuonBase24_TV33() => TestVector(Base16.GetBytes("00100000"), "ZZ229HP");

        [TestMethod]
        public void KuonBase24_TV34() => TestVector(Base16.GetBytes("00200000"), "ZZEFPW4");

        [TestMethod]
        public void KuonBase24_TV35() => TestVector(Base16.GetBytes("00400000"), "ZZT7GRP");

        [TestMethod]
        public void KuonBase24_TV36() => TestVector(Base16.GetBytes("00800000"), "ZAAESK4");

        [TestMethod]
        public void KuonBase24_TV37() => TestVector(Base16.GetBytes("01000000"), "ZCCK7CP");

        [TestMethod]
        public void KuonBase24_TV38() => TestVector(Base16.GetBytes("02000000"), "ZB32E34");

        [TestMethod]
        public void KuonBase24_TV39() => TestVector(Base16.GetBytes("04000000"), "Z4HETHP");

        [TestMethod]
        public void KuonBase24_TV40() => TestVector(Base16.GetBytes("08000000"), "ZP9KZW4");

        [TestMethod]
        public void KuonBase24_TV41() => TestVector(Base16.GetBytes("10000000"), "AG8CARP");

        [TestMethod]
        public void KuonBase24_TV42() => TestVector(Base16.GetBytes("20000000"), "CSHB2K4");

        [TestMethod]
        public void KuonBase24_TV43() => TestVector(Base16.GetBytes("40000000"), "3694FCP");

        [TestMethod]
        public void KuonBase24_TV44() => TestVector(Base16.GetBytes("80000000"), "53PP634");

        [TestMethod]
        public void KuonBase24_RT1() => RoundTrip(Base16.GetBytes("00"));

        [TestMethod]
        public void KuonBase24_RT2() => RoundTrip(Base16.GetBytes("01"));

        [TestMethod]
        public void KuonBase24_RT3() => RoundTrip(Base16.GetBytes("FF"));

        [TestMethod]
        public void KuonBase24_RT4() => RoundTrip(Base16.GetBytes("FF 00"));

        [TestMethod]
        public void KuonBase24_RT5() => RoundTrip(Base16.GetBytes("01 02"));

        [TestMethod]
        public void KuonBase24_RT6() => RoundTrip(Base16.GetBytes("01 02 03"));

        [TestMethod]
        public void KuonBase24_RT7() => RoundTrip(Base16.GetBytes("00 90 60"));

        [TestMethod]
        public void KuonBase24_RT8() => RoundTrip(Base16.GetBytes("82 4A 13 E8 38"));

        [DataTestMethod]
        [DataRow(DataEncodingOptions.None)]
        [DataRow(DataEncodingOptions.Padding)]
        public void KuonBase24_RT_Random(DataEncodingOptions options) => TextDataEncodingTestBench.RandomRoundTrip(KuonBase24.Instance, 16, 100000, options);

        [DataTestMethod]
        // S1
        [DataRow(TextDataEncodingTemplates.S1, DataEncodingOptions.None)]
        [DataRow(TextDataEncodingTemplates.S1, DataEncodingOptions.Padding)]
        // S2
        [DataRow(TextDataEncodingTemplates.S2, DataEncodingOptions.None)]
        [DataRow(TextDataEncodingTemplates.S2, DataEncodingOptions.Padding)]
        // S3
        [DataRow(TextDataEncodingTemplates.S3, DataEncodingOptions.None)]
        [DataRow(TextDataEncodingTemplates.S3, DataEncodingOptions.Padding)]
        public void KuonBase24_RT_S(string s, DataEncodingOptions options) => TextDataEncodingTestBench.RoundTrip(KuonBase24.Instance, s, options);
    }
}
