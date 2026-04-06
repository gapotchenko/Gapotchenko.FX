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
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Begin, -2, -1, 1)] // one beyond underflow boundary
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Begin, -1, -1, 1)] // exact underflow boundary
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Begin, 0, 0, 1)] // in-range position 0
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Begin, 1, 1, 2)] // in-range position 1
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Begin, 2, 2, 3)] // in-range position 2
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Begin, 3, 3, -1)] // at end
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Begin, 4, 4, -1)] // past end
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Begin, 5, 5, -1)] // one past end
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Current, -1, -1, 1)] // exact underflow boundary
    [DataRow(new byte[] { 1, 2, 3 }, 1, SeekOrigin.Current, -2, -1, 2)] // underflow from non-zero position
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Current, 0, 0, 1)] // no movement
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Current, 1, 1, 2)] // 1 forward from start
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Current, 2, 2, 3)] // 2 forward from start
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Current, 3, 3, -1)] // 3 forward from start
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.Current, 4, 4, -1)] // 4 forward from start
    [DataRow(new byte[] { 1, 2, 3 }, 1, SeekOrigin.Current, -1, 0, 1)] // backward to start
    [DataRow(new byte[] { 1, 2, 3 }, 2, SeekOrigin.Current, -1, 1, 2)] // backward mid-stream
    [DataRow(new byte[] { 1, 2, 3 }, 3, SeekOrigin.Current, -1, 2, 3)] // backward from end
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.End, 2, 5, -1)] // past end + 1
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.End, 1, 4, -1)] // past end
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.End, 0, 3, -1)] // at end
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.End, -1, 2, 3)] // in-range position 2
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.End, -2, 1, 2)] // in-range position 1
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.End, -3, 0, 1)] // in-range position 0
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.End, -4, -1, 1)] // exact underflow boundary  
    [DataRow(new byte[] { 1, 2, 3 }, 0, SeekOrigin.End, -5, -1, 1)] // one beyond underflow boundary 
    public void IO_Stream_Seek(byte[]? data, long initialPosition, SeekOrigin seekOrigin, long seekOffset, long expectedPosition, int expectedByte)
    {
        var stream = CreateStream(data, false);

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
    public void IO_Stream_Position()
    {
        var stream = CreateStream([1, 2, 3], false);

        // Underflow
        foreach (long position in (ReadOnlySpan<long>)[-1, -2])
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => stream.Position = position, "Underflow is not rejected.");

        // In-range
        foreach (long position in (ReadOnlySpan<long>)[0, 1, 2])
            SetAndVerify(position);

        // Overflow
        foreach (long position in (ReadOnlySpan<long>)[3, 4])
            SetAndVerify(position);

        void SetAndVerify(long position)
        {
            stream.Position = position;
            Assert.AreEqual(position, stream.Position);
        }
    }
}
