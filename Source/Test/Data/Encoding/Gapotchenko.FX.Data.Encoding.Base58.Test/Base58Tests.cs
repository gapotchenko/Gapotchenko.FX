using Gapotchenko.FX.Data.Encoding.Test.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Numerics;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class Base58Tests
    {
        #region Data

        static void TestVector(byte[] raw, string encoded, DataEncodingOptions options = default)
        {
            //string actualEncoded = Base58.GetString(raw, options);
            //Assert.AreEqual(encoded, actualEncoded);

            //var actualDecoded = Base58.GetBytes(actualEncoded, options);
            //Assert.IsTrue(raw.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = Base58.Instance;

            Assert.AreEqual(Base58.Efficiency, instance.Efficiency);
            Assert.IsFalse(instance.PrefersPadding);

            // -----------------------------------------------------------------

            TextDataEncodingTestBench.TestVector(instance, raw, encoded, options);
        }

        static void TestVector(string raw, string encoded, DataEncodingOptions options = default) =>
            TestVector(Encoding.UTF8.GetBytes(raw), encoded, options);

        [TestMethod]
        public void Base58_Data_Empty() => TestVector("", "");

        [TestMethod]
        public void Base58_Data_TV1() => TestVector(Base16.GetBytes("00"), "1");

        [TestMethod]
        public void Base58_Data_TV2() => TestVector(Base16.GetBytes("00 00 00 00 00 00 00 00"), "11111111");

        [TestMethod]
        public void Base58_Data_TV3() => TestVector("Hello World", "JxF12TrwUP45BMd");

        [TestMethod]
        public void Base58_Data_TV4() => TestVector(Base16.GetBytes("61"), "2g");

        [TestMethod]
        public void Base58_Data_TV5() => TestVector(Base16.GetBytes("62 62 62"), "a3gV");

        [TestMethod]
        public void Base58_Data_TV6() => TestVector(Base16.GetBytes("63 63 63"), "aPEr");

        [TestMethod]
        public void Base58_Data_TV7() => TestVector(Base16.GetBytes("73 69 6D 70 6C 79 20 61 20 6C 6F 6E 67 20 73 74 72 69 6E 67"), "2cFupjhnEsSn59qHXstmK2ffpLv2");

        [TestMethod]
        public void Base58_Data_TV8() => TestVector(Base16.GetBytes("00 EB 15 23 1D FC EB 60 92 58 86 B6 7D 06 52 99 92 59 15 AE B1 72 C0 66 47"), "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L");

        [TestMethod]
        public void Base58_Data_TV9() => TestVector(Base16.GetBytes("1111111111"), "2vgLdhi");

        [TestMethod]
        public void Base58_Data_TV10() => TestVector(Base16.GetBytes("00 01"), "12");

        [TestMethod]
        public void Base58_Data_TV11() => TestVector(Base16.GetBytes("00 00 01 02 03"), "11Ldp");

        [TestMethod]
        public void Base58_Data_TV12() => TestVector(Base16.GetBytes("009C1CA2CBA6422D3988C735BB82B5C880B0441856B9B0910F"), "1FESiat4YpNeoYhW3Lp7sW1T6WydcW7vcE");

        [TestMethod]
        public void Base58_Data_TV13() => TestVector(Base16.GetBytes("000860C220EBBAF591D40F51994C4E2D9C9D88168C33E761F6"), "1mJKRNca45GU2JQuHZqZjHFNktaqAs7gh");

        [TestMethod]
        public void Base58_Data_TV14() => TestVector(Base16.GetBytes("00313E1F905554E7AE2580CD36F86D0C8088382C9E1951C44D010203"), "17f1hgANcLE5bQhAGRgnBaLTTs23rK4VGVKuFQ");

        [TestMethod]
        public void Base58_Data_TV15() => TestVector(Base16.GetBytes("FFEEDDCCBBAA"), "3CSwN61PP");

        [TestMethod]
        public void Base58_Data_TV16() => TestVector(Base16.GetBytes("000102030405060708090A0B0C0D0E0F000102030405060708090A0B0C0D0E0F"), "1thX6LZfHDZZKUs92febWaf4WJZnsKRiVwJusXxB7L");

        [TestMethod]
        public void Base58_Data_TV17() => TestVector(Base16.GetBytes("51 6B 6F CD 0F"), "ABnLTmg");

        [TestMethod]
        public void Base58_Data_TV18() => TestVector(Base16.GetBytes("BF 4F 89 00 1E 67 02 74 DD"), "3SEo3LWLoPntC");

        [TestMethod]
        public void Base58_Data_TV20() => TestVector(Base16.GetBytes("57 2E 47 94"), "3EFU7m");

        [TestMethod]
        public void Base58_Data_TV21() => TestVector(Base16.GetBytes("EC AC 89 CA D9 39 23 C0 23 21"), "EJDM8drfXA6uyA");

        [TestMethod]
        public void Base58_Data_TV22() => TestVector(Base16.GetBytes("00000000000000000000123456789ABCDEF0"), "111111111143c9JGph3DZ");

        [TestMethod]
        public void Base58_Data_TV23() => TestVector(Base16.GetBytes("10 C8 51 1E"), "Rt5zm");

        [TestMethod]
        public void Base58_Data_TV24() => TestVector("1234598760", "3mJr7AoUXx2Wqd");

        [TestMethod]
        public void Base58_Data_TV25() => TestVector("abcdefghijklmnopqrstuvwxyz", "3yxU3u1igY8WkgtjK92fbJQCd4BZiiT1v25f");

        [TestMethod]
        public void Base58_Data_TV26() => TestVector("abc", "ZiCa");

        [TestMethod]
        public void Base58_Data_TV27() => TestVector("\0abc", "1ZiCa");

        [TestMethod]
        public void Base58_Data_TV28() => TestVector("\0\0abc", "11ZiCa");

        [TestMethod]
        public void Base58_Data_TV29() => TestVector("\0\0\0abc", "111ZiCa");

        [TestMethod]
        public void Base58_Data_TV30() => TestVector("\0\0\0\0abc", "1111ZiCa");

        [TestMethod]
        public void Base58_Data_TV31() => TestVector("hello world", "3vQB7B6MrGQZaxCuFg4oh", DataEncodingOptions.Checksum);

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Base58_Data_TV32() => Base58.Instance.GetBytes("3vQB7B6MrGQZaxCuFg4oH".AsSpan(), DataEncodingOptions.Checksum);

        [DataTestMethod]
        [DataRow(DataEncodingOptions.None)]
        [DataRow(DataEncodingOptions.Checksum)]
        public void Base58_Data_RT_Random(DataEncodingOptions options) => TextDataEncodingTestBench.RandomRoundTrip(Base58.Instance, 32, 100000, options);

        #endregion

        #region BigInteger

        static void TestVector(BigInteger raw, string encoded, DataEncodingOptions options = default)
        {
            //string actualEncoded = Base58.GetString(raw, options);
            //Assert.AreEqual(encoded, actualEncoded);

            //var actualDecoded = Base58.GetBigInteger(actualEncoded, options);
            //Assert.IsTrue(raw.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = Base58.Instance;

            TextDataEncodingTestBench.TestVector(instance, raw, encoded, options);
        }

        [TestMethod]
        public void Base58_BigInteger_TV1() => TestVector(BigInteger.Zero, "1");

        [TestMethod]
        public void Base58_BigInteger_TV2() => TestVector(BigInteger.One, "2");

        [TestMethod]
        public void Base58_BigInteger_TV3() => TestVector(new BigInteger(3471844090), "6Ho7Hs");

        [TestMethod]
        public void Base58_BigInteger_TV4() => TestVector(
            BigInteger.Parse("111d38e5fc9071ffcd20b4a763cc9ae4f252bb4e48fd66a835e252ada93ff480d6dd43dc62a641155a5", NumberStyles.HexNumber),
            "23456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz");

        [TestMethod]
        public void Base58_BigInteger_TV5() => TextDataEncodingTestBench.TestVector(
            new CustomBase58("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuv"),
            new BigInteger(987654321),
            "1TFvCj");

        [DataTestMethod]
        [DataRow(DataEncodingOptions.None)]
        [DataRow(DataEncodingOptions.Checksum)]
        public void Base58_BigInteger_RT(DataEncodingOptions options) =>
            TextDataEncodingTestBench.RandomRoundTrip(
                Base58.Instance,
                BigInteger.Zero,
                BigInteger.Parse("0fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff", NumberStyles.HexNumber),
                50000,
                options);

        #endregion
    }
}
