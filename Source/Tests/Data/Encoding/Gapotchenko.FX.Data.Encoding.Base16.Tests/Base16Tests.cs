using Gapotchenko.FX.Data.Encoding.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gapotchenko.FX.Data.Encoding.Tests;

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

        Assert.AreEqual(0.5, instance.Efficiency);
        Assert.AreEqual(instance.Efficiency, Base16.Efficiency);

        Assert.AreEqual(2, instance.Padding);
        Assert.IsTrue(instance.CanPad);
        Assert.IsTrue(instance.PrefersPadding);

        // -----------------------------------------------------------------

        TextDataEncodingTestBench.TestVector(instance, raw, encoded);
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

    [TestMethod]
    public void Base16_RT_Random() => TextDataEncodingTestBench.RandomRoundTrip(Base16.Instance, 16, 100000);

    [DataTestMethod]
    [DataRow(TextDataEncodingTemplates.S1)]
    [DataRow(TextDataEncodingTemplates.S2)]
    [DataRow(TextDataEncodingTemplates.S3)]
    public void Base16_RT_S(string s) => TextDataEncodingTestBench.RoundTrip(Base16.Instance, s);
}
