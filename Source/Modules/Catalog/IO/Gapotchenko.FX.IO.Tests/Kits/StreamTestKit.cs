// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.Tests.Kits;

[TestCategory("io")]
public abstract class StreamTestKit
{
    /// <summary>
    /// Creates a <see cref="Stream"/> instance to test.
    /// </summary>
    /// <param name="content">The content of a created stream.</param>
    /// <param name="writable">Indicates whether the created stream is writeable.</param>
    /// <returns>A testable <see cref="Stream"/>.</returns>
    protected abstract Stream CreateStream(Stream? content, bool writable);

    [TestMethod]
    [DataRow(new byte[] { 1, 2, 3 }, SeekOrigin.End, -2, 1, 2)]
    public void IO_Stream_Seek(byte[] data, SeekOrigin seekOrigin, long seekOffset, long expectedOffset, int expectedByte)
    {
        var stream = CreateStream(new MemoryStream(data, false), false);
        Assert.AreEqual(expectedOffset, stream.Seek(seekOffset, seekOrigin), "Offset mismatch.");
        Assert.AreEqual(expectedByte, stream.ReadByte(), "Data mismatch");
    }
}
