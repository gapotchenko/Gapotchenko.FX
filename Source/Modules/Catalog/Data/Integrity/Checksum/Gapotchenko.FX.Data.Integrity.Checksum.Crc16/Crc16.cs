// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

/// <summary>
/// Computes CRC-16 checksum for the input data.
/// </summary>
/// <remarks>
/// Represents the base class from which implementations of CRC-16 checksum algorithm may derive.
/// </remarks>
/// <param name="initialValue">The initial value of the register when the algorithm starts.</param>
[CLSCompliant(false)]
public abstract partial class Crc16(ushort initialValue) : ChecksumAlgorithm<ushort>, ICrc16
{
    /// <summary>
    /// The initial value of the register when the algorithm starts.
    /// </summary>
#pragma warning disable CA1051 // Do not declare visible instance fields
    protected readonly ushort InitialValue = initialValue;
#pragma warning restore CA1051

    /// <summary>
    /// The size, in bits, of the computed checksum value.
    /// </summary>
    protected const int Width = 16;

    /// <inheritdoc/>
    public sealed override int ChecksumSize => Width;

    /// <inheritdoc/>
    public override ushort ComputeChecksum(ReadOnlySpan<byte> data) => ComputeFinal(ComputeBlock(InitialValue, data));

    /// <inheritdoc/>
    public HashAlgorithm CreateHashAlgorithm(IBitConverter bitConverter) =>
        CreateHashAlgorithmCore(bitConverter ?? throw new ArgumentNullException(nameof(bitConverter)));

    /// <inheritdoc/>
    protected override byte[] GetHashBytesCore(ushort checksum, IBitConverter bitConverter) => bitConverter.GetBytes(checksum);

    /// <summary>
    /// Creates an iterator for checksum computation.
    /// </summary>
    /// <returns>An iterator for checksum computation.</returns>
    public new Iterator CreateIterator() => new(this);

    /// <inheritdoc/>
    protected sealed override IChecksumIterator<ushort> CreateIteratorCore() => CreateIterator();

    /// <summary>
    /// Iterator for CRC-16 checksum computation.
    /// </summary>
    public struct Iterator : IChecksumIterator<ushort>
    {
        internal Iterator(Crc16 algorithm)
        {
            m_Algorithm = algorithm;
            Reset();
        }

        /// <inheritdoc/>
        public void ComputeBlock(ReadOnlySpan<byte> data) => m_Register = m_Algorithm.ComputeBlock(m_Register, data);

        /// <inheritdoc/>
        public ushort ComputeFinal()
        {
            var checksum = m_Algorithm.ComputeFinal(m_Register);
            Reset();
            return checksum;
        }

        object IChecksumIterator.ComputeFinal() => ComputeFinal();

        /// <inheritdoc/>
        public void Reset() => m_Register = m_Algorithm.InitialValue;

        readonly Crc16 m_Algorithm;
        ushort m_Register;
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
