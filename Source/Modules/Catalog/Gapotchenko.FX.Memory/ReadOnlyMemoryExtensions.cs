namespace Gapotchenko.FX.Memory;

/// <summary>
/// Extensions for <see cref="ReadOnlyMemory{T}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ReadOnlyMemoryExtensions
{
    /// <summary>
    /// Creates a new read-only <see cref="Stream"/> instance
    /// based on the specified memory buffer
    /// with the <see cref="Stream.CanWrite"/> property set to <see langword="false"/>.
    /// </summary>
    /// <param name="buffer">The memory buffer from which to create this stream.</param>
    /// <returns>
    /// The new read-only <see cref="Stream"/> instance
    /// based on the specified memory buffer.
    /// The returned instance implements <see cref="IHasReadOnlyMemory{T}"/> interface.
    /// </returns>
    public static Stream ToStream(this ReadOnlyMemory<byte> buffer) => new ReadOnlyMemoryBufferStream(buffer, true);
}
