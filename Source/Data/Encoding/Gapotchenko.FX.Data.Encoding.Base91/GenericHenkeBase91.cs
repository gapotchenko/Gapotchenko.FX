using System;
using System.ComponentModel;
using System.IO;

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
                    string.Format("The alphabet size for {0} encoding should be equal to {1}.", this, Base),
                    nameof(alphabet));
            }
        }

        #region Parameters

        /// <summary>
        /// The base of the encoding.
        /// </summary>
        protected const int Base = 91;

        /// <summary>
        /// Number of symbols per encoded block.
        /// </summary>
        protected const int SymbolsPerEncodedBlock = 2;

        /// <summary>
        /// Maximum number of bits per decoded block.
        /// </summary>
        protected const int MaxBitsPerDecodedBlock = 14;

        /// <summary>
        /// Minimum number of bits per decoded block.
        /// </summary>
        protected const int MinBitsPerDecodedBlock = 13;

        /// <summary>
        /// Maximum efficiency of basE91 encoding.
        /// </summary>
        public new const float MaxEfficiency = (float)MaxBitsPerDecodedBlock / 8 / SymbolsPerEncodedBlock; // = 0.875

        /// <summary>
        /// Typical efficiency of basE91 encoding.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.8132f;

        /// <summary>
        /// Minimum efficiency of basE91 encoding.
        /// </summary>
        public new const float MinEfficiency = (float)MinBitsPerDecodedBlock / 8 / SymbolsPerEncodedBlock; // = 0.8125

        #endregion

        /// <inheritdoc/>
        public int Radix => Base;

        /// <inheritdoc/>
        protected override float MaxEfficiencyCore => MaxEfficiency;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <inheritdoc/>
        protected override float MinEfficiencyCore => MinEfficiency;

        abstract class CodecContextBase
        {
            public CodecContextBase(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
            {
                m_Alphabet = alphabet;
                m_Options = options;
            }

            protected readonly TextDataEncodingAlphabet m_Alphabet;
            protected readonly DataEncodingOptions m_Options;

            #region Parameters

            protected const string Name = "Henke Base91";

            #endregion

            protected uint m_Bits;
            protected int m_Modulus;
            protected bool m_Eof;
        }

        sealed class EncoderContext : CodecContextBase, IEncoderContext
        {
            public EncoderContext(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
            }

            public void Encode(ReadOnlySpan<byte> input, TextWriter output)
            {
                if (m_Eof)
                    return;

                var alphabet = m_Alphabet;

                foreach (var b in input)
                {
                    // Accumulate data bits.
                    m_Bits = (m_Bits << 8) | b;
                    m_Modulus += 8;

                    if (m_Modulus > 13)
                    {
                        m_Modulus = 0;

                        //uint a = m_Bits;
                        //for (int i = SymbolsPerEncodedBlock - 1; i >= 0; --i)
                        //{
                        //    var si = (int)(a % Base);
                        //    a /= Base;

                        //    m_Buffer[i] = alphabet[si];
                        //}

                        //EmitLineBreak(output);
                        //output.Write(m_Buffer);

                        //MoveLinePosition(SymbolsPerEncodedBlock);
                    }
                }

            }
        }

        /// <inheritdoc/>
        protected sealed override IEncoderContext CreateEncoderContext(DataEncodingOptions options) => CreateEncoderContextCore(Alphabet, options);

        /// <inheritdoc/>
        protected sealed override IDecoderContext CreateDecoderContext(DataEncodingOptions options) => CreateDecoderContextCore(Alphabet, options);

        /// <summary>
        /// Creates encoder context with specified alphabet and options.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="options">The options.</param>
        /// <returns>The encoder context.</returns>
        protected virtual IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) => new EncoderContext(alphabet, options);

        /// <summary>
        /// Creates decoder context with specified alphabet and options.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        /// <param name="options">The options.</param>
        /// <returns>The decoder context.</returns>
        protected virtual IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) => throw new NotImplementedException(); // new DecoderContext(alphabet, options);

        /// <inheritdoc/>
        public override bool IsCaseSensitive => Alphabet.IsCaseSensitive;

        /// <inheritdoc/>
        public override bool CanCanonicalize => Alphabet.IsCanonicalizable;

        /// <inheritdoc/>
        protected override void CanonicalizeCore(ReadOnlySpan<char> source, Span<char> destination) => Alphabet.Canonicalize(source, destination);

        /// <inheritdoc/>
        protected override int GetMaxCharCountCore(int byteCount, DataEncodingOptions options) => (int)Math.Ceiling(byteCount / MinEfficiency);

        /// <inheritdoc/>
        protected override int GetMaxByteCountCore(int charCount, DataEncodingOptions options) => (int)Math.Ceiling(charCount * MaxEfficiency);
    }
}
