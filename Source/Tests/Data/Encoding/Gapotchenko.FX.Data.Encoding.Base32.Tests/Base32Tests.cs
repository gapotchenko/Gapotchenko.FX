using Gapotchenko.FX.Data.Encoding.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gapotchenko.FX.Data.Encoding.Tests;

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

        Assert.AreEqual(Base32.Efficiency, instance.Efficiency);
        Assert.AreEqual(Base32.Padding, instance.Padding);
        Assert.IsTrue(instance.PrefersPadding);

        // -----------------------------------------------------------------

        TextDataEncodingTestBench.TestVector(instance, raw, encoded);
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

    [DataTestMethod]
    [DataRow(DataEncodingOptions.None)]
    [DataRow(DataEncodingOptions.NoPadding)]
    public void Base32_RT_Random(DataEncodingOptions options) => TextDataEncodingTestBench.RandomRoundTrip(Base32.Instance, 16, 100000, options);

    [DataTestMethod]
    [DataRow(TextDataEncodingTemplates.S1)]
    [DataRow(TextDataEncodingTemplates.S2)]
    [DataRow(TextDataEncodingTemplates.S3)]
    public void Base32_RT_S(string s) => TextDataEncodingTestBench.RoundTrip(Base32.Instance, s);
}
