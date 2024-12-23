﻿using Gapotchenko.FX.Data.Encoding.Tests.Bench;
using Gapotchenko.FX.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Data.Encoding.Tests;

using Encoding = System.Text.Encoding;

[TestClass]
public class Base64UrlTests
{
    static void TestVector(string raw, string encoded)
    {
        var rawBytes = Encoding.UTF8.GetBytes(raw);

        // -----------------------------------------------------------------

        string actualEncoded = Base64Url.GetString(rawBytes);
        Assert.AreEqual(encoded, actualEncoded);

        var actualDecoded = Base64Url.GetBytes(actualEncoded);
        Assert.IsTrue(rawBytes.SequenceEqual(actualDecoded));

        // -----------------------------------------------------------------

        var instance = Base64Url.Instance;

        Assert.AreEqual(Base64Url.Efficiency, instance.Efficiency);
        Assert.IsFalse(instance.PrefersPadding);
        Assert.IsFalse(actualEncoded.EndsWith('='));

        // -----------------------------------------------------------------

        TextDataEncodingTestBench.TestVector(instance, raw, encoded);
    }

    [TestMethod]
    public void Base64Url_Rfc4648_TV1() => TestVector("", "");

    [TestMethod]
    public void Base64Url_Rfc4648_TV2() => TestVector("f", "Zg");

    [TestMethod]
    public void Base64Url_Rfc4648_TV3() => TestVector("fo", "Zm8");

    [TestMethod]
    public void Base64Url_Rfc4648_TV4() => TestVector("foo", "Zm9v");

    [TestMethod]
    public void Base64Url_Rfc4648_TV5() => TestVector("foob", "Zm9vYg");

    [TestMethod]
    public void Base64Url_Rfc4648_TV6() => TestVector("fooba", "Zm9vYmE");

    [TestMethod]
    public void Base64Url_Rfc4648_TV7() => TestVector("foobar", "Zm9vYmFy");

    [TestMethod]
    public void Base64Url_Pad_Hello() =>
        Assert.AreEqual(
            "SGVsbG8",
            Base64Url.GetString(Encoding.UTF8.GetBytes("Hello")));

    [TestMethod]
    public void Base64Url_Pad_1()
    {
        var data = Encoding.UTF8.GetBytes("1");
        Assert.AreEqual("MQ", Base64Url.GetString(data));
        Assert.AreEqual("MQ==", Base64Url.GetString(data, DataEncodingOptions.Padding));
    }

    [TestMethod]
    public void Base64Url_Pad_11()
    {
        var data = Encoding.UTF8.GetBytes("11");
        Assert.AreEqual("MTE", Base64Url.GetString(data));
        Assert.AreEqual("MTE=", Base64Url.GetString(data, DataEncodingOptions.Padding));
    }

    [TestMethod]
    public void Base64Url_Pad_111()
    {
        var data = Encoding.UTF8.GetBytes("111");
        Assert.AreEqual("MTEx", Base64Url.GetString(data));
        Assert.AreEqual("MTEx", Base64Url.GetString(data, DataEncodingOptions.Padding));
    }

    [TestMethod]
    [DataRow(DataEncodingOptions.None)]
    [DataRow(DataEncodingOptions.Padding)]
    public void Base64Url_RT_Random(DataEncodingOptions options) => TextDataEncodingTestBench.RandomRoundTrip(Base64Url.Instance, 16, 100000, options);

    [TestMethod]
    // S1
    [DataRow(TextDataEncodingTemplates.S1, DataEncodingOptions.None)]
    [DataRow(TextDataEncodingTemplates.S1, DataEncodingOptions.Padding)]
    // S2
    [DataRow(TextDataEncodingTemplates.S2, DataEncodingOptions.None)]
    [DataRow(TextDataEncodingTemplates.S2, DataEncodingOptions.Padding)]
    // S3
    [DataRow(TextDataEncodingTemplates.S3, DataEncodingOptions.None)]
    [DataRow(TextDataEncodingTemplates.S3, DataEncodingOptions.Padding)]
    public void Base64Url_RT_S(string s, DataEncodingOptions options) => TextDataEncodingTestBench.RoundTrip(Base64Url.Instance, s, options);
}
