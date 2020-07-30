using Gapotchenko.FX.Data.Encoding.Test.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    using Encoding = System.Text.Encoding;

    [TestClass]
    public class HenkeBase91Tests
    {
        static void TestVector(string raw, string encoded)
        {
            var rawBytes = Encoding.UTF8.GetBytes(raw);

            // -----------------------------------------------------------------

            //string actualEncoded = HenkeBase91.GetString(rawBytes);
            //Assert.AreEqual(encoded, actualEncoded);

            //var actualDecoded = HenkeBase91.GetBytes(actualEncoded);
            //Assert.IsTrue(rawBytes.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var instance = HenkeBase91.Instance;

            //Assert.AreEqual(HenkeBase91.Efficiency, instance.Efficiency);
            //Assert.AreEqual(HenkeBase91.Padding, instance.Padding);
            //Assert.IsFalse(instance.PrefersPadding);

            // -----------------------------------------------------------------

            TextDataEncodingTestBench.TestVector(instance, raw, encoded);
        }

        [TestMethod]
        public void HenkeBase91_TV1() => TestVector("", "");
    }
}
