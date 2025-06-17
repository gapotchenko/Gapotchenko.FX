// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Diagnostics;

#pragma warning disable IDE0032 // Use auto property

namespace Gapotchenko.FX.IO;

/// <summary>
/// Represents a reader that can track the current position within a sequential series of characters.
/// </summary>
public sealed class PositionTrackingTextReader : TextReader
{
    /// <summary>
    /// Initialize a new instance of the <see cref="PositionTrackingTextReader"/> class for the specified text reader.
    /// </summary>
    /// <param name="reader">The underlying text reader.</param>
    /// <exception cref="ArgumentNullException"><paramref name="reader"/> is <see langword="null"/>.</exception>
    public PositionTrackingTextReader(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        m_BaseReader = reader;
    }

    /// <inheritdoc/>
    public override int Read()
    {
        int result = m_BaseReader.Read();
        if (result != -1)
            AdvancePosition(1);
        return result;
    }

    /// <inheritdoc/>
    public override int Read(char[] buffer, int index, int count)
    {
        int charsRead = m_BaseReader.Read(buffer, index, count);
        if (charsRead > 0)
            AdvancePosition(charsRead);
        return charsRead;
    }

    /// <inheritdoc/>
    public override async Task<int> ReadAsync(char[] buffer, int index, int count)
    {
        int charsRead = await m_BaseReader.ReadAsync(buffer, index, count).ConfigureAwait(false);
        if (charsRead > 0)
            AdvancePosition(charsRead);
        return charsRead;
    }

    /// <inheritdoc/>
    public override int ReadBlock(char[] buffer, int index, int count)
    {
        int charsRead = m_BaseReader.ReadBlock(buffer, index, count);
        AdvancePosition(charsRead);
        return charsRead;
    }

    /// <inheritdoc/>
    public override async Task<int> ReadBlockAsync(char[] buffer, int index, int count)
    {
        int charsRead = await m_BaseReader.ReadBlockAsync(buffer, index, count).ConfigureAwait(false);
        AdvancePosition(charsRead);
        return charsRead;
    }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc/>
    public override int Read(Span<char> buffer)
    {
        int charsRead = m_BaseReader.Read(buffer);
        if (charsRead > 0)
            AdvancePosition(charsRead);
        return charsRead;
    }

    /// <inheritdoc/>
    public override async ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
    {
        int charsRead = await m_BaseReader.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        if (charsRead > 0)
            AdvancePosition(charsRead);
        return charsRead;
    }

    /// <inheritdoc/>
    public override int ReadBlock(Span<char> buffer)
    {
        int charsRead = m_BaseReader.ReadBlock(buffer);
        AdvancePosition(charsRead);
        return charsRead;
    }

    /// <inheritdoc/>
    public override async ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
    {
        int charsRead = await m_BaseReader.ReadBlockAsync(buffer, cancellationToken).ConfigureAwait(false);
        AdvancePosition(charsRead);
        return charsRead;
    }
#endif

    void AdvancePosition(int n) => m_Position += n;

    /// <summary>
    /// Gets a position within a sequential series of characters.
    /// </summary>
    public long Position => m_Position;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    long m_Position;

    /// <inheritdoc/>
    public override void Close() => m_BaseReader.Close();

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            m_BaseReader.Dispose();
        base.Dispose(disposing);
    }

    /// <summary>
    /// Returns the underlying text reader.
    /// </summary>
    public TextReader BaseReader => m_BaseReader;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly TextReader m_BaseReader;
}
