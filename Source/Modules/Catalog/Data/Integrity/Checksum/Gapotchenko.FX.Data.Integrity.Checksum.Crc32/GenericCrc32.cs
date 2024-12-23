﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using Gapotchenko.FX.Numerics;

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
    /// <param name="useTable">The value indicating whether to use a table generated for the specified polynomial.</param>
    private protected GenericCrc32(
        uint polynomial,
        uint initialValue,
        bool reflectedInput,
        bool reflectedOutput,
        uint xorOutput,
        bool useTable = true)
        : base(
            reflectedOutput ?
                BitOperationsEx.Reverse(initialValue) :
                initialValue)
    {
        m_ReflectedOutput = reflectedOutput;
        m_XorOutput = xorOutput;
        m_Table = useTable ? Crc32TableFactory.GetTable(polynomial, reflectedInput) : [];
    }

    readonly bool m_ReflectedOutput;
    readonly uint m_XorOutput;
    readonly uint[] m_Table;

    /// <inheritdoc/>
    protected override uint ComputeBlock(uint register, ReadOnlySpan<byte> data)
    {
        var table = m_Table;

        if (m_ReflectedOutput)
        {
            foreach (var b in data)
                register = (register >> 8) ^ table[(byte)(register ^ b)];
        }
        else
        {
            foreach (var b in data)
                register = (register << 8) ^ table[(byte)((register >> (Width - 8)) ^ b)];
        }

        return register;
    }

    /// <inheritdoc/>
    protected sealed override uint ComputeFinal(uint register) => register ^ m_XorOutput;
}
