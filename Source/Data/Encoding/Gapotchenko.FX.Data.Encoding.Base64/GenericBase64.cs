using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Base64 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericBase64 : DataTextEncoding, IBase64
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase64"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase64(in DataTextEncodingAlphabet alphabet)
        {
            if (alphabet.Size != 64)
                throw new ArgumentException("The alphabet size of a Base64 encoding should be 64.", nameof(alphabet));

            Alphabet = alphabet;
        }

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly DataTextEncodingAlphabet Alphabet;

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.75f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <inheritdoc/>
        public override bool IsCaseSensitive => true;

        /// <inheritdoc/>
        protected override int PaddingCore => 3;

        /// <summary>
        /// The padding character.
        /// </summary>
        protected const char PaddingChar = '=';

        /// <inheritdoc/>
        protected override string PadCore(ReadOnlySpan<char> s) => PadRight(s, PaddingChar);

        /// <inheritdoc/>
        protected override ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => UnpadRight(s, PaddingChar);
    }
}
