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
    /// <inheritdoc cref="CreateStream(Stream?, bool)"/>
    protected Stream CreateStream(byte[]? content, bool writable)
    {
        return CreateStream(
            content is null ? null : new MemoryStream(content, false),
            writable);
    }

    /// <summary>
    /// Creates a <see cref="Stream"/> instance to test.
    /// </summary>
    /// <param name="content">The content of a created stream.</param>
    /// <param name="writable">Indicates whether the created stream is writeable.</param>
    /// <returns>A testable <see cref="Stream"/>.</returns>
    protected abstract Stream CreateStream(Stream? content, bool writable);

    [TestMethod]
    [DataRow(new byte[] { 1, 2, 3 }, SeekOrigin.End, -2, 1, 2)]
    [DataRow(new byte[] { 1, 2, 3 }, SeekOrigin.Current, -1, -1, 1)]
    public void IO_Stream_Seek(byte[]? data, SeekOrigin seekOrigin, long seekOffset, long expectedPosition, int expectedByte)
    {
        var stream = CreateStream(data, false);

        if (expectedPosition == -1)
        {
            long originalPosition = stream.Position;
            Assert.ThrowsExactly<IOException>(() => stream.Seek(seekOffset, seekOrigin));
            Assert.AreEqual(originalPosition, stream.Position);
        }
        else
        {
            Assert.AreEqual(expectedPosition, stream.Seek(seekOffset, seekOrigin), "Position mismatch.");
        }

        Assert.ThrowsExactly<ArgumentException>(() => stream.Seek(1, (SeekOrigin)100));

        Assert.AreEqual(expectedByte, stream.ReadByte(), "Data mismatch");
    }

    [TestMethod]
    public void IO_Stream_Position()
    {
        var stream = CreateStream([1, 2, 3], false);

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
