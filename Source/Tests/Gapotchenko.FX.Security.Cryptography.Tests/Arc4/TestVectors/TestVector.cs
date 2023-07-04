namespace Gapotchenko.FX.Security.Cryptography.Tests.Arc4.TestVectors;

/// <summary>
/// Represents a test vector for Alleged Rivest Cipher 4 (ARC4) algorithm.
/// </summary>
sealed class TestVector
{
    /// <summary>
    /// Gets or sets the key length in bits.
    /// </summary>
    public int KeyLength { get; set; }

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public byte[] Key { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Represents a chunk of data in the key stream at the given offset.
    /// </summary>
    public sealed class Chunk
    {
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the data representing an expected key stream at the given offset.
        /// </summary>
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }

    /// <summary>
    /// Gets the list of data chunks in the key stream.
    /// </summary>
    public IList<Chunk> Chunks { get; } = new List<Chunk>();
}
