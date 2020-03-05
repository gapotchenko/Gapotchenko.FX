using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Base32 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericBase58 : TextDataEncoding, IBase58
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase58"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase58(TextDataEncodingAlphabet alphabet) :
            base(BytesPerDecodedBlock, SymbolsPerEncodedBlock)
        {
            if (alphabet == null)
                throw new ArgumentNullException(nameof(alphabet));

            ValidateAlphabet(alphabet);

            Alphabet = alphabet;
        }

        /// <summary>
        /// Validates alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected virtual void ValidateAlphabet(TextDataEncodingAlphabet alphabet)
        {
            if (alphabet.Size != Base)
            {
                throw new ArgumentException(
                    string.Format("The alphabet size of {0} encoding should be {1}.", Name, Base),
                    nameof(alphabet));
            }
        }

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly TextDataEncodingAlphabet Alphabet;

        #region Parameters

        /// <summary>
        /// Number of symbols per encoded block.
        /// </summary>
        protected const int SymbolsPerEncodedBlock = 34;

        /// <summary>
        /// Number of bytes per decoded block.
        /// </summary>
        protected const int BytesPerDecodedBlock = 12;

        /// <summary>
        /// The base of the encoding.
        /// </summary>
        protected const int Base = 58;

        #endregion

        /// <inheritdoc/>
        public int Radix => Base;

        /// <summary>
        /// Base58 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 4.060443010546419336600504153820088f / 5.545177444479562475337856971665413f; // log(Base) / log(256) = log(58) / log(256) = 0.7322476

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;
    }
}
