using System;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Integrity.Checksum
{
    /// <summary>
    /// Computes CRC-32 checksum for the input data.
    /// </summary>
    /// <remarks>
    /// Represents the base class from which implementations of CRC-32 checksum algorithm may derive.
    /// </remarks>
    [CLSCompliant(false)]
    public abstract partial class Crc32 : ChecksumAlgorithm<uint>, ICrc32
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Crc32"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value of the register when the algorithm starts.</param>
        protected Crc32(uint initialValue)
        {
            InitialValue = initialValue;
        }

        /// <summary>
        /// The initial value of the register when the algorithm starts.
        /// </summary>
        protected readonly uint InitialValue;

        /// <summary>
        /// The size, in bits, of the computed checksum value.
        /// </summary>
        protected const int Width = 32;

        /// <inheritdoc/>
        public sealed override int ChecksumSize => Width;

        /// <inheritdoc/>
        public override uint ComputeChecksum(ReadOnlySpan<byte> data) => ComputeFinal(ComputeBlock(InitialValue, data));

        /// <inheritdoc/>
        public HashAlgorithm CreateHashAlgorithm(IBitConverter bitConverter) =>
            CreateHashAlgorithmCore(bitConverter ?? throw new ArgumentNullException(nameof(bitConverter)));

        /// <inheritdoc/>
        protected override byte[] GetHashBytesCore(uint checksum, IBitConverter bitConverter) => bitConverter.GetBytes(checksum);

        /// <summary>
        /// Creates an iterator for checksum computation.
        /// </summary>
        /// <returns>An iterator for checksum computation.</returns>
        public new Iterator CreateIterator() => new(this);

        /// <inheritdoc/>
        protected sealed override IChecksumIterator<uint> CreateIteratorCore() => CreateIterator();

        /// <summary>
        /// Iterator for CRC-32 checksum computation.
        /// </summary>
        public struct Iterator : IChecksumIterator<uint>
        {
            internal Iterator(Crc32 algorithm)
            {
                m_Algorithm = algorithm;
                m_Register = m_Algorithm.InitialValue;
            }

            readonly Crc32 m_Algorithm;
            uint m_Register;

            /// <inheritdoc/>
            public void ComputeBlock(ReadOnlySpan<byte> data) => m_Register = m_Algorithm.ComputeBlock(m_Register, data);

            /// <inheritdoc/>
            public uint ComputeFinal()
            {
                var checksum = m_Algorithm.ComputeFinal(m_Register);
                Reset();
                return checksum;
            }

            object IChecksumIterator.ComputeFinal() => ComputeFinal();

            /// <inheritdoc/>
            public void Reset() => m_Register = m_Algorithm.InitialValue;
        }

        /// <summary>
        /// Computes the checksum for the specified byte span.
        /// </summary>
        /// <param name="register">The intermediate checksum register.</param>
        /// <param name="data">The data to compute the checksum for.</param>
        /// <returns>The updated intermediate checksum register.</returns>
        protected abstract uint ComputeBlock(uint register, ReadOnlySpan<byte> data);

        /// <summary>
        /// Finalizes the checksum computation after the last data is processed.
        /// </summary>
        /// <returns>The computed checksum.</returns>
        protected abstract uint ComputeFinal(uint register);
    }
}
