﻿using Gapotchenko.FX.Data.Encoding.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Data.Encoding.Tests;

using Encoding = System.Text.Encoding;

[TestClass]
public class Base32HexTests
{
    static void TestVector(string raw, string encoded)
    {
        var rawBytes = Encoding.UTF8.GetBytes(raw);

        // -----------------------------------------------------------------

        string actualEncoded = Base32Hex.GetString(rawBytes);
        Assert.AreEqual(encoded, actualEncoded);

        var actualDecoded = Base32Hex.GetBytes(actualEncoded);
        Assert.IsTrue(rawBytes.SequenceEqual(actualDecoded));

        // -----------------------------------------------------------------

        var instance = Base32Hex.Instance;

        Assert.AreEqual(Base32Hex.Efficiency, instance.Efficiency);
        Assert.AreEqual(Base32Hex.Padding, instance.Padding);
        Assert.IsTrue(instance.PrefersPadding);

        // -----------------------------------------------------------------

        TextDataEncodingTestBench.TestVector(instance, raw, encoded);
    }

    [TestMethod]
    public void Base32Hex_Rfc4648_TV1() => TestVector("", "");

    [TestMethod]
    public void Base32Hex_Rfc4648_TV2() => TestVector("f", "CO======");

    [TestMethod]
    public void Base32Hex_Rfc4648_TV3() => TestVector("fo", "CPNG====");

    [TestMethod]
    public void Base32Hex_Rfc4648_TV4() => TestVector("foo", "CPNMU===");

    [TestMethod]
    public void Base32Hex_Rfc4648_TV5() => TestVector("foob", "CPNMUOG=");

    [TestMethod]
    public void Base32Hex_Rfc4648_TV6() => TestVector("fooba", "CPNMUOJ1");

    [TestMethod]
    public void Base32Hex_Rfc4648_TV7() => TestVector("foobar", "CPNMUOJ1E8======");

    [TestMethod]
    [DataRow(DataEncodingOptions.None)]
    [DataRow(DataEncodingOptions.NoPadding)]
    public void Base32Hex_RT_Random(DataEncodingOptions options) => TextDataEncodingTestBench.RandomRoundTrip(Base32Hex.Instance, 16, 100000, options);

    [TestMethod]
    [DataRow(TextDataEncodingTemplates.S1)]
    [DataRow(TextDataEncodingTemplates.S2)]
    [DataRow(TextDataEncodingTemplates.S3)]
    public void Base32Hex_RT_S(string s) => TextDataEncodingTestBench.RoundTrip(Base32Hex.Instance, s);
}
