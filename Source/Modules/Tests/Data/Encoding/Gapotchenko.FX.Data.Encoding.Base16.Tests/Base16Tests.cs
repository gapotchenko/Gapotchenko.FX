using Gapotchenko.FX.Data.Encoding.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        actualDecoded = Base16.GetBytes(actualEncoded, DataEncodingOptions.Pure);
        Assert.IsTrue(rawBytes.SequenceEqual(actualDecoded));

        // -----------------------------------------------------------------

        var instance = Base16.Instance;

        Assert.AreEqual(0.5, instance.Efficiency);
        Assert.AreEqual(Base16.Efficiency, instance.Efficiency);

        Assert.AreEqual(2, instance.Padding);
        Assert.IsTrue(instance.CanPad);
        Assert.IsTrue(instance.PrefersPadding);

        // -----------------------------------------------------------------

        TextDataEncodingTestBench.TestVector(instance, raw, encoded);
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("f", "66")]
    [DataRow("fo", "666F")]
    [DataRow("foo", "666F6F")]
    [DataRow("foob", "666F6F62")]
    [DataRow("fooba", "666F6F6261")]
    [DataRow("foobar", "666F6F626172")]
    public void Base16_Rfc4648_TV(string raw, string encoded) => TestVector(raw, encoded);

    [TestMethod]
    public void Base16_RT_Random() => TextDataEncodingTestBench.RandomRoundTrip(Base16.Instance, 16, 100000);

    [TestMethod]
    [DataRow(TextDataEncodingTemplates.S1)]
    [DataRow(TextDataEncodingTemplates.S2)]
    [DataRow(TextDataEncodingTemplates.S3)]
    public void Base16_RT_S(string s) => TextDataEncodingTestBench.RoundTrip(Base16.Instance, s);
}
