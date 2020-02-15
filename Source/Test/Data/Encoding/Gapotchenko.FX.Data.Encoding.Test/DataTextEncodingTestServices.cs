using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    using Encoding = System.Text.Encoding;

    public static class DataTextEncodingTestServices
    {
        public static void TestVector(
            IDataTextEncoding dataEncoding,
            string raw,
            string encoded,
            Encoding textEncoding = null)
        {
            if (dataEncoding == null)
                throw new ArgumentNullException(nameof(dataEncoding));
            if (raw == null)
                throw new ArgumentNullException(nameof(raw));
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));

            if (textEncoding == null)
                textEncoding = Encoding.ASCII;

            var rawBytes = textEncoding.GetBytes(raw);
            var encodedBytes = textEncoding.GetBytes(encoded);

            // -----------------------------------------------------------------

            string actualEncoded = dataEncoding.GetString(rawBytes);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = dataEncoding.GetBytes(actualEncoded);
            Assert.IsTrue(rawBytes.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------

            var actualEncodedUnpadded = dataEncoding.Unpad(actualEncoded);
            string actualEncodedRepadded = dataEncoding.Pad(actualEncodedUnpadded);

            Assert.AreEqual(actualEncoded, actualEncodedRepadded);

            if (dataEncoding.Padding == 1)
            {
                Assert.AreEqual(actualEncodedUnpadded.ToString(), actualEncoded);
                Assert.AreEqual(actualEncodedUnpadded.ToString(), actualEncodedRepadded);
            }

            string actualEncodedOverpadded = dataEncoding.Pad(actualEncoded);
            Assert.AreEqual(actualEncoded, actualEncodedOverpadded);

            var actualEncodedUnderpadded = dataEncoding.Unpad(actualEncodedUnpadded);
            Assert.AreEqual(actualEncodedUnpadded.ToString(), actualEncodedUnderpadded.ToString());

            // -----------------------------------------------------------------

            var actualEncodedBytes = dataEncoding.EncodeData(rawBytes);
            Assert.IsTrue(encodedBytes.SequenceEqual(actualEncodedBytes));

            var actualDecodedBytes = dataEncoding.DecodeData(actualEncodedBytes);
            Assert.IsTrue(rawBytes.SequenceEqual(actualDecodedBytes));

            // -----------------------------------------------------------------

            int actualEncodedBytesCount = textEncoding.GetByteCount(actualEncodedUnpadded);
            if (actualEncodedBytesCount > 0)
            {
                float actualEfficiencyCeiling = (float)rawBytes.Length / actualEncodedBytesCount;
                Assert.IsTrue(actualEfficiencyCeiling <= dataEncoding.MaxEfficiency, "Max encoding efficiency violated.");

                if (actualEncodedBytesCount > 1)
                {
                    float actualEfficiencyFloor = (float)rawBytes.Length / (actualEncodedBytesCount - 1);
                    Assert.IsTrue(actualEfficiencyFloor >= dataEncoding.MinEfficiency, "Min encoding efficiency violated.");
                }
            }
        }
    }
}
