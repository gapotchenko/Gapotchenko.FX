using System;
using System.ComponentModel;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Computes CRC-16 checksum for the input data.
    /// </summary>
    /// <remarks>
    /// Represents the base class from which implementations of CRC-16 checksum algorithm may derive.
    /// </remarks>
    [CLSCompliant(false)]
    public abstract partial class Crc16 : ICrc16
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCrc16"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value of the register when the algorithm starts.</param>
        protected Crc16(ushort initialValue)
        {
            InitialValue = initialValue;
        }

        /// <summary>
        /// The initial value of the register when the algorithm starts.
        /// </summary>
        protected readonly ushort InitialValue;

        /// <summary>
        /// The size, in bits, of the computed checksum value.
        /// </summary>
        protected const int Width = 16;

        /// <inheritdoc/>
        public int ChecksumSize => Width;

        /// <inheritdoc/>
        public virtual ushort ComputeChecksum(ReadOnlySpan<byte> data) => ComputeFinal(ComputeBlock(InitialValue, data));

        /// <summary>
        /// Creates an iterator for checksum computation.
        /// </summary>
        /// <returns>An iterator for checksum computation.</returns>
        public Iterator CreateIterator() => new(this);

        IChecksumIterator<ushort> IChecksumAlgorithm<ushort>.CreateIterator() => CreateIterator();

        /// <summary>
        /// Iterator for CRC-16 checksum computation.
        /// </summary>
        public struct Iterator : IChecksumIterator<ushort>
        {
            internal Iterator(Crc16 algorithm)
            {
                m_Algorithm = algorithm;
                m_Register = m_Algorithm.InitialValue;
            }

            readonly Crc16 m_Algorithm;
            ushort m_Register;

            /// <inheritdoc/>
            public void ComputeBlock(ReadOnlySpan<byte> data) => m_Register = m_Algorithm.ComputeBlock(m_Register, data);

            /// <inheritdoc/>
            public ushort ComputeFinal()
            {
                var checksum = m_Algorithm.ComputeFinal(m_Register);
                Reset();
                return checksum;
            }

            /// <inheritdoc/>
            public void Reset() => m_Register = m_Algorithm.InitialValue;
        }

        /// <summary>
        /// Computes the checksum for the specified byte span.
        /// </summary>
        /// <param name="register">The intermediate checksum register.</param>
        /// <param name="data">The data to compute the checksum for.</param>
        /// <returns>The updated intermediate checksum register.</returns>
        protected abstract ushort ComputeBlock(ushort register, ReadOnlySpan<byte> data);

        /// <summary>
        /// Finalizes the checksum computation after the last data is processed.
        /// </summary>
        /// <returns>The computed checksum.</returns>
        protected abstract ushort ComputeFinal(ushort register);
    }
}
