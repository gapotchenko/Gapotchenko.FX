using System;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Computes CRC-16 checksum for the input data.
    /// </summary>
    [CLSCompliant(false)]
    public class Crc16 : ChecksumAlgorithm<ushort>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16"/> class as <see cref="Crc16Kind.Standard"/> algorithm.
        /// </summary>
        public Crc16() :
            this(Crc16Kind.Standard)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16"/> class with a specified algorithm kind.
        /// </summary>
        /// <param name="kind">The CRC-16 algorithm kind.</param>
        public Crc16(Crc16Kind kind)
        {
            if (kind != Crc16Kind.Standard)
                throw new ArgumentOutOfRangeException(nameof(kind), "Unrecognized CRC-16 algorithm kind.");

            HashSizeValue = 16;
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc16"/> class with custom algorithm parameters.
        /// </summary>
        /// <param name="poly">The polynomial.</param>
        /// <param name="init">The initial value.</param>
        /// <param name="refIn"></param>
        /// <param name="refOut"></param>
        /// <param name="xorOut"></param>
        [CLSCompliant(false)]
        public Crc16(ushort poly, ushort init, bool refIn, bool refOut, ushort xorOut)
        {
            throw new NotImplementedException("TODO");
        }

        static Crc16()
        {
            const ushort polynomial = 0xA001;

            m_CrcTable = new ushort[256];

            for (int i = 0; i < m_CrcTable.Length; ++i)
            {
                ushort value = 0;

                var temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                        value = (ushort)((value >> 1) ^ polynomial);
                    else
                        value >>= 1;
                    temp >>= 1;
                }

                m_CrcTable[i] = value;
            }
        }

        static readonly ushort[] m_CrcTable;

        ushort m_Crc;

        /// <inheritdoc/>
        public override void Initialize()
        {
            m_Crc = 0;
        }

        /// <inheritdoc/>
        protected override void ChecksumCore(ReadOnlySpan<byte> source)
        {
            foreach (var b in source)
            {
                byte x = (byte)(m_Crc ^ b);
                m_Crc = (ushort)((m_Crc >> 8) ^ m_CrcTable[x]);
            }
        }

        /// <inheritdoc/>
        protected override ushort ChecksumFinal() => m_Crc;

        /// <inheritdoc/>
        protected override byte[] ChecksumHash(ushort checksum) => LittleEndianBitConverter.GetBytes(checksum);
    }
}
