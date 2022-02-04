using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Checksum
{
    /// <summary>
    /// Represents the base class from which all implementations of checksum algorithms must derive.
    /// </summary>
    /// <typeparam name="T">The type of checksum value.</typeparam>
    public abstract class ChecksumAlgorithm<T> : HashAlgorithm
        where T : struct
    {
        /// <summary>
        /// Represents the value of the computed checksum.
        /// </summary>
        protected T ChecksumValue;

        /// <summary>
        /// Gets the value of the computed checksum.
        /// </summary>
        /// <exception cref="CryptographicUnexpectedOperationException">Checksum has not yet finalized.</exception>
        /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
        public virtual T Checksum
        {
            get
            {
                EnsureNotDisposed();

                if (State != 0)
                    throw new CryptographicUnexpectedOperationException("Checksum has not yet finalized.");

                return ChecksumValue;
            }
        }

        T ChecksumFinalAndInitialize()
        {
            var checksum = ChecksumFinal();
            ChecksumValue = checksum;
            HashValue = null;

            Initialize();

            return checksum;
        }

        const int BufferSize = 4096;

        /// <summary>
        /// Computes the checksum for the specified <see cref="Stream"/> object.
        /// </summary>
        /// <param name="inputStream">The input stream to compute the checksum for.</param>
        /// <returns>The computed checksum.</returns>
        /// <exception cref="ArgumentNullException">The arguments is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
        public T ComputeChecksum(Stream inputStream)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            EnsureNotDisposed();

            var buffer = new byte[BufferSize];
            int bytesRead;
            do
            {
                bytesRead = inputStream.Read(buffer, 0, BufferSize);
                if (bytesRead > 0)
                    ChecksumCore(buffer.AsSpan(0, bytesRead));
            }
            while (bytesRead > 0);

            return ChecksumFinalAndInitialize();
        }

        /// <summary>
        /// Asynchronously computes the checksum for the specified <see cref="Stream"/> object.
        /// </summary>
        /// <param name="inputStream">The input stream to compute the checksum for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The computed checksum.</returns>
        /// <exception cref="ArgumentNullException">The arguments is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
        public async Task<T> ComputeChecksumAsync(Stream inputStream, CancellationToken cancellationToken = default)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            EnsureNotDisposed();

            var buffer = new byte[BufferSize];
            int bytesRead;
            do
            {
                bytesRead = await
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                    inputStream.ReadAsync(buffer, cancellationToken)
#else
                    inputStream.ReadAsync(buffer, 0, BufferSize, cancellationToken)
#endif
                    .ConfigureAwait(false);

                if (bytesRead > 0)
                    ChecksumCore(buffer.AsSpan(0, bytesRead));
            }
            while (bytesRead > 0);

            return ChecksumFinalAndInitialize();
        }

        /// <summary>
        /// Computes the checksum for the specified byte span.
        /// </summary>
        /// <param name="buffer">The input to compute the checksum for.</param>
        /// <returns>The computed checksum.</returns>
        /// <exception cref="ArgumentNullException">The arguments is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">The object has already been disposed.</exception>
        public T ComputeChecksum(ReadOnlySpan<byte> buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            EnsureNotDisposed();

            ChecksumCore(buffer);
            return ChecksumFinalAndInitialize();
        }

        /// <inheritdoc/>
        public override byte[]? Hash
        {
            get
            {
                var checksum = Checksum;
                return HashValue ??= ChecksumHash(checksum);
            }
        }

        /// <inheritdoc/>
        protected sealed override void HashCore(byte[] array, int ibStart, int cbSize) =>
            ChecksumCore(array.AsSpan(ibStart, cbSize));

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        /// <inheritdoc/>
        protected sealed override void HashCore(ReadOnlySpan<byte> source) => ChecksumCore(source);
#endif

        /// <inheritdoc/>
        protected sealed override byte[] HashFinal()
        {
            var checksum = ChecksumFinal();
            ChecksumValue = checksum;
            return ChecksumHash(checksum);
        }

        bool m_Disposed;

        void EnsureNotDisposed()
        {
            if (m_Disposed)
                throw new ObjectDisposedException(null);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ChecksumValue = default;

                m_Disposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Routes data written to the object into the checksum algorithm for computing the checksum.
        /// </summary>
        /// <param name="source">The input to compute the checksum for.</param>
        protected abstract void ChecksumCore(ReadOnlySpan<byte> source);

        /// <summary>
        /// Finalizes the checksum computation after the last data is processed by the checksum algorithm.
        /// </summary>
        /// <returns>The computed checksum.</returns>
        protected abstract T ChecksumFinal();

        /// <summary>
        /// Converts checksum to a hash representation.
        /// </summary>
        /// <param name="checksum">The checksum to get the hash representation for.</param>
        /// <returns>The hash representation of the specified checksum.</returns>
        protected abstract byte[] ChecksumHash(T checksum);
    }
}
