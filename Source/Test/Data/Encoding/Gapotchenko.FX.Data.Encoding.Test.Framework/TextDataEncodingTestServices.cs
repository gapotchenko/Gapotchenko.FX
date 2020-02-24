﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding.Test.Framework
{
    using Encoding = System.Text.Encoding;

    public static class TextDataEncodingTestServices
    {
        public static void TestVector(
            ITextDataEncoding dataEncoding,
            string raw,
            string encoded,
            bool padded = true,
            Encoding textEncoding = null,
            DataEncodingOptions options = DataEncodingOptions.None)
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
                textEncoding = Encoding.UTF8;

            // -----------------------------------------------------------------
            // Data preparation
            // -----------------------------------------------------------------

            var rawBytes = textEncoding.GetBytes(raw);

            // -----------------------------------------------------------------

            TestVector(dataEncoding, rawBytes, encoded, padded, textEncoding, options);
        }

        public static void TestVector(
            ITextDataEncoding dataEncoding,
            byte[] raw,
            string encoded,
            bool padded = true,
            Encoding textEncoding = null,
            DataEncodingOptions options = DataEncodingOptions.None)
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
                textEncoding = Encoding.UTF8;

            // -----------------------------------------------------------------
            // Data preparation
            // -----------------------------------------------------------------

            var encodedBytes = textEncoding.GetBytes(encoded);

            // -----------------------------------------------------------------
            // Check text-based data encoding API
            // -----------------------------------------------------------------

            string actualEncoded = dataEncoding.GetString(raw, options);
            Assert.AreEqual(encoded, actualEncoded);

            var actualDecoded = dataEncoding.GetBytes(actualEncoded.AsSpan(), options);
            Assert.IsTrue(raw.SequenceEqual(actualDecoded));

            // -----------------------------------------------------------------
            // Check padding operations
            // -----------------------------------------------------------------

            var actualEncodedUnpadded = dataEncoding.Unpad(actualEncoded.AsSpan()).ToString();
            string actualEncodedRepadded = dataEncoding.Pad(actualEncodedUnpadded.AsSpan());

            if (padded)
                Assert.AreEqual(actualEncoded, actualEncodedRepadded);
            Assert.IsTrue(dataEncoding.IsPadded(actualEncodedRepadded.AsSpan()));

            if (dataEncoding.Padding == 1)
            {
                Assert.AreEqual(actualEncodedUnpadded, actualEncoded);
                Assert.AreEqual(actualEncodedUnpadded, actualEncodedRepadded);
            }

            string actualEncodedOverpadded = dataEncoding.Pad(actualEncoded.AsSpan());
            if (padded)
                Assert.AreEqual(actualEncoded, actualEncodedOverpadded);

            string actualEncodedUnderpadded = dataEncoding.Unpad(actualEncodedUnpadded.AsSpan()).ToString();
            Assert.AreEqual(actualEncodedUnpadded, actualEncodedUnderpadded);

            Assert.IsTrue(raw.SequenceEqual(dataEncoding.GetBytes(actualEncodedUnpadded.AsSpan(), options)), "Cannot decode unpadded string.");

            string actualEncodedWithoutPadding = dataEncoding.GetString(raw, options | DataEncodingOptions.Unpad);
            Assert.AreEqual(actualEncodedUnpadded, actualEncodedWithoutPadding, "DataTextEncodingOptions.NoPadding is not honored.");

            // -----------------------------------------------------------------
            // Check the general data encoding API
            // -----------------------------------------------------------------

            var actualEncodedBytes = dataEncoding.EncodeData(raw, options);
            Assert.IsTrue(encodedBytes.SequenceEqual(actualEncodedBytes));

            var actualDecodedBytes = dataEncoding.DecodeData(actualEncodedBytes, options);
            Assert.IsTrue(raw.SequenceEqual(actualDecodedBytes));

            // -----------------------------------------------------------------
            // Check actual encoding efficiency and boundaries
            // -----------------------------------------------------------------

            int actualEncodedBytesCount = textEncoding.GetByteCount(actualEncodedUnpadded);
            if (actualEncodedBytesCount > 0)
            {
                if ((options & DataEncodingOptions.Compress) == 0)
                {
                    float actualEfficiencyCeiling = (float)raw.Length / actualEncodedBytesCount;
                    Assert.IsTrue(actualEfficiencyCeiling <= dataEncoding.MaxEfficiency, "Max encoding efficiency violated.");
                }

                if (actualEncodedBytesCount > 1)
                {
                    float actualEfficiencyFloor = (float)raw.Length / (actualEncodedBytesCount - 1);
                    Assert.IsTrue(actualEfficiencyFloor >= dataEncoding.MinEfficiency, "Min encoding efficiency violated.");
                }
            }
        }
    }
}
