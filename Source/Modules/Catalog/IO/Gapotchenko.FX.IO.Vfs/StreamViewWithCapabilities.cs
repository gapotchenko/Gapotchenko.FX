// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Kits;

#pragma warning disable CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'

namespace Gapotchenko.FX.IO.Vfs;

sealed class StreamViewWithCapabilities(Stream baseStream, bool canRead, bool canWrite, bool canSeek) :
    StreamProxyKit(baseStream)
{
    #region Read

    public override bool CanRead => canRead && base.CanRead;

    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateRead();
        return base.Read(buffer, offset, count);
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        ValidateRead();
        return base.BeginRead(buffer, offset, count, callback, state);
    }

    public override int ReadByte()
    {
        ValidateRead();
        return base.ReadByte();
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateRead();
        return await base.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
    }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER

    public override int Read(Span<byte> buffer)
    {
        ValidateRead();
        return base.Read(buffer);
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        ValidateRead();
        return await base.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
    }

#endif

    void ValidateRead()
    {
        if (!canRead)
            throw new NotSupportedException("Stream does not support reading.");
    }

    #endregion

    #region Write

    public override void Write(byte[] buffer, int offset, int count)
    {
        ValidateWrite();
        base.Write(buffer, offset, count);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        ValidateWrite();
        return base.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void WriteByte(byte value)
    {
        ValidateWrite();
        base.WriteByte(value);
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateWrite();
        await base.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
    }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        ValidateWrite();
        base.Write(buffer);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        ValidateWrite();
        await base.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
    }

#endif

    void ValidateWrite()
    {
        if (!canWrite)
            throw new NotSupportedException("Stream does not support writing.");
    }

    #endregion

    #region Seek

    public override long Length
    {
        get
        {
            ValidateSeek();
            return base.Length;
        }
    }

    public override void SetLength(long value)
    {
        ValidateSeek();
        ValidateWrite();
        base.SetLength(value);
    }

    public override long Position
    {
        get
        {
            ValidateSeek();
            return base.Position;
        }
        set
        {
            ValidateSeek();
            base.Position = value;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        ValidateSeek();
        return base.Seek(offset, origin);
    }

    void ValidateSeek()
    {
        if (!canSeek)
            throw new NotSupportedException("Stream does not support seeking.");
    }

    #endregion
}
