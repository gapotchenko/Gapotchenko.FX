// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Integrity.Checksum;

partial class ChecksumAlgorithm<T>
{
    sealed class HashAlgorithmImpl(ChecksumAlgorithm<T> algorithm, IBitConverter bitConverter) : HashAlgorithm
    {
        public override void Initialize() => m_Iterator.Reset();

        protected override void HashCore(byte[] array, int ibStart, int cbSize) =>
            m_Iterator.ComputeBlock(array.AsSpan(ibStart, cbSize));

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        protected override void HashCore(ReadOnlySpan<byte> source) =>
            m_Iterator.ComputeBlock(source);
#endif

        protected override byte[] HashFinal() =>
            algorithm.GetHashBytesCore(
                m_Iterator.ComputeFinal(),
                bitConverter);

        readonly IChecksumIterator<T> m_Iterator = algorithm.CreateIterator();
    }
}
