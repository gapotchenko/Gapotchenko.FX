using System;
using System.ComponentModel;
using System.IO;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines interface of a data encoding.
    /// </summary>
    public interface IDataEncoding
    {
        /// <summary>
        /// Gets the average encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        float Efficiency { get; }

        /// <summary>
        /// Gets the maximum encoding efficiency.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        float MaxEfficiency { get; }

        /// <summary>
        /// Gets the minimum encoding efficiency.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        float MinEfficiency { get; }

        /// <summary>
        /// Encodes the data.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <returns>The encoded output data.</returns>
        byte[] EncodeData(ReadOnlySpan<byte> data);

        /// <summary>
        /// Encodes the data with specified options.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="options">The options.</param>
        /// <returns>The encoded output data.</returns>
        byte[] EncodeData(ReadOnlySpan<byte> data, DataEncodingOptions options);

        /// <summary>
        /// Decodes the data.
        /// </summary>
        /// <param name="data">The encoded input data.</param>
        /// <returns>The decoded output data.</returns>
        byte[] DecodeData(ReadOnlySpan<byte> data);

        /// <summary>
        /// Decodes the data with specified options.
        /// </summary>
        /// <param name="data">The encoded input data.</param>
        /// <param name="options">The options.</param>
        /// <returns>The decoded output data.</returns>
        byte[] DecodeData(ReadOnlySpan<byte> data, DataEncodingOptions options);

        /// <summary>
        /// <para>
        /// Gets a value indicating whether the current encoding supports streaming operations.
        /// </para>
        /// <para>
        /// Streaming operations are represented by <see cref="CreateEncoder(Stream, DataEncodingOptions)"/> and <see cref="CreateDecoder(Stream, DataEncodingOptions)"/> methods.\
        /// </para>
        /// </summary>
        bool CanStream { get; }

        /// <summary>
        /// Creates a streaming encoder.
        /// </summary>
        /// <param name="outputStream">The output stream of an encoder.</param>
        /// <param name="options">The options.</param>
        /// <returns>The stream.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="outputStream"/> argument is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">The encoding does not support streaming operations.</exception>
        Stream CreateEncoder(Stream outputStream, DataEncodingOptions options = DataEncodingOptions.None);

        /// <summary>
        /// Creates a streaming decoder.
        /// </summary>
        /// <param name="inputStream">The input stream of a decoder.</param>
        /// <param name="options">The options.</param>
        /// <returns>The stream.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="inputStream"/> argument is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">The encoding does not support streaming operations.</exception>
        Stream CreateDecoder(Stream inputStream, DataEncodingOptions options = DataEncodingOptions.None);

        /// <summary>
        /// Gets a value indicating whether the current encoding supports padding.
        /// </summary>
        bool CanPad { get; }

        /// <summary>
        /// Gets the number of symbols for padding of the encoded data representation.
        /// </summary>
        int Padding { get; }

        /// <summary>
        /// Gets a value indicating whether the current encoding prefers to produce the padded output.
        /// </summary>
        bool PrefersPadding { get; }
    }
}
