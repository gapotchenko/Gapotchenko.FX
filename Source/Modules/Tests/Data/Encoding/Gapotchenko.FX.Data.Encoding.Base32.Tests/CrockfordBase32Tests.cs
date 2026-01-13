using Gapotchenko.FX.Data.Encoding.Tests.Bench;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Numerics;

namespace Gapotchenko.FX.Data.Encoding.Tests;

using Encoding = System.Text.Encoding;

[TestClass]
public class CrockfordBase32Tests
{
    #region Data

    static void TestVector(byte[] raw, string encoded, DataEncodingOptions options = default)
    {
        string actualEncoded = CrockfordBase32.GetString(raw, options);
        Assert.AreEqual(encoded, actualEncoded);

        var actualDecoded = CrockfordBase32.GetBytes(encoded, options);
        Assert.IsTrue(raw.SequenceEqual(actualDecoded));

        var instance = CrockfordBase32.Instance;
        Assert.AreEqual(CrockfordBase32.Efficiency, instance.Efficiency);

        TextDataEncodingTestBench.TestVector(instance, raw, encoded, options);
    }

    static void TestVector(string raw, string encoded) => TestVector(Encoding.UTF8.GetBytes(raw), encoded);

    [TestMethod]
    public void CrockfordBase32_Data_Empty() => TestVector("", "");

    [TestMethod]
    [DataRow("0Oo", "000")]
    [DataRow("1Ll", "111")]
    [DataRow("1Ii", "111")]
    [DataRow("Loi", "101")]
    public void CrockfordBase32_Canonicalization(string from, string to) =>
        Assert.AreEqual(
            to,
            CrockfordBase32.Instance.Canonicalize(from.AsSpan()));

    [TestMethod]
    public void CrockfordBase32_Data_TV1() => TestVector("f", "CR");

    [TestMethod]
    public void CrockfordBase32_Data_TV2() => TestVector("fo", "CSQG");

    [TestMethod]
    public void CrockfordBase32_Data_TV3() => TestVector("foo", "CSQPY");

    [TestMethod]
    public void CrockfordBase32_Data_TV4() => TestVector("foob", "CSQPYRG");

    [TestMethod]
    public void CrockfordBase32_Data_TV5() => TestVector("fooba", "CSQPYRK1");

    [TestMethod]
    public void CrockfordBase32_Data_TV6() => TestVector("foobar", "CSQPYRK1E8");

    [TestMethod]
    public void CrockfordBase32_Data_TV7() =>
        Assert.AreEqual(
            "foobar",
            Encoding.UTF8.GetString(CrockfordBase32.GetBytes("CsQP-YRkL-E8")));

    [TestMethod]
    [DataRow(DataEncodingOptions.None)]
    [DataRow(DataEncodingOptions.Padding)]
    public void CrockfordBase32_Data_RT_Random(DataEncodingOptions options) => TextDataEncodingTestBench.RandomRoundTrip(CrockfordBase32.Instance, 16, 100000, options);

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
    public void CrockfordBase32_Data_RT_S(string s, DataEncodingOptions options) => TextDataEncodingTestBench.RoundTrip(CrockfordBase32.Instance, s, options);

    #endregion

    #region Int32

    static void TestVector(int raw, string encoded, DataEncodingOptions options = default)
    {
        var actualDecoded = CrockfordBase32.GetInt32(encoded, options);
        Assert.AreEqual(raw, actualDecoded);

        string actualEncoded = CrockfordBase32.GetString(raw, options);
        Assert.AreEqual(encoded, actualEncoded);

        TextDataEncodingTestBench.TestVector(CrockfordBase32.Instance, raw, encoded, options);
    }

    [TestMethod]
    public void CrockfordBase32_Int32_TV1() => TestVector(1337, "19S");

    [TestMethod]
    public void CrockfordBase32_Int32_TV2() => TestVector(1234, "16J");

    [TestMethod]
    public void CrockfordBase32_Int32_TV3() => TestVector(5111, "4ZQ");

    [TestMethod]
    public void CrockfordBase32_Int32_TV4() => TestVector(0, "0");

    [TestMethod]
    public void CrockfordBase32_Int32_TV5() => TestVector(32, "10*", DataEncodingOptions.Checksum);

    [TestMethod]
    public void CrockfordBase32_Int32_TV6() => Assert.ThrowsExactly<FormatException>(() => TestVector(32, "10~", DataEncodingOptions.Checksum));

    [TestMethod]
    public void CrockfordBase32_Int32_TV7() => TestVector(1234, "16JD", DataEncodingOptions.Checksum);

    [TestMethod]
    public void CrockfordBase32_Int32_TV8() => TestVector(0, "00", DataEncodingOptions.Checksum);

    [TestMethod]
    [DataRow(DataEncodingOptions.None)]
    [DataRow(DataEncodingOptions.Checksum)]
    public void CrockfordBase32_Int32_RT_Random(DataEncodingOptions options) =>
        TextDataEncodingTestBench.RandomRoundTrip(CrockfordBase32.Instance, 0, int.MaxValue, 100000, options);

    #endregion

    #region UInt32

    [TestMethod]
    [DataRow(DataEncodingOptions.None)]
    [DataRow(DataEncodingOptions.Checksum)]
    public void CrockfordBase32_UInt32_RT_Random(DataEncodingOptions options) =>
        TextDataEncodingTestBench.RandomRoundTrip(CrockfordBase32.Instance, 0, uint.MaxValue, 100000, options);

    #endregion

    #region Int64

    static void TestVector(long raw, string encoded, DataEncodingOptions options = default)
    {
        var actualDecoded = CrockfordBase32.GetInt64(encoded, options);
        Assert.AreEqual(raw, actualDecoded);

        string actualEncoded = CrockfordBase32.GetString(raw, options);
        Assert.AreEqual(encoded, actualEncoded);

        TextDataEncodingTestBench.TestVector(CrockfordBase32.Instance, raw, encoded, options);
    }

    [TestMethod]
    public void CrockfordBase32_Int64_TV1() => TestVector(1337L, "19S");

    [TestMethod]
    public void CrockfordBase32_Int64_TV2() => TestVector(1234L, "16J");

    [TestMethod]
    public void CrockfordBase32_Int64_TV3() => TestVector(5111L, "4ZQ");

    [TestMethod]
    public void CrockfordBase32_Int64_TV4() => TestVector(32L, "10*", DataEncodingOptions.Checksum);

    [TestMethod]
    public void CrockfordBase32_Int64_TV5() => Assert.ThrowsExactly<FormatException>(() => TestVector(32L, "10~", DataEncodingOptions.Checksum));

    [TestMethod]
    public void CrockfordBase32_Int64_TV6() => TestVector(1234L, "16JD", DataEncodingOptions.Checksum);

    [TestMethod]
    public void CrockfordBase32_Int64_TV7() => TestVector(0L, "00", DataEncodingOptions.Checksum);

    [TestMethod]
    [DataRow(DataEncodingOptions.None)]
    [DataRow(DataEncodingOptions.Checksum)]
    public void CrockfordBase32_Int64_RT_Random(DataEncodingOptions options) =>
        TextDataEncodingTestBench.RandomRoundTrip(CrockfordBase32.Instance, 0, Int64.MaxValue, 100000, options);

    #endregion

    #region UInt64

    [TestMethod]
    [DataRow(DataEncodingOptions.None)]
    [DataRow(DataEncodingOptions.Checksum)]
    public void CrockfordBase32_UInt64_RT_Random(DataEncodingOptions options) =>
        TextDataEncodingTestBench.RandomRoundTrip(CrockfordBase32.Instance, 0, UInt64.MaxValue, 100000, options);

    #endregion

    #region BigInteger

    static void TestVector(BigInteger raw, string encoded, DataEncodingOptions options = default)
    {
        var actualDecoded = CrockfordBase32.GetBigInteger(encoded, options);
        Assert.AreEqual(raw, actualDecoded);

        string actualEncoded = CrockfordBase32.GetString(raw, options);
        Assert.AreEqual(encoded, actualEncoded);

        TextDataEncodingTestBench.TestVector(CrockfordBase32.Instance, raw, encoded, options);
    }

    [TestMethod]
    public void CrockfordBase32_BigInteger_TV1() => TestVector(BigInteger.Zero, "0");

    [TestMethod]
    public void CrockfordBase32_BigInteger_TV2() => TestVector(BigInteger.Parse("1234"), "16J");

    [TestMethod]
    public void CrockfordBase32_BigInteger_TV3() => TestVector(BigInteger.Parse("3019140802085400304608040952"), "2E1BZQDAGC4G6TTENZR");

    [TestMethod]
    public void CrockfordBase32_BigInteger_TV4() => TestVector(BigInteger.Parse("100000000000000000000"), "2PQHTY5NHH0000");

    [TestMethod]
    public void CrockfordBase32_BigInteger_TV5() => TestVector(BigInteger.Zero, "00", DataEncodingOptions.Checksum);

    [TestMethod]
    public void CrockfordBase32_BigInteger_TV6() => TestVector(BigInteger.Parse("1234"), "16JD", DataEncodingOptions.Checksum);

    [TestMethod]
    public void CrockfordBase32_BigInteger_RT1() =>
        TextDataEncodingTestBench.RoundTrip(
            CrockfordBase32.Instance,
            BigInteger.Parse("44473186245345"),
            DataEncodingOptions.Checksum);

    [TestMethod]
    [DataRow(DataEncodingOptions.None)]
    [DataRow(DataEncodingOptions.Checksum)]
    public void CrockfordBase32_BigInteger_RT_Random(DataEncodingOptions options) =>
        TextDataEncodingTestBench.RandomRoundTrip(
            CrockfordBase32.Instance,
            BigInteger.Zero,
            BigInteger.Parse("0fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff", NumberStyles.HexNumber),
            50000,
            options);

    #endregion
}
