using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
        /// <summary>
        /// Initializes a new instance of <see cref="TextDataEncoding"/> class.
        /// </summary>
        protected TextDataEncoding()
        {
        }

        /// <inheritdoc/>
        public abstract bool IsCaseSensitive { get; }

        /// <inheritdoc/>
        [return: NotNullIfNotNull("data")]
        public string? GetString(ReadOnlySpan<byte> data) => GetString(data, DataEncodingOptions.None);

        /// <inheritdoc/>
        [return: NotNullIfNotNull("data")]
        public string? GetString(ReadOnlySpan<byte> data, DataEncodingOptions options)
        {
            options = GetEffectiveOptions(options);
            return data == null ? null : GetStringCore(data, options);
        }

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="data"/>.</returns>
        protected virtual string GetStringCore(ReadOnlySpan<byte> data, DataEncodingOptions options)
        {
            var capacity = GetMaxCharCountCore(data.Length, options);

            var sb = new StringBuilder(capacity);
            var sw = new StringWriter(sb);

            var context = CreateEncoderContext(options);
            context.Encode(data, sw);
            context.Encode(null, sw);

            Debug.Assert(sb.Length <= capacity, "Invalid capacity.");

            return sw.ToString();
        }

        /// <inheritdoc/>
        [return: NotNullIfNotNull("s")]
        public byte[]? GetBytes(ReadOnlySpan<char> s) => GetBytes(s, DataEncodingOptions.None);

        /// <inheritdoc/>
        [return: NotNullIfNotNull("s")]
        public byte[]? GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            options = GetEffectiveOptions(options);
            return s == null ? null : GetBytesCore(s, options);
        }

        /// <summary>
        /// Decodes the specified string to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        protected virtual byte[] GetBytesCore(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            var capacity = GetMaxByteCountCore(s.Length, options);

            var ms = new MemoryStream(capacity);

            var context = CreateDecoderContext(options);
            try
            {
                context.Decode(s, ms);
                context.Decode(null, ms);
            }
            catch (InvalidDataException e)
            {
                throw new FormatException(e.Message);
            }

            Debug.Assert(ms.Length <= capacity, "Invalid capacity.");

            return ms.ToArray();
        }

        /// <inheritdoc/>
        protected override byte[] EncodeDataCore(ReadOnlySpan<byte> data, DataEncodingOptions options) => Encoding.UTF8.GetBytes(GetStringCore(data, options));

        /// <inheritdoc/>
        protected override byte[] DecodeDataCore(ReadOnlySpan<byte> data, DataEncodingOptions options) =>
            GetBytesCore(
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

            options = GetEffectiveStreamingOptions(options);

            if ((options & DataEncodingOptions.Unpad) == 0)
            {
                // Encode stream with a padding unless it is explicitly disabled.
                options |= DataEncodingOptions.Padding;
            }

            var context = CreateEncoderContext(options);
            return new EncoderStream(this, textWriter, context, options, StreamBufferSize);
        }

        /// <inheritdoc/>
        public Stream CreateDecoder(TextReader textReader, DataEncodingOptions options = DataEncodingOptions.None)
        {
            if (textReader == null)
                throw new ArgumentNullException(nameof(textReader));

            options = GetEffectiveStreamingOptions(options);

            var context = CreateDecoderContext(options);
            return new DecoderStream(this, textReader, context, options, StreamBufferSize);
        }

        sealed class EncoderStream : Stream
        {
            public EncoderStream(TextDataEncoding encoding, TextWriter textWriter, IEncoderContext context, DataEncodingOptions options, int bufferSize)
            {
                m_Encoding = encoding;
                m_TextWriter = textWriter;
                m_Context = context;
                m_Options = options;

                m_BufferSize = bufferSize;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    FlushFinalBlock();

                    if ((m_Options & DataEncodingOptions.NoOwnership) == 0)
                        m_TextWriter.Dispose();
                }
            }

#if TFF_ASYNC_DISPOSABLE
            public override async ValueTask DisposeAsync()
            {
                await FlushFinalBlockAsync().ConfigureAwait(false);

                if ((m_Options & DataEncodingOptions.NoOwnership) == 0)
                    await m_TextWriter.DisposeAsync().ConfigureAwait(false);
            }
#endif

#nullable disable
            readonly TextDataEncoding m_Encoding;
            readonly TextWriter m_TextWriter;
            readonly IEncoderContext m_Context;
            readonly DataEncodingOptions m_Options;

            readonly int m_BufferSize;
            StringBuilder m_Buffer;
            StringWriter m_BufferWriter;
            int m_BufferCapacityByteCount;

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
                    EnsureBufferByteCapacity(chunk.Length);

#if DEBUG
                    int capacity = m_Buffer.Capacity;
#endif
                    m_Context.Encode(chunk.Span, m_BufferWriter);
#if DEBUG
                    Debug.Assert(m_Buffer.Length <= capacity, "Invalid buffer capacity.");
#endif

                    await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            void EnsureBufferByteCapacity(int byteCount)
            {
                if (byteCount > m_BufferCapacityByteCount)
                {
                    int capacity = m_Encoding.GetMaxCharCount(byteCount, m_Options) + m_Encoding.Padding;
                    m_Buffer.EnsureCapacity(capacity);
                    m_BufferCapacityByteCount = byteCount;
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
#nullable restore
        }

        sealed class DecoderStream : Stream
        {
            public DecoderStream(
                TextDataEncoding encoding,
                TextReader textReader,
                IDecoderContext context,
                DataEncodingOptions options,
                int bufferSize)
            {
                m_Encoding = encoding;
                m_TextReader = textReader;
                m_Context = context;
                m_Options = options;

                m_BufferSize = bufferSize;
                m_Buffer = new MemoryStream();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if ((m_Options & DataEncodingOptions.NoOwnership) == 0)
                        m_TextReader.Dispose();
                }
            }

            readonly TextDataEncoding m_Encoding;
            readonly TextReader m_TextReader;
            readonly IDecoderContext m_Context;
            readonly DataEncodingOptions m_Options;

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

            int GetMaxCharCount(int byteCount) =>
                m_Encoding.GetMaxCharCount(
                    byteCount,
                    (m_Options & ~DataEncodingOptions.Unpad) | DataEncodingOptions.Padding | DataEncodingOptions.Wrap | DataEncodingOptions.Indent);

            void FillBuffer(int count)
            {
                count = Math.Min(count, m_BufferSize);

                var text = new char[m_BufferSize / sizeof(char)];
                while (m_Buffer.Length < count)
                {
                    int byteCountToRead = count - (int)m_Buffer.Length;
                    int charCountToRead = Math.Min(GetMaxCharCount(byteCountToRead), text.Length);

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
                    int charCountToRead = Math.Min(GetMaxCharCount(byteCountToRead), text.Length);

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
        public override bool PrefersPadding =>
            CanPad &&
            (GetEffectiveOptions(DataEncodingOptions.None) & DataEncodingOptions.Unpad) == 0;

        /// <inheritdoc/>
        [return: NotNullIfNotNull("s")]
        public string? Pad(ReadOnlySpan<char> s) => s == null ? null : PadCore(s);

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
        public abstract bool CanCanonicalize { get; }

        /// <inheritdoc/>
        [return: NotNullIfNotNull("s")]
        public string? Canonicalize(ReadOnlySpan<char> s)
        {
            if (s == null)
                return null;
            if (s.IsEmpty)
                return string.Empty;

            if (!CanCanonicalize)
                return s.ToString();

            var d = new char[s.Length];
            CanonicalizeCore(s, d);

            return new string(d);
        }

        /// <summary>
        /// Canonicalizes the encoded symbols.
        /// Canonicalization substitutes the encoded symbols from <paramref name="source"/> with their canonical forms and writes the result to <paramref name="destination"/>.
        /// Unrecognized and whitespace symbols are left intact.
        /// </summary>
        /// <param name="source">The source characters span.</param>
        /// <param name="destination">
        /// The destination characters span.
        /// Can be the same as <paramref name="source"/>.
        /// </param>
        protected virtual void CanonicalizeCore(ReadOnlySpan<char> source, Span<char> destination)
        {
            if (source != destination)
                source.CopyTo(destination);
        }

        /// <inheritdoc/>
        public int GetMaxCharCount(int byteCount) => GetMaxCharCountCore(byteCount, DataEncodingOptions.None);

        /// <inheritdoc/>
        public int GetMaxCharCount(int byteCount, DataEncodingOptions options)
        {
            if (byteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Byte count cannot be negative.");

            return GetMaxCharCountCore(byteCount, GetEffectiveOptions(options));
        }

        /// <summary>
        /// Calculates the maximum number of characters produced by encoding the specified number of bytes with options.
        /// </summary>
        /// <param name="byteCount">The number of bytes to encode.</param>
        /// <param name="options">The options.</param>
        /// <returns>The maximum number of characters produced by encoding the specified number of bytes.</returns>
        protected abstract int GetMaxCharCountCore(int byteCount, DataEncodingOptions options);

        /// <inheritdoc/>
        public int GetMaxByteCount(int charCount) => GetMaxByteCountCore(charCount, DataEncodingOptions.None);

        /// <inheritdoc/>
        public int GetMaxByteCount(int charCount, DataEncodingOptions options)
        {
            if (charCount < 0)
                throw new ArgumentOutOfRangeException(nameof(charCount), "Character count cannot be negative.");

            return GetMaxByteCountCore(charCount, GetEffectiveOptions(options));
        }

        /// <summary>
        /// Calculates the maximum number of bytes produced by decoding the specified number of characters with options.
        /// </summary>
        /// <param name="charCount">The number of characters to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>The maximum number of bytes produced by decoding the specified number of characters.</returns>
        protected abstract int GetMaxByteCountCore(int charCount, DataEncodingOptions options);

        #region Implementation Helpers

        [return: NotNullIfNotNull("s")]
        string? Pad(ReadOnlySpan<char> s, char paddingChar, bool right)
        {
            if (s == null)
                return null;

            int padding = Padding;
            if (padding < 2)
            {
                // No padding. Pass through.
                return s.ToString();
            }

            int width = Pad(s.Length);

            string output = s.ToString();
            if (right)
                return output.PadRight(width, paddingChar);
            else
                return output.PadLeft(width, paddingChar);
        }

        /// <summary>
        /// Calculates the number of characters with applied padding.
        /// </summary>
        /// <param name="charCount">The number of characters.</param>
        /// <returns>The number of characters with applied padding.</returns>
        protected int Pad(int charCount)
        {
            int padding = Padding;
            return
                padding < 2 ?
                    charCount :
                    (charCount + padding - 1) / padding * padding;
        }

        /// <summary>
        /// Pads the encoded string to the right.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The padded encoded string.</returns>
        [return: NotNullIfNotNull("s")]
        protected string? PadRight(ReadOnlySpan<char> s, char paddingChar) => Pad(s, paddingChar, true);

        /// <summary>
        /// Pads the encoded string to the left.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The padded encoded string.</returns>
        [return: NotNullIfNotNull("s")]
        protected string? PadLeft(ReadOnlySpan<char> s, char paddingChar) => Pad(s, paddingChar, false);

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

        /// <summary>
        /// Maximum number of characters in a line terminator.
        /// </summary>
        protected const int MaxNewLineCharCount = 2; // CR LF is the longest line terminator

        /// <summary>
        /// Gets count of leading zero characters.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="zeroChar">The character to treat as zero.</param>
        /// <returns>The count of leading zero characters.</returns>
        protected static int GetLeadingZeroCount(ReadOnlySpan<char> s, char zeroChar)
        {
            int count = 0;
            foreach (var c in s)
            {
                if (c != zeroChar)
                    break;
                checked { ++count; }
            }
            return count;
        }

        #endregion
    }
}
