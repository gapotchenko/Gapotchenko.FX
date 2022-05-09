using System;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Integrity.Checksum
{
    partial class ChecksumAlgorithm<T>
    {
        sealed class HashAlgorithmImpl : HashAlgorithm
        {
            readonly ChecksumAlgorithm<T> m_Algorithm;
            readonly IBitConverter m_BitConverter;
            readonly IChecksumIterator<T> m_Iterator;

            public HashAlgorithmImpl(ChecksumAlgorithm<T> algorithm, IBitConverter bitConverter)
            {
                m_Algorithm = algorithm;
                m_BitConverter = bitConverter;

                m_Iterator = algorithm.CreateIterator();
            }

            public override void Initialize() => m_Iterator.Reset();

            protected override void HashCore(byte[] array, int ibStart, int cbSize) =>
                m_Iterator.ComputeBlock(array.AsSpan(ibStart, cbSize));

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            protected override void HashCore(ReadOnlySpan<byte> source) =>
                m_Iterator.ComputeBlock(source);
#endif

            protected override byte[] HashFinal() =>
                m_Algorithm.GetHashBytesCore(
                    m_Iterator.ComputeFinal(),
                    m_BitConverter);
        }
    }
}
