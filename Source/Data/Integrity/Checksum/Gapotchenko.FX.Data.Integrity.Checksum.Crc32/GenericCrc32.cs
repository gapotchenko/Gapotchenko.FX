using Gapotchenko.FX.Numerics;
using System;
using System.ComponentModel;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Provides a generic implementation of CRC-32 checksum algorithm.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[CLSCompliant(false)]
public abstract class GenericCrc32 : Crc32
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCrc32"/> class with specified CRC-32 algorithm parameters.
    /// </summary>
    /// <param name="polynomial">The polynomial used for the CRC-32 calculation, omitting the top bit.</param>
    /// <param name="initialValue">The initial value of the register when the algorithm starts.</param>
    /// <param name="reflectedInput">A value indicating whether to treat bit 7 of input bytes as the least significant bit (LSB).</param>
    /// <param name="reflectedOutput">A value indicating whether to reflect the final register value before feeding it into the XORout stage.</param>
    /// <param name="xorOutput">The value which is XORed with the final register value to form an official checksum value.</param>
    private protected GenericCrc32(
        uint polynomial,
        uint initialValue,
        bool reflectedInput,
        bool reflectedOutput,
        uint xorOutput)
        : base(
            reflectedOutput ?
                BitOperationsEx.Reverse(initialValue) :
                initialValue)
    {
        m_ReflectedOutput = reflectedOutput;
        m_XorOutput = xorOutput;
        m_Table = Crc32TableFactory.GetTable(polynomial, reflectedInput);
    }

    readonly bool m_ReflectedOutput;
    readonly uint m_XorOutput;
    readonly uint[] m_Table;

    /// <inheritdoc/>
    protected sealed override uint ComputeBlock(uint register, ReadOnlySpan<byte> data)
    {
        if (m_ReflectedOutput)
        {
            foreach (var b in data)
                register = (register >> 8) ^ m_Table[(byte)(register ^ b)];
        }
        else
        {
            foreach (var b in data)
                register = (register << 8) ^ m_Table[(byte)((register >> (Width - 8)) ^ b)];
        }
        return register;
    }

    /// <inheritdoc/>
    protected sealed override uint ComputeFinal(uint register) => register ^ m_XorOutput;
}
