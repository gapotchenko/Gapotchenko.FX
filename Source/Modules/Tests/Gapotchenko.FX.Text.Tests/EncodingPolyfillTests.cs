// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Text;

namespace Gapotchenko.FX.Text.Tests;

[TestClass]
public sealed class EncodingPolyfillTests
{
    [TestMethod]
    public void Encoding_GetString_Span_Empty()
    {
        Assert.AreEqual(
            string.Empty,
            Encoding.UTF8.GetString(ReadOnlySpan<byte>.Empty));
    }

    [TestMethod]
    public void Encoding_GetString_Span_Utf8()
    {
        const string expected = "Zażółć gęślą 🦊";
        byte[] bytes = Encoding.UTF8.GetBytes(expected);

        Assert.AreEqual(expected, Encoding.UTF8.GetString(bytes.AsSpan()));
    }

    [TestMethod]
    public void Encoding_GetString_Span_Sliced()
    {
        const string expected = "encoded slice";
        byte[] encoded = Encoding.Unicode.GetBytes(expected);
        byte[] buffer = new byte[encoded.Length + 4];
        buffer[0] = 0xff;
        buffer[1] = 0xff;
        encoded.CopyTo(buffer, 2);
        buffer[^2] = 0xff;
        buffer[^1] = 0xff;

        Assert.AreEqual(
            expected,
            Encoding.Unicode.GetString(buffer.AsSpan(2, encoded.Length)));
    }

    [TestMethod]
    public void Encoding_GetString_Span_InvalidBytes_ReplacementFallback()
    {
        byte[] bytes = [0x61, 0xff, 0x62];

        Assert.AreEqual("a\ufffdb", Encoding.UTF8.GetString(bytes.AsSpan()));
    }

    [TestMethod]
    public void Encoding_GetString_Span_InvalidBytes_ExceptionFallback()
    {
        var encoding = new UTF8Encoding(false, true);
        byte[] bytes = [0x61, 0xff, 0x62];

        Assert.ThrowsExactly<DecoderFallbackException>(() => encoding.GetString(bytes.AsSpan()));
    }
}
