using System;
using System.ComponentModel;
using System.IO;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// The base class for <see cref="IDataEncoding"/> implementations.
    /// </summary>
    public abstract class DataEncoding : IDataEncoding
    {
        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public float Efficiency => EfficiencyCore;

        /// <summary>
        /// Gets the average encoding efficiency.
        /// </summary>
        protected abstract float EfficiencyCore { get; }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public float MaxEfficiency => MaxEfficiencyCore;

        /// <summary>
        /// Gets the maximum encoding efficiency.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual float MaxEfficiencyCore => EfficiencyCore;

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public float MinEfficiency => MinEfficiencyCore;

        /// <summary>
        /// Gets the minimum encoding efficiency.                   
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual float MinEfficiencyCore => EfficiencyCore;

        /// <inheritdoc/>
        public byte[] EncodeData(ReadOnlySpan<byte> data) => EncodeData(data, DataEncodingOptions.None);

        /// <inheritdoc/>
        public byte[] EncodeData(ReadOnlySpan<byte> data, DataEncodingOptions options)
        {
            options = ValidateOptions(options);
            return data == null ? null : EncodeDataCore(data, options);
        }

        /// <summary>
        /// Provides the core implementation of data encoding.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="options">The options.</param>
        /// <returns>The encoded output data.</returns>
        protected abstract byte[] EncodeDataCore(ReadOnlySpan<byte> data, DataEncodingOptions options);

        /// <inheritdoc/>
        public byte[] DecodeData(ReadOnlySpan<byte> data) => DecodeData(data, DataEncodingOptions.None);

        /// <inheritdoc/>
        public byte[] DecodeData(ReadOnlySpan<byte> data, DataEncodingOptions options)
        {
            options = ValidateOptions(options);
            return data == null ? null : DecodeDataCore(data, options);
        }

        /// <summary>
        /// Provides the core implementation of data decoding.
        /// </summary>
        /// <param name="data">The encoded input data.</param>
        /// <param name="options">The options.</param>
        /// <returns>The decoded output data.</returns>
        protected abstract byte[] DecodeDataCore(ReadOnlySpan<byte> data, DataEncodingOptions options);

        /// <summary>
        /// Validates the encoding options and returns effective options to use.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The effective encoding options to use.</returns>
        protected virtual DataEncodingOptions ValidateOptions(DataEncodingOptions options)
        {
            const DataEncodingOptions PaddingConflictMask = DataEncodingOptions.Padding | DataEncodingOptions.Unpad;
            if ((options & PaddingConflictMask) == PaddingConflictMask)
            {
                throw new ArgumentException(
                    string.Format(
                        "'{0}' and '{1}' options cannot be used simultaneously.",
                        nameof(DataEncodingOptions.Padding),
                        nameof(DataEncodingOptions.Unpad)),
                    nameof(options));
            }

            return options;
        }

        /// <inheritdoc/>
        public abstract Stream CreateEncoder(Stream outputStream, DataEncodingOptions options = DataEncodingOptions.None);

        /// <inheritdoc/>
        public abstract Stream CreateDecoder(Stream inputStream, DataEncodingOptions options = DataEncodingOptions.None);

        /// <inheritdoc/>
        public int Padding => PaddingCore;

        /// <summary>
        /// Gets the number of symbols used for padding of an encoded data representation.
        /// </summary>
        protected virtual int PaddingCore => 1;
    }
}
