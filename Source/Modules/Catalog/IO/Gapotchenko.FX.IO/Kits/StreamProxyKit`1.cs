// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable CA1835 // Prefer the 'Memory'-based overloads for 'ReadAsync' and 'WriteAsync'

namespace Gapotchenko.FX.IO.Kits;

/// <summary>
/// Provides a base implementation of a proxy for <see cref="Stream"/>.
/// </summary>
/// <typeparam name="T">The type of the base stream.</typeparam>
public class StreamProxyKit<T> : Stream
    where T : Stream
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProxyKit{T}"/> class with the specified base stream.
    /// </summary>
    /// <param name="baseStream">The base steam to create the proxy for.</param>
    /// <exception cref="ArgumentNullException"><paramref name="baseStream"/> is <see langword="null"/>.</exception>
    protected StreamProxyKit(T baseStream)
    {
        ArgumentNullException.ThrowIfNull(baseStream);

        BaseStream = baseStream;
    }

    #region Capabilities

    /// <inheritdoc/>
    public override bool CanRead => BaseStream.CanRead;

    /// <inheritdoc/>
    public override bool CanWrite => BaseStream.CanWrite;

    /// <inheritdoc/>
    public override bool CanSeek => BaseStream.CanSeek;

    /// <inheritdoc/>
    public override bool CanTimeout => BaseStream.CanTimeout;

    #endregion

    #region Read

    /// <inheritdoc/>
    public override int ReadTimeout
    {
        get => BaseStream.ReadTimeout;
        set => BaseStream.ReadTimeout = value;
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);

    /// <inheritdoc/>
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        BaseStream.BeginRead(buffer, offset, count, callback!, state);

    /// <inheritdoc/>
    public override int EndRead(IAsyncResult asyncResult) => BaseStream.EndRead(asyncResult);

    /// <inheritdoc/>
    public override int ReadByte() => BaseStream.ReadByte();

    /// <inheritdoc/>
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        BaseStream.ReadAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc/>
    public override int Read(Span<byte> buffer) => BaseStream.Read(buffer);

    /// <inheritdoc/>
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        base.ReadAsync(buffer, cancellationToken);
#endif

    #endregion

    #region Write

    /// <inheritdoc/>
    public override int WriteTimeout
    {
        get => BaseStream.WriteTimeout;
        set => BaseStream.WriteTimeout = value;
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);

    /// <inheritdoc/>
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        BaseStream.BeginWrite(buffer, offset, count, callback!, state);

    /// <inheritdoc/>
    public override void EndWrite(IAsyncResult asyncResult) => BaseStream.EndWrite(asyncResult);

    /// <inheritdoc/>
    public override void WriteByte(byte value) => BaseStream.WriteByte(value);

    /// <inheritdoc/>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        BaseStream.WriteAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer) => BaseStream.Write(buffer);

    /// <inheritdoc/>
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        BaseStream.WriteAsync(buffer, cancellationToken);

#endif

    #endregion

    #region Seek

    /// <inheritdoc/>
    public override long Length => BaseStream.Length;

    /// <inheritdoc/>
    public override void SetLength(long value) => BaseStream.SetLength(value);

    /// <inheritdoc/>
    public override long Position
    {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

    #endregion

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc/>
    public override void CopyTo(Stream destination, int bufferSize) =>
        BaseStream.CopyTo(destination, bufferSize);
#endif

    /// <inheritdoc/>
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
        BaseStream.CopyToAsync(destination, bufferSize, cancellationToken);

    /// <inheritdoc/>
    public override void Flush() => BaseStream.Flush();

    /// <inheritdoc/>
    public override Task FlushAsync(CancellationToken cancellationToken) => BaseStream.FlushAsync(cancellationToken);

    /// <inheritdoc/>
    public override void Close() => BaseStream.Close();

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            BaseStream.Dispose();
        base.Dispose(disposing);
    }

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc/>
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public override async ValueTask DisposeAsync()
#pragma warning restore CA1816
    {
        await BaseStream.DisposeAsync().ConfigureAwait(false);
        await base.DisposeAsync().ConfigureAwait(false);
    }
#endif

    /// <summary>
    /// Gets the base stream.
    /// </summary>
    protected T BaseStream { get; }
}
