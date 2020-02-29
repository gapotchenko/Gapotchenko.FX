using System;
using System.ComponentModel;
using System.IO;

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

        /// <summary>
        /// Iterative modulo calculator for 'x mod 37' checksum.
        /// </summary>
        sealed class Checksum
        {
            /// <summary>
            /// Checksum divisor.
            /// </summary>
            const int Divisor = 37;

            /// <summary>
            /// Bits per checksum symbol.
            /// </summary>
            const int BitsPerSymbol = 8;

            /// <summary>
            /// Checksum symbol radix.
            /// </summary>
            const int Radix = 1 << 8;

            /// <summary>
            /// Checksum accumulator.
            /// </summary>
            int m_Accumulator;

            /// <summary>
            /// Checksum multiplier.
            /// </summary>
            int m_Multiplier = 1;

            /// <summary>
            /// Modulo recalculation limit for multiplier.
            /// </summary>
            const int m_MultiplierLimit = int.MaxValue / Radix;

            public void WriteSymbol(byte symbol)
            {
                // Use the fact that
                //
                //   c mod m = (a ⋅ b) mod m
                //
                // is equivalent to
                //
                //   c mod m = [(a mod m) ⋅ (b mod m)] mod m
                //
                // in order to avoid the arithmetic overflow during the iterative modulo calculation.

                if (m_Multiplier >= m_MultiplierLimit)
                {
                    // Crop multiplier only when there is a risk of an overflow in order to minimize the amount of expensive modulo calculations.
                    m_Multiplier %= Divisor;
                }

                int a = symbol * m_Multiplier;
                if (int.MaxValue - m_Accumulator < a)
                {
                    // Crop accumulator only when there is a risk of an overflow in order to minimize the amount of expensive modulo calculations.
                    m_Accumulator %= Divisor;
                }
                m_Accumulator += a;

                m_Multiplier <<= BitsPerSymbol;
            }

            /// <summary>
            /// Gets checksum value.
            /// </summary>
            public int Value => m_Accumulator % Divisor;
        }

        sealed class CrockfordEncoderContext : EncoderContext
        {
            public CrockfordEncoderContext(GenericCrockfordBase32 encoding, TextDataEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(encoding, alphabet, options)
            {
                if ((options & DataEncodingOptions.Checksum) != 0)
                    m_Checksum = new Checksum();
            }

            Checksum m_Checksum;

            public override void Encode(ReadOnlySpan<byte> input, TextWriter output)
            {
                base.Encode(input, output);

                if (m_Checksum != null)
                {
                    if (input == null)
                    {
                        // Write checksum.
                        output.Write(m_Alphabet[m_Checksum.Value]);
                        m_Checksum = null;
                    }
                    else
                    {
                        // Calculate checksum.
                        foreach (var b in input)
                            m_Checksum.WriteSymbol(b);
                    }
                }
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
