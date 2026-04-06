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
    [DataRow(0, SeekOrigin.Begin, -2, -1, 1)] // one beyond underflow boundary
    [DataRow(0, SeekOrigin.Begin, -1, -1, 1)] // exact underflow boundary
    [DataRow(0, SeekOrigin.Begin, 0, 0, 1)] // in-range position 0
    [DataRow(0, SeekOrigin.Begin, 1, 1, 2)] // in-range position 1
    [DataRow(0, SeekOrigin.Begin, 2, 2, 3)] // in-range position 2
    [DataRow(0, SeekOrigin.Begin, 3, 3, -1)] // at end
    [DataRow(0, SeekOrigin.Begin, 4, 4, -1)] // past end
    [DataRow(0, SeekOrigin.Begin, 5, 5, -1)] // one past end
    [DataRow(0, SeekOrigin.Current, -1, -1, 1)] // exact underflow boundary
    [DataRow(1, SeekOrigin.Current, -2, -1, 2)] // underflow from non-zero position
    [DataRow(0, SeekOrigin.Current, 0, 0, 1)] // no movement
    [DataRow(0, SeekOrigin.Current, 1, 1, 2)] // 1 forward from start
    [DataRow(0, SeekOrigin.Current, 2, 2, 3)] // 2 forward from start
    [DataRow(0, SeekOrigin.Current, 3, 3, -1)] // 3 forward from start
    [DataRow(0, SeekOrigin.Current, 4, 4, -1)] // 4 forward from start
    [DataRow(1, SeekOrigin.Current, -1, 0, 1)] // backward to start
    [DataRow(2, SeekOrigin.Current, -1, 1, 2)] // backward mid-stream
    [DataRow(3, SeekOrigin.Current, -1, 2, 3)] // backward from end
    [DataRow(0, SeekOrigin.End, 2, 5, -1)] // past end + 1
    [DataRow(0, SeekOrigin.End, 1, 4, -1)] // past end
    [DataRow(0, SeekOrigin.End, 0, 3, -1)] // at end
    [DataRow(0, SeekOrigin.End, -1, 2, 3)] // in-range position 2
    [DataRow(0, SeekOrigin.End, -2, 1, 2)] // in-range position 1
    [DataRow(0, SeekOrigin.End, -3, 0, 1)] // in-range position 0
    [DataRow(0, SeekOrigin.End, -4, -1, 1)] // exact underflow boundary  
    [DataRow(0, SeekOrigin.End, -5, -1, 1)] // one beyond underflow boundary 
    public void IO_Stream_Seek(long initialPosition, SeekOrigin seekOrigin, long seekOffset, long expectedPosition, int expectedByte)
    {
        var stream = CreateStream([1, 2, 3], false);

        // Initial position.
        if (initialPosition != 0)
            stream.Position = initialPosition;

        // Stream seek.
        long position;
        if (expectedPosition == -1)
        {
            position = stream.Position;
            Assert.ThrowsExactly<IOException>(() => stream.Seek(seekOffset, seekOrigin));
        }
        else
        {
            position = stream.Seek(seekOffset, seekOrigin);
            Assert.AreEqual(expectedPosition, position, "Position mismatch.");
        }
        Assert.AreEqual(position, stream.Position);

        // Validation of seek arguments.
        Assert.ThrowsExactly<ArgumentException>(() => stream.Seek(1, (SeekOrigin)100));
        Assert.AreEqual(position, stream.Position, "Wrong seek argument should not change stream position.");

        // Verify the data.
        Assert.AreEqual(expectedByte, stream.ReadByte(), "Data mismatch");
    }

    [TestMethod]
    [DataRow(-2, true, 1)] // one beyond underflow boundary
    [DataRow(-1, true, 1)] // exact underflow boundary
    [DataRow(0, false, 1)] // in-range position 0
    [DataRow(1, false, 2)] // in-range position 1
    [DataRow(2, false, 3)] // in-range position 2
    [DataRow(3, false, -1)] // at end
    [DataRow(4, false, -1)] // past end
    public void IO_Stream_Position(long position, bool underflow, int expectedByte)
    {
        var stream = CreateStream([1, 2, 3], false);

        if (underflow)
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => stream.Position = position);
        }
        else
        {
            stream.Position = position;
            Assert.AreEqual(position, stream.Position);
        }

        Assert.AreEqual(expectedByte, stream.ReadByte(), "Data mismatch");
    }
}
