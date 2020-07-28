using System;
using System.ComponentModel;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Jochaim Henke's Base91 (basE91) encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericHenkeBase91 : TextDataEncoding, IBase91
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericHenkeBase91"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericHenkeBase91(TextDataEncodingAlphabet alphabet)
        {
            if (alphabet == null)
                throw new ArgumentNullException(nameof(alphabet));

            ValidateAlphabet(alphabet);

            Alphabet = alphabet;
        }

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly TextDataEncodingAlphabet Alphabet;

        /// <summary>
        /// Validates alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected virtual void ValidateAlphabet(TextDataEncodingAlphabet alphabet)
        {
            if (alphabet.Size != Base)
            {
                throw new ArgumentException(
                    string.Format("The alphabet size of {0} encoding should be {1}.", this, Base),
                    nameof(alphabet));
            }
        }

        #region Parameters

        /// <summary>
        /// The base of the encoding.
        /// </summary>
        protected const int Base = 91;

        /// <summary>
        /// Maximum efficiency of basE91 encoding.
        /// </summary>
        public new const float MaxEfficiency = 0.875f;

        /// <summary>
        /// Average efficiency of basE91 encoding.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.8132f;

        /// <summary>
        /// Minimum efficiency of basE91 encoding.
        /// </summary>
        public new const float MinEfficiency = 0.8125f;

        #endregion

        /// <inheritdoc/>
        public int Radix => Base;

        /// <inheritdoc/>
        protected override float MaxEfficiencyCore => MaxEfficiency;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <inheritdoc/>
        protected override float MinEfficiencyCore => MinEfficiency;

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
        public override bool IsCaseSensitive => Alphabet.IsCaseSensitive;

        /// <inheritdoc/>
        public override bool CanCanonicalize => Alphabet.IsCanonicalizable;

        /// <inheritdoc/>
        protected override void CanonicalizeCore(ReadOnlySpan<char> source, Span<char> destination) => Alphabet.Canonicalize(source, destination);

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
