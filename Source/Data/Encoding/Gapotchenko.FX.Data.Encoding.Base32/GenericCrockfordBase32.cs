using System;
using System.ComponentModel;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Crockford Base 32 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericCrockfordBase32 : GenericBase32, ICrockfordBase32
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericCrockfordBase32"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericCrockfordBase32(TextDataEncodingAlphabet alphabet) :
            base(alphabet)
        {
        }

        // The size of the main alphabet.
        const int MainAlphabetSize = 32;

        // The 5 last characters of the alphabet allow to encode a checksum value ∈ [0; 37).
        const int ChecksumAlphabetSize = MainAlphabetSize + 5;

        /// <inheritdoc/>
        protected override void ValidateAlphabet(TextDataEncodingAlphabet alphabet)
        {
            int size = alphabet.Size;
            if (size != MainAlphabetSize &&
                size != ChecksumAlphabetSize)
            {
                throw new ArgumentException(
                    string.Format(
                        "The alphabet size of {0} encoding should be {1} or {2}.",
                        Name,
                        ChecksumAlphabetSize,
                        MainAlphabetSize),
                    nameof(alphabet));
            }
        }

        /// <inheritdoc/>
        protected override void ValidateOptions(DataEncodingOptions options)
        {
            base.ValidateOptions(options);

            if ((options & DataEncodingOptions.Checksum) != 0 &&
                Alphabet.Size != ChecksumAlphabetSize)
            {
                throw new ArgumentException(
                    string.Format(
                        "{0} option cannot be used for {1} encoding because it has a restricted alphabet of {2} symbols, not {3}.",
                        nameof(DataEncodingOptions.Checksum),
                        Name,
                        Alphabet.Size,
                        ChecksumAlphabetSize),
                    nameof(options));
            }
        }

        sealed class CrockfordEncoderContext : EncoderContext
        {
            public CrockfordEncoderContext(GenericCrockfordBase32 encoding, TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(encoding, alphabet, options)
            {
            }
        }

        sealed class CrockfordDecoderContext : DecoderContext
        {
            public CrockfordDecoderContext(GenericCrockfordBase32 encoding, TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(encoding, alphabet, options)
            {
                AltSeparator = '-';
            }
        }

        DataEncodingOptions PrepareOptions(DataEncodingOptions options)
        {
            if (PaddingCore == 1)
            {
                options =
                    options & ~DataEncodingOptions.Padding |
                    DataEncodingOptions.Unpad;
            }
            else if ((options & DataEncodingOptions.Padding) == 0)
            {
                // Produce unpadded strings unless padding is explicitly requested.
                options |= DataEncodingOptions.Unpad;
            }

            return options;
        }

        /// <inheritdoc/>
        protected override IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) => new CrockfordEncoderContext(this, alphabet, PrepareOptions(options));

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) => new CrockfordDecoderContext(this, alphabet, PrepareOptions(options));

        /// <inheritdoc/>
        protected override int PaddingCore => 1;
    }
}
