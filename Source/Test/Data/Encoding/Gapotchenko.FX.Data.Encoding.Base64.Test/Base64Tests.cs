﻿using Gapotchenko.FX.Data.Encoding.Test.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class Base64Tests
    {
        static void TestVector(string raw, string encoded)
        {
            var rawBytes = Encoding.UTF8.GetBytes(raw);

            // -----------------------------------------------------------------

            string actualEncoded = Base64.GetString(rawBytes);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = Base64.GetBytes(actualEncoded);
            Assert.IsTrue(rawBytes.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = Base64.Instance;

            Assert.AreEqual(Base64.Efficiency, instance.Efficiency);
            Assert.AreEqual(Base64.Padding, instance.Padding);

            // -----------------------------------------------------------------

            TextDataEncodingTestServices.TestVector(instance, raw, encoded);
        }

        [TestMethod]
        public void Base64_Rfc4648_TV1() => TestVector("", "");

        [TestMethod]
        public void Base64_Rfc4648_TV2() => TestVector("f", "Zg==");

        [TestMethod]
        public void Base64_Rfc4648_TV3() => TestVector("fo", "Zm8=");

        [TestMethod]
        public void Base64_Rfc4648_TV4() => TestVector("foo", "Zm9v");

        [TestMethod]
        public void Base64_Rfc4648_TV5() => TestVector("foob", "Zm9vYg==");

        [TestMethod]
        public void Base64_Rfc4648_TV6() => TestVector("fooba", "Zm9vYmE=");

        [TestMethod]
        public void Base64_Rfc4648_TV7() => TestVector("foobar", "Zm9vYmFy");

        [TestMethod]
        public void Base64_PadCon() =>
            Assert.IsTrue(
                Base64.GetBytes("SQ==QU0=VEpN")
                .SequenceEqual(Encoding.ASCII.GetBytes("IAMTJM")));

        [TestMethod]
        public void Base64_PadCon_Invalid_Consume() =>
            Assert.IsTrue(
                Base64.GetBytes("SQ=QU0=VEpN")
                .SequenceEqual(Encoding.ASCII.GetBytes("IAMTJM")));

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Base64_PadCon_Invalid_Check() =>
            Assert.IsTrue(
                Base64.GetBytes("SQ=QU0=VEpN", DataEncodingOptions.Pad)
                .SequenceEqual(Encoding.ASCII.GetBytes("IAMTJM")));

        [TestMethod]
        public void Base64_PadCon_Relax() =>
            Assert.IsTrue(
                Base64.GetBytes("SQ=Я=QU0=VEpN", DataEncodingOptions.Pad | DataEncodingOptions.Relax)
                .SequenceEqual(Encoding.ASCII.GetBytes("IAMTJM")));
    }
}
