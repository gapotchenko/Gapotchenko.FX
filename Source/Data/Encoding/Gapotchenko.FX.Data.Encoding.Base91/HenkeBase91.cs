using System;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Implements Jochaim Henke's Base91 (basE91) encoding.
    /// </summary>
    public sealed class HenkeBase91 : TextDataEncoding, IBase91
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HenkeBase91"/> class.
        /// </summary>
        public HenkeBase91()
        {
        }

        /// <inheritdoc/>
        public int Radix => 91;

        /// <inheritdoc/>
        public override string Name => "basE91";

        /// <inheritdoc/>
        protected override float MaxEfficiencyCore => 0.875f;

        /// <summary>
        /// Average efficiency of basE91 encoding.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.8132f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <inheritdoc/>
        protected override float MinEfficiencyCore => 0.8125f;

        /// <inheritdoc/>
        protected override IEncoderContext CreateEncoderContext(DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContext(DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsCaseSensitive => true;

        /// <inheritdoc/>
        protected override void CanonicalizeCore(ReadOnlySpan<char> source, Span<char> destination)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override int GetMaxCharCountCore(int byteCount, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override int GetMaxByteCountCore(int charCount, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
