using Gapotchenko.FX.Data.Encoding.Test;
using Gapotchenko.FX.Data.Encoding.Test.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class Base32Tests
    {
        static void TestVector(string raw, string encoded)
        {
            var rawBytes = Encoding.UTF8.GetBytes(raw);

            // -----------------------------------------------------------------

            string actualEncoded = Base32.GetString(rawBytes);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = Base32.GetBytes(actualEncoded);
            Assert.IsTrue(rawBytes.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = Base32.Instance;

            Assert.AreEqual(Base32.Padding, instance.Padding);
            Assert.AreEqual(Base32.Efficiency, instance.Efficiency);

            // -----------------------------------------------------------------

            DataTextEncodingTestServices.TestVector(instance, raw, encoded);
        }

        [TestMethod]
        public void Base32_Rfc4648_TV1() => TestVector("", "");

        [TestMethod]
        public void Base32_Rfc4648_TV2() => TestVector("f", "MY======");

        [TestMethod]
        public void Base32_Rfc4648_TV3() => TestVector("fo", "MZXQ====");

        [TestMethod]
        public void Base32_Rfc4648_TV4() => TestVector("foo", "MZXW6===");

        [TestMethod]
        public void Base32_Rfc4648_TV5() => TestVector("foob", "MZXW6YQ=");

        [TestMethod]
        public void Base32_Rfc4648_TV6() => TestVector("fooba", "MZXW6YTB");

        [TestMethod]
        public void Base32_Rfc4648_TV7() => TestVector("foobar", "MZXW6YTBOI======");
    }
}
