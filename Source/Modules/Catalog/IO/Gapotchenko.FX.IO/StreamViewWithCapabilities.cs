// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Kits;

#pragma warning disable CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'

namespace Gapotchenko.FX.IO;

sealed class StreamViewWithCapabilities(Stream baseStream, bool canRead, bool canWrite, bool canSeek) :
    StreamProxyKit(baseStream)
{
    #region Capabilities

    public override bool CanRead => canRead && base.CanRead;

    public override bool CanWrite => canWrite && base.CanWrite;

    public override bool CanSeek => canSeek && base.CanSeek;

    void EnsureCanRead()
    {
        if (!canRead)
            throw new NotSupportedException("Stream does not support reading.");
    }

    void EnsureCanWrite()
    {
        if (!canWrite)
            throw new NotSupportedException("Stream does not support writing.");
    }

    void EnsureCanSeek()
    {
        if (!canSeek)
            throw new NotSupportedException("Stream does not support seeking.");
    }

    #endregion

    #region Read

    public override int Read(byte[] buffer, int offset, int count)
    {
        EnsureCanRead();
        return base.Read(buffer, offset, count);
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        EnsureCanRead();
        return base.BeginRead(buffer, offset, count, callback, state);
    }

    public override int ReadByte()
    {
        EnsureCanRead();
        return base.ReadByte();
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        EnsureCanRead();
        return await base.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
    }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER

    public override int Read(Span<byte> buffer)
    {
        EnsureCanRead();
        return base.Read(buffer);
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        EnsureCanRead();
        return await base.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
    }

#endif

    #endregion

    #region Write

    public override void Write(byte[] buffer, int offset, int count)
    {
        EnsureCanWrite();
        base.Write(buffer, offset, count);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        EnsureCanWrite();
        return base.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void WriteByte(byte value)
    {
        EnsureCanWrite();
        base.WriteByte(value);
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        EnsureCanWrite();
        await base.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
    }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        EnsureCanWrite();
        base.Write(buffer);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        EnsureCanWrite();
        await base.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
    }

#endif

    #endregion

    #region Seek

    public override long Length
    {
        get
        {
            EnsureCanSeek();
            return base.Length;
        }
    }

    public override void SetLength(long value)
    {
        EnsureCanSeek();
        EnsureCanWrite();

        base.SetLength(value);
    }

    public override long Position
    {
        get
        {
            EnsureCanSeek();
            return base.Position;
        }
        set
        {
            EnsureCanSeek();
            base.Position = value;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        EnsureCanSeek();
        return base.Seek(offset, origin);
    }

    #endregion
}
