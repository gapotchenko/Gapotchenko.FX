using System;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Implements Jochaim Henke's Base91 (basE91) encoding.
    /// </summary>
    public sealed class HenkeBase91 : DataTextEncoding, IBase91
    {
        /// <inheritdoc/>
        public override string Name => "basE91";

        /// <inheritdoc/>
        protected override float MaxEfficiencyCore => 0.875f;

        /// <summary>
        /// Base91 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.8132f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <inheritdoc/>
        protected override float MinEfficiencyCore => 0.8125f;

        /// <inheritdoc/>
        protected override byte[] GetBytesCore(ReadOnlySpan<char> s, DataTextEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override string GetStringCore(ReadOnlySpan<byte> data, DataTextEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsCaseSensitive => true;
    }
}
