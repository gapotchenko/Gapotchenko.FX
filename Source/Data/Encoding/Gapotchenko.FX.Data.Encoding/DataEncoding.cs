using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        /// Provides the core implementation of encoding efficiency retrieval.
        /// </summary>
        protected abstract float EfficiencyCore { get; }

        /// <inheritdoc/>
        public byte[] EncodeData(byte[] data) => data == null ? null : EncodeDataCore(data);

        /// <summary>
        /// Provides the core implementation of data encoding.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <returns>The encoded output data.</returns>
        protected abstract byte[] EncodeDataCore(byte[] data);

        /// <inheritdoc/>
        public byte[] DecodeData(byte[] data) => data == null ? null : DecodeDataCore(data);

        /// <summary>
        /// Provides the core implementation of data decoding.
        /// </summary>
        /// <param name="data">The encoded input data.</param>
        /// <returns>The decoded output data.</returns>
        protected abstract byte[] DecodeDataCore(byte[] data);
    }
}
