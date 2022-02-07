using Gapotchenko.FX.Numerics;
using System;
using System.ComponentModel;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Provides a generic implementation of CRC-16 checksum algorithm.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CLSCompliant(false)]
    public abstract class GenericCrc16 : Crc16
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCrc16"/> class with specified CRC-16 algorithm parameters.
        /// </summary>
        /// <param name="polynomial">The polynomial used for the CRC-16 calculation, omitting the top bit.</param>
        /// <param name="initialValue">The initial value of the register when the algorithm starts.</param>
        /// <param name="reflectedInput">A value indicating whether to treat bit 7 of input bytes as the least significant bit (LSB).</param>
        /// <param name="reflectedOutput">A value indicating whether to reflect the final register value before feeding it into the XORout stage.</param>
        /// <param name="xorOutput">The value which is XORed with the final register value to form an official checksum value.</param>
        private protected GenericCrc16(
            ushort polynomial,
            ushort initialValue,
            bool reflectedInput,
            bool reflectedOutput,
            ushort xorOutput)
            : base(
                reflectedInput ?
                    BitOperationsEx.Reverse(initialValue) :
                    initialValue)
        {
            m_Table = Crc16TableFactory.GetTable(polynomial, reflectedInput);
            m_XorOutput = xorOutput;
        }

        readonly ushort[] m_Table;
        readonly ushort m_XorOutput;

        /// <inheritdoc/>
        protected sealed override ushort ComputeBlock(ushort register, ReadOnlySpan<byte> data)
        {
            foreach (var b in data)
                register = (ushort)((register >> 8) ^ m_Table[(byte)(register ^ b)]);
            return register;
        }

        /// <inheritdoc/>
        protected sealed override ushort ComputeFinal(ushort register) => (ushort)(register ^ m_XorOutput);
    }
}
