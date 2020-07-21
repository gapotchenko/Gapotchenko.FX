using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

#nullable enable

namespace Gapotchenko.FX.IO
{
    /// <summary>
    /// <see cref="Stream"/> extensions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class StreamExtensions
    {
        const int DefaultBlockCopyBufferSize = 0x10000;

        /// <summary>
        /// Reads the bytes from the source stream and writes them to destination stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="count">The count of bytes to copy.</param>
        /// <param name="bufferSize">The buffer size.</param>
        public static void CopyBlockTo(this Stream source, Stream destination, long count, int bufferSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return;

            byte[] buffer = new byte[bufferSize];
            while (count != 0)
            {
                int bytesToRead = (int)(Math.Min(bufferSize, count));

                int readBytes = source.Read(buffer, 0, bytesToRead);
                if (readBytes == 0)
                {
                    // EOF
                    break;
                }

                destination.Write(buffer, 0, readBytes);
                count -= readBytes;
            }

            if (count != 0)
                throw new EndOfStreamException();
        }

        /// <summary>
        /// Reads the bytes from the source stream and writes them to destination stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="count">The count of bytes to copy.</param>
        public static void CopyBlockTo(this Stream source, Stream destination, long count) =>
            source.CopyBlockTo(destination, count, DefaultBlockCopyBufferSize);

#if TFF_ASYNC_STREAM

        /// <summary>
        /// Reads the bytes from the source stream and writes them to destination stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="count">The count of bytes to copy.</param>
        /// <param name="bufferSize">The buffer size.</param>
        /// <returns>A task that represents the asynchronous copy operation.</returns>
        public static async Task CopyBlockToAsync(this Stream source, Stream destination, long count, int bufferSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return;

            byte[] buffer = new byte[bufferSize];
            while (count != 0)
            {
                int bytesToRead = (int)(Math.Min(bufferSize, count));

                int readBytes = await source.ReadAsync(buffer, 0, bytesToRead).ConfigureAwait(false);
                if (readBytes == 0)
                {
                    // EOF
                    break;
                }

                await destination.WriteAsync(buffer, 0, readBytes).ConfigureAwait(false);
                count -= readBytes;
            }

            if (count != 0)
                throw new EndOfStreamException();
        }

        /// <summary>
        /// Reads the bytes from the source stream and writes them to destination stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        /// <param name="count">The count of bytes to copy.</param>
        /// <returns>A task that represents the asynchronous copy operation.</returns>
        public static Task CopyBlockToAsync(this Stream source, Stream destination, long count) =>
            source.CopyBlockToAsync(destination, count, DefaultBlockCopyBufferSize);

#endif
    }
}
