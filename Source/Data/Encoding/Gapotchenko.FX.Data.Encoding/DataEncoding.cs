using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

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
            ValidateOptions(options);
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
            ValidateOptions(options);
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
        /// Validates encoding options.
        /// </summary>
        /// <param name="options">The options.</param>
        protected virtual void ValidateOptions(DataEncodingOptions options)
        {
            const DataEncodingOptions PaddingMask = DataEncodingOptions.Padding | DataEncodingOptions.Unpad;
            if ((options & PaddingMask) == PaddingMask)
            {
                throw new ArgumentException(
                    string.Format(
                        "'{0}' and '{1}' options cannot be specified simultaneously.",
                        nameof(DataEncodingOptions.Padding),
                        nameof(DataEncodingOptions.Unpad)),
                    nameof(options));
            }
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
