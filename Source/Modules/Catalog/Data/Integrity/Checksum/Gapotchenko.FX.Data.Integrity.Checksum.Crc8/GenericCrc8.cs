// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Numerics;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Provides a generic implementation of CRC-8 checksum algorithm.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericCrc8 : Crc8
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericCrc8"/> class with specified CRC-8 algorithm parameters.
    /// </summary>
    /// <param name="polynomial">The polynomial used for the CRC-8 calculation, omitting the top bit.</param>
    /// <param name="initialValue">The initial value of the register when the algorithm starts.</param>
    /// <param name="reflectedInput">A value indicating whether to treat bit 7 of input bytes as the least significant bit (LSB).</param>
    /// <param name="reflectedOutput">A value indicating whether to reflect the final register value before feeding it into the XORout stage.</param>
    /// <param name="xorOutput">The value which is XORed with the final register value to form an official checksum value.</param>
    private protected GenericCrc8(
        byte polynomial,
        byte initialValue,
        bool reflectedInput,
        bool reflectedOutput,
        byte xorOutput) :
        base(
            reflectedOutput ?
                BitOperationsEx.Reverse(initialValue) :
                initialValue)
    {
        m_XorOutput = xorOutput;
        m_Table = Crc8TableFactory.GetTable(polynomial, reflectedInput);
    }

    readonly byte m_XorOutput;
    readonly byte[] m_Table;

    /// <inheritdoc/>
    protected sealed override byte ComputeBlock(byte register, ReadOnlySpan<byte> data)
    {
        foreach (var b in data)
            register = m_Table[(byte)(register ^ b)];
        return register;
    }

    /// <inheritdoc/>
    protected sealed override byte ComputeFinal(byte register) => (byte)(register ^ m_XorOutput);
}
