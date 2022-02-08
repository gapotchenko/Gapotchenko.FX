using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// The base class for <see cref="IChecksumAlgorithm{T}"/> implementations.
    /// </summary>
    public abstract class ChecksumAlgorithm<T> : IChecksumAlgorithm<T>
        where T : struct
    {
        /// <inheritdoc/>
        public abstract int ChecksumSize { get; }

        /// <inheritdoc/>
        public virtual T ComputeChecksum(ReadOnlySpan<byte> data)
        {
            var iterator = CreateIterator();
            iterator.ComputeBlock(data);
            return iterator.ComputeFinal();
        }

        const int BufferSize = 4096;

        /// <inheritdoc/>
        public virtual T ComputeChecksum(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            var iterator = CreateIterator();

            var buffer = new byte[BufferSize];
            for (; ; )
            {
                int bytesRead = inputStream.Read(buffer, 0, BufferSize);
                if (bytesRead <= 0)
                    break;
                iterator.ComputeBlock(buffer.AsSpan(0, bytesRead));
            }

            return iterator.ComputeFinal();
        }

        /// <inheritdoc/>
        public virtual async Task<T> ComputeChecksumAsync(Stream inputStream, CancellationToken cancellationToken = default)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            var iterator = CreateIterator();

            var buffer = new byte[BufferSize];
            for (; ; )
            {
                int bytesRead = await
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    inputStream.ReadAsync(buffer, cancellationToken)
#else
                    inputStream.ReadAsync(buffer, 0, BufferSize, cancellationToken)
#endif
                    .ConfigureAwait(false);
                if (bytesRead <= 0)
                    break;
                iterator.ComputeBlock(buffer.AsSpan(0, bytesRead));
            }

            return iterator.ComputeFinal();
        }

        /// <inheritdoc/>
        public IChecksumIterator<T> CreateIterator() => CreateIteratorCore();

        /// <summary>
        /// Creates an iterator for checksum computation.
        /// </summary>
        /// <returns>An iterator for checksum computation.</returns>
        protected abstract IChecksumIterator<T> CreateIteratorCore();
    }
}
