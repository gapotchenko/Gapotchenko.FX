// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Microsoft.Testing.Platform.Extensions.Messages;

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

    [TestMethod]
    public void IO_Stream_Position()
    {
        var stream = CreateStream(new MemoryStream([1, 2, 3], false), false);

        // Undershot.
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => stream.Position = -1, "Undershoot is not rejected.");

        // Exact.
        SetAndVerify(0);
        SetAndVerify(1);
        SetAndVerify(2);

        // Overshot.
        SetAndVerify(3);
        SetAndVerify(4);

        void SetAndVerify(long position)
        {
            stream.Position = position;
            Assert.AreEqual(position, stream.Position);
        }
    }
}
