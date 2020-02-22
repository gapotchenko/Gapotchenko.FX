using Gapotchenko.FX.Data.Encoding.Test.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class Base16Tests
    {
        static void TestVector(string raw, string encoded)
        {
            var rawBytes = Encoding.UTF8.GetBytes(raw);

            // -----------------------------------------------------------------

            string actualEncoded = Base16.GetString(rawBytes);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = Base16.GetBytes(actualEncoded);
            Assert.IsTrue(rawBytes.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = Base16.Instance;

            Assert.AreEqual(Base16.Efficiency, instance.Efficiency);

            // -----------------------------------------------------------------

            TextDataEncodingTestServices.TestVector(instance, raw, encoded);
        }

        [TestMethod]
        public void Base16_Rfc4648_TV1() => TestVector("", "");

        [TestMethod]
        public void Base16_Rfc4648_TV2() => TestVector("f", "66");

        [TestMethod]
        public void Base16_Rfc4648_TV3() => TestVector("fo", "666F");

        [TestMethod]
        public void Base16_Rfc4648_TV4() => TestVector("foo", "666F6F");

        [TestMethod]
        public void Base16_Rfc4648_TV5() => TestVector("foob", "666F6F62");

        [TestMethod]
        public void Base16_Rfc4648_TV6() => TestVector("fooba", "666F6F6261");

        [TestMethod]
        public void Base16_Rfc4648_TV7() => TestVector("foobar", "666F6F626172");
    }
}
