// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Computes CRC-8 checksum for the input data.
/// </summary>
/// <remarks>
/// Represents the base class from which implementations of CRC-8 checksum algorithm may derive.
/// </remarks>
public abstract partial class Crc8 : ChecksumAlgorithm<byte>, ICrc8
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Crc8"/> class.
    /// </summary>
    /// <param name="initialValue">The initial value of the register when the algorithm starts.</param>
    protected Crc8(byte initialValue)
    {
        InitialValue = initialValue;
    }

    /// <summary>
    /// The initial value of the register when the algorithm starts.
    /// </summary>
    protected readonly byte InitialValue;

    /// <summary>
    /// The size, in bits, of the computed checksum value.
    /// </summary>
    protected const int Width = 8;

    /// <inheritdoc/>
    public sealed override int ChecksumSize => Width;

    /// <inheritdoc/>
    public override byte ComputeChecksum(ReadOnlySpan<byte> data) => ComputeFinal(ComputeBlock(InitialValue, data));

    /// <inheritdoc/>
    protected override byte[] GetHashBytesCore(byte checksum, IBitConverter bitConverter) => new[] { checksum };

    /// <summary>
    /// Creates an iterator for checksum computation.
    /// </summary>
    /// <returns>An iterator for checksum computation.</returns>
    public new Iterator CreateIterator() => new(this);

    /// <inheritdoc/>
    protected sealed override IChecksumIterator<byte> CreateIteratorCore() => CreateIterator();

    /// <summary>
    /// Iterator for CRC-8 checksum computation.
    /// </summary>
    public struct Iterator : IChecksumIterator<byte>
    {
        internal Iterator(Crc8 algorithm)
        {
            m_Algorithm = algorithm;
            m_Register = m_Algorithm.InitialValue;
        }

        readonly Crc8 m_Algorithm;
        byte m_Register;

        /// <inheritdoc/>
        public void ComputeBlock(ReadOnlySpan<byte> data) => m_Register = m_Algorithm.ComputeBlock(m_Register, data);

        /// <inheritdoc/>
        public byte ComputeFinal()
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
    protected abstract byte ComputeBlock(byte register, ReadOnlySpan<byte> data);

    /// <summary>
    /// Finalizes the checksum computation after the last data is processed.
    /// </summary>
    /// <returns>The computed checksum.</returns>
    protected abstract byte ComputeFinal(byte register);
}
