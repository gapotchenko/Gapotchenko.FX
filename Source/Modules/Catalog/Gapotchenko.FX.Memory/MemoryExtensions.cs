namespace Gapotchenko.FX.Memory;

/// <summary>
/// Extensions for <see cref="Memory{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class MemoryExtensions
{
    // The ToStream(...) methods design is based on https://github.com/dotnet/runtime/issues/22838.

    /// <summary>
    /// Creates a new non-resizable <see cref="Stream"/> instance
    /// based on the specified memory buffer
    /// with the <see cref="Stream.CanWrite"/> property set to <see langword="true"/>.
    /// </summary>
    /// <param name="buffer">The memory buffer from which to create this stream.</param>
    /// <returns>
    /// The new non-resizable <see cref="Stream"/> instance
    /// based on the specified memory buffer.
    /// The returned instance implements <see cref="IHasMemory{T}"/> and <see cref="IHasReadOnlyMemory{T}"/> interfaces.
    /// </returns>
    public static Stream ToStream(this Memory<byte> buffer) => buffer.ToStream(true);

    /// <summary>
    /// Creates a new non-resizable instance of the <see cref="MemoryBufferStream"/> class
    /// based on the specified memory buffer
    /// with the <see cref="Stream.CanWrite"/> property set as specified.
    /// </summary>
    /// <param name="buffer">The memory buffer from which to create this stream.</param>
    /// <param name="writeable">
    /// The settings of the <see cref="Stream.CanWrite"/> property, 
    /// which determines whether the stream supports writing.
    /// </param>
    /// <returns>
    /// The new non-resizable <see cref="Stream"/> instance
    /// based on the specified memory buffer.
    /// The returned instance implements <see cref="IHasMemory{T}"/> and <see cref="IHasReadOnlyMemory{T}"/> interfaces.
    /// </returns>
    public static Stream ToStream(this Memory<byte> buffer, bool writeable) => new MemoryBufferStream(buffer, writeable, true);
}
