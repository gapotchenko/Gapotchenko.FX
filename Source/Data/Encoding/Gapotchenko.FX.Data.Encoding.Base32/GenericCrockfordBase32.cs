using System;
using System.ComponentModel;
using System.IO;
using System.Text;

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
                        "'{0}' option cannot be used for {1} encoding because it has a restricted alphabet of {2} symbols, not {3}.",
                        nameof(DataEncodingOptions.Checksum),
                        Name,
                        Alphabet.Size,
                        ChecksumAlphabetSize),
                    nameof(options));
            }
        }

        /// <inheritdoc/>
        public string GetString(int value) => GetString(value, DataEncodingOptions.None);

        /// <inheritdoc/>
        public string GetString(int value, DataEncodingOptions options)
        {
            if (value == 0)
                return "0";

            ValidateOptions(options);

            uint bits = (uint)value;

            const int BitsPerValue = sizeof(int) * 8;
            const int InsignificantBits = BitsPerValue - BitsPerSymbol;
            const int RestBits = BitsPerValue / BitsPerSymbol * BitsPerSymbol;
            const int FirstBits = BitsPerValue - RestBits;
            const uint StopBit = 1U << RestBits;
            const int Capacity = (BitsPerValue + BitsPerSymbol - 1) / BitsPerSymbol + 1 /* checksum */;

            var sb = new StringBuilder(Capacity);

            var firstBits = (int)(bits >> RestBits);
            bits = (bits << FirstBits) | 0b1;

            var alphabet = Alphabet;

            if (firstBits != 0)
            {
                sb.Append(alphabet[firstBits]);
            }
            else
            {
                // Skip leading zeros.
                while (bits >> InsignificantBits == 0)
                    bits <<= BitsPerSymbol;
            }

            while (bits != StopBit)
            {
                sb.Append(alphabet[(int)(bits >> InsignificantBits)]);
                bits <<= BitsPerSymbol;
            }

            if ((options & DataEncodingOptions.Checksum) != 0)
                sb.Append(alphabet[(int)((uint)value % ChecksumAlphabetSize)]);

            return sb.ToString();
        }

        static bool IsValidSeparator(char c) =>
            char.IsWhiteSpace(c) ||
            c == '-';

        /// <inheritdoc/>
        public int GetInt32(ReadOnlySpan<char> s) => GetInt32(s, DataEncodingOptions.None);

        /// <inheritdoc/>
        public int GetInt32(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            if (!TryGetInt32(s, out var value, options))
                throw new FormatException("Input string was not in a correct format.");

            return value;
        }

        bool TryGetInt32(ReadOnlySpan<char> s, out int value, DataEncodingOptions options)
        {
            value = 0;

            ValidateOptions(options);

            if (s.IsEmpty)
                return false;

            int bits = 0;

            var alphabet = Alphabet;

            foreach (var c in s)
            {
                if (c == PaddingChar)
                    continue;

                int b = alphabet.IndexOf(c);
                if (b == -1)
                {
                    if ((options & DataEncodingOptions.Relax) == 0)
                    {
                        if (!IsValidSeparator(c))
                            return false;
                    }
                    continue;
                }

                bits = (bits << BitsPerSymbol) | b;
            }

            value = bits;
            return true;
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

        DataEncodingOptions PrepareCodecOptions(DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Checksum) != 0)
            {
                throw new NotSupportedException(
                    string.Format(
                        "{0} encoding does not provide checksum operations over arbitrary data blocks.",
                        Name));
            }

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
        protected override IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) => new CrockfordEncoderContext(this, alphabet, PrepareCodecOptions(options));

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options) => new CrockfordDecoderContext(this, alphabet, PrepareCodecOptions(options));

        /// <inheritdoc/>
        protected override int PaddingCore => 1;
    }
}
