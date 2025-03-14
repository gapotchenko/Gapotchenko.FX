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
        ValidateCanRead();
        return base.Read(buffer, offset, count);
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        ValidateCanRead();
        return base.BeginRead(buffer, offset, count, callback, state);
    }

    public override int ReadByte()
    {
        ValidateCanRead();
        return base.ReadByte();
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateCanRead();
        return await base.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
    }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER

    public override int Read(Span<byte> buffer)
    {
        ValidateCanRead();
        return base.Read(buffer);
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        ValidateCanRead();
        return await base.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
    }

#endif

    void ValidateCanRead()
    {
        if (!canRead)
            throw new NotSupportedException("Stream does not support reading.");
    }

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

    void EnsureCanWrite()
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

    void EnsureCanSeek()
    {
        if (!canSeek)
            throw new NotSupportedException("Stream does not support seeking.");
    }

    #endregion
}
