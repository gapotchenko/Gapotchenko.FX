using System;
using System.ComponentModel;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Computes CRC-16 checksum for the input data.
    /// </summary>
    /// <remarks>
    /// Represents the base class from which all implementations of CRC-16 checksum algorithm must derive.
    /// </remarks>
    [CLSCompliant(false)]
    public abstract class Crc16 : ICrc16
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCrc16"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value of the register when the algorithm starts.</param>
        protected Crc16(ushort initialValue)
        {
            InitialValue = initialValue;
        }

        readonly ushort InitialValue;

        /// <inheritdoc/>
        public int ChecksumSize => 16;

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

        /// <summary>
        /// <para>
        /// Gets the standard CRC-16 algorithm
        /// which performs checksum computation using x^16 + x^15 + x^2 + 1 polynomial with initial value of 0.
        /// </para>
        /// <para>
        /// Aliases: CRC-16, CRC-16/ARC, CRC-IBM, CRC-16/LHA.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x8005, init=0x0000, refin=true, refout=true, xorout=0x0000, check=0xbb3d.
        /// </para>
        /// </remarks>
        public static Crc16 Standard => Implementations.Standard.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-16/CCITT algorithm
        /// which performs checksum computation using x^16 + x^12 + x^8 + 1 polynomial with initial value of 0.
        /// </para>
        /// <para>
        /// Aliases: CRC-16/KERMIT, CRC-16/CCITT-TRUE, CRC-16/V-41-LSB, CRC-CCITT, KERMIT.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x1021, init=0x0000, refin=true, refout=true, xorout=0x0000, check=0x2189.
        /// </para>
        /// </remarks>
        public static Crc16 Ccitt => Implementations.Ccitt.Instance;

        /// <summary>
        /// <para>
        /// Gets CRC-16/ISO-IEC-14443-3-A algorithm
        /// which performs checksum computation using x^16 + x^12 + x^8 + 1 polynomial with initial value of 0xC6C6.
        /// </para>
        /// <para>
        /// Alias: CRC-A.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Parameters: poly=0x1021, init=0xc6c6, refin=true, refout=true, xorout=0x0000, check=0xbf05
        /// </para>
        /// </remarks>
        public static Crc16 IsoIec14443_3_A => Implementations.IsoIec14443_3_A.Instance;

        static class Implementations
        {
            public sealed class Standard : GenericCrc16
            {
                Standard() : base(0x8005, 0, true, true, 0) { }
                public static readonly Standard Instance = new();
            }

            public sealed class Ccitt : GenericCrc16
            {
                Ccitt() : base(0x1021, 0, true, true, 0) { }
                public static readonly Ccitt Instance = new();
            }

            public sealed class IsoIec14443_3_A : GenericCrc16
            {
                IsoIec14443_3_A() : base(0x1021, 0xc6c6, true, true, 0) { }
                public static readonly IsoIec14443_3_A Instance = new();
            }
        }
    }
}
