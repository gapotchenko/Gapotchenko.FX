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

            // -----------------------------------------------------------------
            // Parameters normalization
            // -----------------------------------------------------------------

            if (textEncoding == null)
                textEncoding = Encoding.ASCII;

            // -----------------------------------------------------------------
            // Data preparation
            // -----------------------------------------------------------------

            var rawBytes = textEncoding.GetBytes(raw);
            var encodedBytes = textEncoding.GetBytes(encoded);

            // -----------------------------------------------------------------
            // Check text-based data encoding API
            // -----------------------------------------------------------------

            string actualEncoded = dataEncoding.GetString(rawBytes);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = dataEncoding.GetBytes(actualEncoded);
            Assert.IsTrue(rawBytes.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------
            // Check padding operations
            // -----------------------------------------------------------------

            var actualEncodedUnpadded = dataEncoding.Unpad(actualEncoded).ToString();
            string actualEncodedRepadded = dataEncoding.Pad(actualEncodedUnpadded);

            Assert.AreEqual(actualEncoded, actualEncodedRepadded);

            if (dataEncoding.Padding == 1)
            {
                Assert.AreEqual(actualEncodedUnpadded, actualEncoded);
                Assert.AreEqual(actualEncodedUnpadded, actualEncodedRepadded);
            }

            string actualEncodedOverpadded = dataEncoding.Pad(actualEncoded);
            Assert.AreEqual(actualEncoded, actualEncodedOverpadded);

            string actualEncodedUnderpadded = dataEncoding.Unpad(actualEncodedUnpadded).ToString();
            Assert.AreEqual(actualEncodedUnpadded, actualEncodedUnderpadded);

            Assert.IsTrue(rawBytes.SequenceEqual(dataEncoding.GetBytes(actualEncodedUnpadded)), "Cannot decode unpadded string.");

            string actualEncodedWithoutPadding = dataEncoding.GetString(rawBytes, DataTextEncodingOptions.NoPadding);
            Assert.AreEqual(actualEncodedUnpadded, actualEncodedWithoutPadding, "DataTextEncodingOptions.NoPadding is not honored.");

            // -----------------------------------------------------------------
            // Check the general data encoding API
            // -----------------------------------------------------------------

            var actualEncodedBytes = dataEncoding.EncodeData(rawBytes);
            Assert.IsTrue(encodedBytes.SequenceEqual(actualEncodedBytes));

            var actualDecodedBytes = dataEncoding.DecodeData(actualEncodedBytes);
            Assert.IsTrue(rawBytes.SequenceEqual(actualDecodedBytes));

            // -----------------------------------------------------------------
            // Check actual encoding efficiency and boundaries
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
