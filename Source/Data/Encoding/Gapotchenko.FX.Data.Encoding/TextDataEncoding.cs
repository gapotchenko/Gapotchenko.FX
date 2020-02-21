using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    using Encoding = System.Text.Encoding;

    /// <summary>
    /// The base class for <see cref="ITextDataEncoding"/> implementations.
    /// </summary>
    public abstract class TextDataEncoding : DataEncoding, ITextDataEncoding
    {
        /// <inheritdoc/>
        public abstract bool IsCaseSensitive { get; }

        /// <inheritdoc/>
        public string GetString(ReadOnlySpan<byte> data) => GetString(data, DataEncodingOptions.None);

        /// <inheritdoc/>
        public string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => data == null ? null : GetStringCore(data, options);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="data"/>.</returns>
        protected virtual string GetStringCore(ReadOnlySpan<byte> data, DataEncodingOptions options)
        {
            var sw = new StringWriter();

            var context = CreateEncoderContext(options);
            context.Encode(data, sw);
            context.Encode(null, sw);

            return sw.ToString();
        }

        /// <inheritdoc/>
        public byte[] GetBytes(ReadOnlySpan<char> s) => GetBytes(s, DataEncodingOptions.None);

        /// <inheritdoc/>
        public byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => s == null ? null : GetBytesCore(s, options);

        /// <summary>
        /// Decodes the specified string to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        protected virtual byte[] GetBytesCore(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            var ms = new MemoryStream();

            var context = CreateDecoderContext(options);
            context.Decode(s, ms);
            context.Decode(null, ms);

            return ms.ToArray();
        }

        /// <inheritdoc/>
        protected override byte[] EncodeDataCore(ReadOnlySpan<byte> data, DataEncodingOptions options) => Encoding.UTF8.GetBytes(GetString(data, options));

        /// <inheritdoc/>
        protected override byte[] DecodeDataCore(ReadOnlySpan<byte> data, DataEncodingOptions options) =>
            GetBytes(
                Encoding.UTF8.GetString(
#if TFF_MEMORY && !TFF_MEMORY_OOB
                    data
#else
                    data.ToArray()
#endif
                )
                .AsSpan(),
                options);

        /// <summary>
        /// The encoder context.
        /// </summary>
        protected interface IEncoderContext
        {
            /// <summary>
            /// Encodes a block of data.
            /// </summary>
            /// <param name="input">
            /// The input.
            /// The <c>null</c> value signals a final block.
            /// </param>
            /// <param name="output">The output.</param>
            void Encode(ReadOnlySpan<byte> input, TextWriter output);
        }

        /// <summary>
        /// Creates encoder context.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The encoder context.</returns>
        protected abstract IEncoderContext CreateEncoderContext(DataEncodingOptions options);

        /// <summary>
        /// The decoder context.
        /// </summary>
        protected interface IDecoderContext
        {
            /// <summary>
            /// Decodes a block of data.
            /// </summary>
            /// <param name="input">
            /// The input.
            /// The <c>null</c> value signals a final block.
            /// </param>
            /// <param name="output">The output.</param>
            void Decode(ReadOnlySpan<char> input, Stream output);
        }

        /// <summary>
        /// Creates decoder context.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The decoder context.</returns>
        protected abstract IDecoderContext CreateDecoderContext(DataEncodingOptions options);

        const int StreamBufferSize = 2048;

        /// <inheritdoc/>
        public Stream CreateEncoder(TextWriter textWriter, DataEncodingOptions options = DataEncodingOptions.None)
        {
            if (textWriter == null)
                throw new ArgumentNullException(nameof(textWriter));

            var context = CreateEncoderContext(options);
            return new EncoderStream(textWriter, context, options, StreamBufferSize);
        }

        /// <inheritdoc/>
        public Stream CreateDecoder(TextReader textReader, DataEncodingOptions options = DataEncodingOptions.None)
        {
            if (textReader == null)
                throw new ArgumentNullException(nameof(textReader));

            var context = CreateDecoderContext(options);
            return new DecoderStream(textReader, context, options, StreamBufferSize, MinEfficiency);
        }

        sealed class EncoderStream : Stream
        {
            public EncoderStream(TextWriter textWriter, IEncoderContext context, DataEncodingOptions options, int bufferSize)
            {
                m_TextWriter = textWriter;
                m_Context = context;
                m_BufferSize = bufferSize;

                m_OwnsTextWriter = (options & DataEncodingOptions.NoOwnership) == 0;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    FlushFinalBlock();

                    if (m_OwnsTextWriter)
                        m_TextWriter.Dispose();
                }
            }

#if TFF_ASYNC_DISPOSABLE
            public override async ValueTask DisposeAsync()
            {
                await FlushFinalBlockAsync().ConfigureAwait(false);

                if (m_OwnsTextWriter)
                    await m_TextWriter.DisposeAsync().ConfigureAwait(false);
            }
#endif

            readonly TextWriter m_TextWriter;
            readonly IEncoderContext m_Context;
            readonly bool m_OwnsTextWriter;

            StringBuilder m_Buffer;
            StringWriter m_BufferWriter;
            readonly int m_BufferSize;

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count)
            {
                m_Context.Encode(new ReadOnlySpan<byte>(buffer, offset, count), m_TextWriter);
            }

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                if (count != 0)
                    await WriteAsyncCore(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).ConfigureAwait(false);
            }

#if TFF_VALUETASK && !NETCOREAPP2_0
            public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
            {
                await WriteAsyncCore(buffer, cancellationToken).ConfigureAwait(false);
            }
#endif

            async Task WriteAsyncCore(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
            {
                if (buffer.IsEmpty)
                    return;

                EnsureBufferCreated();

                foreach (var chunk in SplitMemoryIntoChunks(buffer, m_BufferSize))
                {
                    m_Context.Encode(chunk.Span, m_BufferWriter);
                    await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            static IEnumerable<ReadOnlyMemory<T>> SplitMemoryIntoChunks<T>(ReadOnlyMemory<T> memory, int chunkSize)
            {
                int memoryLength = memory.Length;
                if (memoryLength <= chunkSize)
                {
                    yield return memory;
                    yield break;
                }

                int chunkOffset = 0;
                while (chunkOffset < memoryLength)
                {
                    int chunkLength = Math.Min(chunkSize, memoryLength - chunkOffset);
                    yield return memory.Slice(chunkOffset, chunkLength);
                    chunkOffset += chunkLength;
                }
            }

            public override void Flush()
            {
                FlushFinalBlock();
                m_TextWriter.Flush();
            }

            void FlushFinalBlock()
            {
                FlushBuffer();
                m_Context.Encode(null, m_TextWriter);
            }

#if TFF_ASYNC_DISPOSABLE
            async Task FlushFinalBlockAsync(CancellationToken cancellationToken = default)
            {
                EnsureBufferCreated();
                m_Context.Encode(null, m_BufferWriter);
                await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
            }
#endif

            void EnsureBufferCreated()
            {
                if (m_Buffer != null)
                    return;

                m_Buffer = new StringBuilder();
                m_BufferWriter = new StringWriter(m_Buffer)
                {
                    NewLine = m_TextWriter.NewLine
                };
            }

            void FlushBuffer()
            {
                if (m_Buffer == null)
                    return;

                m_BufferWriter.Flush();
                if (m_Buffer.Length != 0)
                {
#if TFF_TEXT_IO_STRINGBUILDER
                    m_TextWriter.Write(m_Buffer);
#else
                    m_TextWriter.Write(m_Buffer.ToString());
#endif
                    m_Buffer.Clear();
                }
            }

            async Task FlushBufferAsync(CancellationToken cancellationToken)
            {
                if (m_Buffer == null)
                    return;

                m_BufferWriter.Flush();
                if (m_Buffer.Length != 0)
                {
#if TFF_TEXT_IO_STRINGBUILDER
                    await m_TextWriter.WriteAsync(m_Buffer, cancellationToken).ConfigureAwait(false);
#elif TFF_TEXT_IO_CANCELLATION
                    await m_TextWriter.WriteAsync(m_Buffer.ToString().AsMemory(), cancellationToken).ConfigureAwait(false);
#else
                    cancellationToken.ThrowIfCancellationRequested();
                    await m_TextWriter.WriteAsync(m_Buffer.ToString()).ConfigureAwait(false);
#endif
                    m_Buffer.Clear();
                }
            }
        }

        sealed class DecoderStream : Stream
        {
            public DecoderStream(
                TextReader textReader,
                IDecoderContext context,
                DataEncodingOptions options,
                int bufferSize,
                float efficiency)
            {
                m_TextReader = textReader;
                m_Context = context;
                m_Efficiency = efficiency;
                m_BufferSize = bufferSize;

                m_OwnsTextReader = (options & DataEncodingOptions.NoOwnership) == 0;

                m_Buffer = new MemoryStream();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (m_OwnsTextReader)
                        m_TextReader.Dispose();
                }
            }

            readonly TextReader m_TextReader;
            readonly IDecoderContext m_Context;
            readonly float m_Efficiency;
            readonly bool m_OwnsTextReader;

            readonly int m_BufferSize;
            MemoryStream m_Buffer;

            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => throw new NotSupportedException();

            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override int Read(byte[] buffer, int offset, int count)
            {
                int totalRead = 0;
                for (; ; )
                {
                    if (m_Buffer.Length == 0)
                        FillBuffer(count);

                    int r = m_Buffer.Read(buffer, offset, count);
                    if (r == 0)
                        break;

                    offset += r;
                    count -= r;
                    totalRead += r;

                    if (count == 0)
                        break;

                    m_Buffer.SetLength(0);
                }
                return totalRead;
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                int totalRead = 0;
                for (; ; )
                {
                    if (m_Buffer.Length == 0)
                        await FillBufferAsync(count, cancellationToken).ConfigureAwait(false);

                    int r = m_Buffer.Read(buffer, offset, count);
                    if (r == 0)
                        break;

                    offset += r;
                    count -= r;
                    totalRead += r;

                    if (count == 0)
                        break;

                    m_Buffer.SetLength(0);
                }
                return totalRead;
            }

#if TFF_VALUETASK && !NETCOREAPP2_0
            public override int Read(Span<byte> buffer)
            {
                int count = buffer.Length;
                int offset = 0;
                int totalRead = 0;

                for (; ; )
                {
                    if (m_Buffer.Length == 0)
                        FillBuffer(count);

                    int r = m_Buffer.Read(buffer.Slice(offset, count));
                    if (r == 0)
                        break;

                    offset += r;
                    count -= r;
                    totalRead += r;

                    if (count == 0)
                        break;

                    m_Buffer.SetLength(0);
                }

                return totalRead;
            }

            public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
            {
                int count = buffer.Length;
                int offset = 0;
                int totalRead = 0;

                for (; ; )
                {
                    if (m_Buffer.Length == 0)
                        await FillBufferAsync(count, cancellationToken).ConfigureAwait(false);

                    int r = m_Buffer.Read(buffer.Slice(offset, count).Span);
                    if (r == 0)
                        break;

                    offset += r;
                    count -= r;
                    totalRead += r;

                    if (count == 0)
                        break;

                    m_Buffer.SetLength(0);
                }

                return totalRead;
            }
#endif

            public override void Flush()
            {
            }

            void FillBuffer(int count)
            {
                count = Math.Min(count, m_BufferSize);

                var text = new char[m_BufferSize / sizeof(char)];
                while (m_Buffer.Length < count)
                {
                    int byteCountToRead = count - (int)m_Buffer.Length;

                    int charCountToRead = Math.Min(
                        checked((int)Math.Ceiling(byteCountToRead / m_Efficiency * 1.1f)),
                        text.Length);

                    int r = m_TextReader.ReadBlock(text, 0, charCountToRead);
                    if (r == 0)
                    {
                        // Final block.
                        m_Context.Decode(null, m_Buffer);
                        break;
                    }

                    m_Context.Decode(text.AsSpan(0, r), m_Buffer);
                }

                m_Buffer.Position = 0;
            }

            async Task FillBufferAsync(int count, CancellationToken cancellationToken)
            {
                count = Math.Min(count, m_BufferSize);

                var text = new char[m_BufferSize / sizeof(char)];
                while (m_Buffer.Length < count)
                {
                    int byteCountToRead = count - (int)m_Buffer.Length;

                    int charCountToRead = Math.Min(
                        checked((int)Math.Ceiling(byteCountToRead / m_Efficiency * 1.1f)),
                        text.Length);

                    int r;
#if TFF_TEXT_IO_CANCELLATION
                    r = await m_TextReader.ReadBlockAsync(text.AsMemory(0, charCountToRead), cancellationToken).ConfigureAwait(false);
#else
                    cancellationToken.ThrowIfCancellationRequested();
                    r = await m_TextReader.ReadBlockAsync(text, 0, charCountToRead).ConfigureAwait(false);
#endif
                    if (r == 0)
                    {
                        // Final block.
                        m_Context.Decode(null, m_Buffer);
                        break;
                    }

                    m_Context.Decode(text.AsSpan(0, r), m_Buffer);
                }

                m_Buffer.Position = 0;
            }
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Stream CreateEncoder(Stream outputStream, DataEncodingOptions options = DataEncodingOptions.None)
        {
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));
            return CreateEncoder(new StreamWriter(outputStream), options);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Stream CreateDecoder(Stream inputStream, DataEncodingOptions options = DataEncodingOptions.None)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));
            return CreateDecoder(new StreamReader(inputStream), options);
        }

        /// <inheritdoc/>
        public string Pad(ReadOnlySpan<char> s) => s == null ? null : PadCore(s);

        /// <summary>
        /// Pads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to pad.</param>
        /// <returns>The padded encoded string.</returns>
        protected virtual string PadCore(ReadOnlySpan<char> s) => s.ToString();

        /// <inheritdoc/>
        public ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s) => s == null ? null : UnpadCore(s);

        /// <summary>
        /// Unpads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to unpad.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected virtual ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => s;

        /// <inheritdoc/>
        public bool IsPadded(ReadOnlySpan<char> s) => s.Length % Padding == 0;

        #region Implementation Helpers

        string Pad(ReadOnlySpan<char> s, char paddingChar, bool right)
        {
            if (s == null)
                return null;

            int padding = Padding;

            int width =
                padding < 2 ?
                    0 :
                    (s.Length + padding - 1) / padding * padding;

            string output = s.ToString();

            if (width == 0)
                return output;
            else if (right)
                return output.PadRight(width, paddingChar);
            else
                return output.PadLeft(width, paddingChar);
        }

        /// <summary>
        /// Pads the encoded string to the right.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The padded encoded string.</returns>
        protected string PadRight(ReadOnlySpan<char> s, char paddingChar) => Pad(s, paddingChar, true);

        /// <summary>
        /// Pads the encoded string to the left.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The padded encoded string.</returns>
        protected string PadLeft(ReadOnlySpan<char> s, char paddingChar) => Pad(s, paddingChar, false);

        /// <summary>
        /// Unpads the encoded string from the right side.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected ReadOnlySpan<char> UnpadRight(ReadOnlySpan<char> s, char paddingChar) => s.TrimEnd(paddingChar);

        /// <summary>
        /// Unpads the encoded string from the left side.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected ReadOnlySpan<char> UnpadLeft(ReadOnlySpan<char> s, char paddingChar) => s.TrimStart(paddingChar);

        #endregion
    }
}
