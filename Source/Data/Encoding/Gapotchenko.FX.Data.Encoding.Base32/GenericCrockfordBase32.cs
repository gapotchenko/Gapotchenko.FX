using System;
using System.ComponentModel;
using System.Numerics;
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
        protected override DataEncodingOptions ValidateOptions(DataEncodingOptions options)
        {
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

            if ((options & DataEncodingOptions.Padding) == 0)
            {
                // Produce unpadded strings unless padding is explicitly requested.
                options |= DataEncodingOptions.Unpad;
            }

            return base.ValidateOptions(options);
        }

        string Epilogue(string s, DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Unpad) == 0)
                s = Pad(s.AsSpan());
            return s;
        }

        StringBuilder Epilogue(StringBuilder sb, DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Unpad) == 0)
            {
                int padding = Padding;
                while (sb.Length % padding != 0)
                    sb.Append(PaddingChar);
            }

            return sb;
        }

        string GetZeroString(DataEncodingOptions options) =>
            Epilogue(
                (options & DataEncodingOptions.Checksum) != 0 ?
                    "00" :
                    "0",
                options);

        /// <inheritdoc/>
        public string GetString(int value) => GetString(value, DataEncodingOptions.None);

        /// <inheritdoc/>
        public string GetString(int value, DataEncodingOptions options)
        {
            options = ValidateOptions(options);

            if (value == 0)
                return GetZeroString(options);

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

            return Epilogue(sb, options).ToString();
        }

        const char Separator = '-';

        static bool IsValidSeparator(char c) =>
            char.IsWhiteSpace(c) ||
            c == Separator;

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

        /// <inheritdoc/>
        public bool TryGetInt32(ReadOnlySpan<char> s, out int value) => TryGetInt32(s, out value, DataEncodingOptions.None);

        /// <inheritdoc/>
        public bool TryGetInt32(ReadOnlySpan<char> s, out int value, DataEncodingOptions options)
        {
            value = 0;

            options = ValidateOptions(options);

            if (s.IsEmpty)
                return false;

            const int BitsPerValue = sizeof(int) * 8;

            bool checksum = (options & DataEncodingOptions.Checksum) != 0;

            int bits = 0; // accumulated bits
            int bitCount = 0; // count of accumulated bits
            int psi = -1; // previous symbol index

            var alphabet = Alphabet;

            foreach (var c in s)
            {
                if (c == PaddingChar)
                    continue;

                int si = alphabet.IndexOf(c); // symbol index lookup
                if (si == -1)
                {
                    if ((options & DataEncodingOptions.Relax) == 0)
                    {
                        if (!IsValidSeparator(c))
                            return false;
                    }
                    continue;
                }

                byte b;
                if (checksum)
                {
                    // Feed the decoder with one symbol delay.
                    if (psi == -1)
                    {
                        psi = si;

                        // No previous symbol to feed.
                        continue;
                    }
                    else
                    {
                        // Feed the previous symbol to the decoder.
                        b = (byte)psi;

                        // The last psi would contain a checksum.
                        psi = si;
                    }
                }
                else
                {
                    if (si >= MainAlphabetSize)
                        return false; // Cannot contain a symbol from the checksum alphabet when checksum is not used.

                    // Feed the symbol directly to the decoder.
                    b = (byte)si;
                }

                if (bitCount >= BitsPerValue)
                {
                    // The accumulated value is too large.
                    return false;
                }

                // Accumulate symbol bits.
                bits = (bits << BitsPerSymbol) | b;
                bitCount += BitsPerSymbol;
            }

            if (checksum)
            {
                if (psi == -1)
                    return false; // No checksum symbol was read.

                // Verify the checksum.
                if (bits % ChecksumAlphabetSize != psi)
                    return false;
            }

            value = bits;
            return true;
        }

        /// <inheritdoc/>
        public string GetString(long value) => GetString(value, DataEncodingOptions.None);

        /// <inheritdoc/>
        public string GetString(long value, DataEncodingOptions options)
        {
            options = ValidateOptions(options);

            if (value == 0)
                return GetZeroString(options);

            ulong bits = (ulong)value;

            const int BitsPerValue = sizeof(long) * 8;
            const int InsignificantBits = BitsPerValue - BitsPerSymbol;
            const int RestBits = BitsPerValue / BitsPerSymbol * BitsPerSymbol;
            const int FirstBits = BitsPerValue - RestBits;
            const ulong StopBit = 1UL << RestBits;
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
                sb.Append(alphabet[(int)((ulong)value % ChecksumAlphabetSize)]);

            return Epilogue(sb, options).ToString();
        }

        /// <inheritdoc/>
        public long GetInt64(ReadOnlySpan<char> s) => GetInt64(s, DataEncodingOptions.None);

        /// <inheritdoc/>
        public long GetInt64(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            if (!TryGetInt64(s, out var value, options))
                throw new FormatException("Input string was not in a correct format.");

            return value;
        }

        /// <inheritdoc/>
        public bool TryGetInt64(ReadOnlySpan<char> s, out long value) => TryGetInt64(s, out value, DataEncodingOptions.None);

        /// <inheritdoc/>
        public bool TryGetInt64(ReadOnlySpan<char> s, out long value, DataEncodingOptions options)
        {
            value = 0;

            options = ValidateOptions(options);

            if (s.IsEmpty)
                return false;

            const int BitsPerValue = sizeof(long) * 8;

            bool checksum = (options & DataEncodingOptions.Checksum) != 0;

            ulong bits = 0; // accumulated bits
            int bitCount = 0; // count of accumulated bits
            int psi = -1; // previous symbol index

            var alphabet = Alphabet;

            foreach (var c in s)
            {
                if (c == PaddingChar)
                    continue;

                int si = alphabet.IndexOf(c); // symbol index lookup
                if (si == -1)
                {
                    if ((options & DataEncodingOptions.Relax) == 0)
                    {
                        if (!IsValidSeparator(c))
                            return false;
                    }
                    continue;
                }

                byte b;
                if (checksum)
                {
                    // Feed the decoder with one symbol delay.
                    if (psi == -1)
                    {
                        psi = si;

                        // No previous symbol to feed.
                        continue;
                    }
                    else
                    {
                        // Feed the previous symbol to the decoder.
                        b = (byte)psi;

                        // The last psi would contain a checksum.
                        psi = si;
                    }
                }
                else
                {
                    if (si >= MainAlphabetSize)
                        return false; // Cannot contain a symbol from the checksum alphabet when checksum is not used.

                    // Feed the symbol directly to the decoder.
                    b = (byte)si;
                }

                if (bitCount >= BitsPerValue)
                {
                    // The accumulated value is too large.
                    return false;
                }

                // Accumulate symbol bits.
                bits = (bits << BitsPerSymbol) | b;
                bitCount += BitsPerSymbol;
            }

            if (checksum)
            {
                if (psi == -1)
                    return false; // No checksum symbol was read.

                // Verify the checksum.
                if ((int)(bits % ChecksumAlphabetSize) != psi)
                    return false;
            }

            value = (long)bits;
            return true;
        }

        /// <inheritdoc/>
        public string GetString(BigInteger value) => GetString(value, DataEncodingOptions.None);

        /// <inheritdoc/>
        public string GetString(BigInteger value, DataEncodingOptions options)
        {
            options = ValidateOptions(options);

            if (value.IsZero)
                return GetZeroString(options);

            var bytes = value.ToByteArray();

            int totalBytes = bytes.Length;
            int totalBits = totalBytes << 3;
            int restBits = totalBits / BitsPerSymbol * BitsPerSymbol;
            int capacity = (totalBits + BitsPerSymbol - 1) / BitsPerSymbol + 1 /* checksum */;

            var sb = new StringBuilder(capacity);

            var alphabet = Alphabet;

            for (int bit = restBits; bit >= 0; bit -= BitsPerSymbol)
            {
                int bs = bit & 7; // bit shift
                int bi = bit >> 3; // byte index

                int ab = 8 - bs; // available bits 

                int si; // symbol index
                if (ab >= BitsPerSymbol)
                {
                    // Byte has enough bits for a symbol.
                    si = (bytes[bi] >> bs) & SymbolMask;
                }
                else
                {
                    // Byte contains only lower bits of a symbol.
                    si = bytes[bi] >> bs;

                    // Try get the rest (higher bits) from the next byte.
                    int nbi = bi + 1; // next byte index
                    if (nbi < totalBytes)
                        si |= (bytes[nbi] << ab) & SymbolMask;
                }

                if (si == 0 && sb.Length == 0)
                {
                    // Skip leading zeros.
                    continue;
                }

                sb.Append(alphabet[si]);
            }

            if ((options & DataEncodingOptions.Checksum) != 0)
                sb.Append(alphabet[(int)(value % ChecksumAlphabetSize)]);

            return Epilogue(sb, options).ToString();
        }

        /// <inheritdoc/>
        public BigInteger GetBigInteger(ReadOnlySpan<char> s) => GetBigInteger(s, DataEncodingOptions.None);

        /// <inheritdoc/>
        public BigInteger GetBigInteger(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            if (!TryGetBigInteger(s, out var value, options))
                throw new FormatException("Input string was not in a correct format.");

            return value;
        }

        /// <inheritdoc/>
        public bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value) => TryGetBigInteger(s, out value, DataEncodingOptions.None);

        /// <inheritdoc/>
        public bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value, DataEncodingOptions options)
        {
            value = 0;

            options = ValidateOptions(options);

            if (s.IsEmpty)
                return false;

            bool checksum = (options & DataEncodingOptions.Checksum) != 0;

            BigInteger bits = 0; // accumulated bits
            int psi = -1; // previous symbol index

            var alphabet = Alphabet;

            foreach (var c in s)
            {
                if (c == PaddingChar)
                    continue;

                int si = alphabet.IndexOf(c); // symbol index lookup
                if (si == -1)
                {
                    if ((options & DataEncodingOptions.Relax) == 0)
                    {
                        if (!IsValidSeparator(c))
                            return false;
                    }
                    continue;
                }

                byte b;
                if (checksum)
                {
                    // Feed the decoder with one symbol delay.
                    if (psi == -1)
                    {
                        psi = si;

                        // No previous symbol to feed.
                        continue;
                    }
                    else
                    {
                        // Feed the previous symbol to the decoder.
                        b = (byte)psi;

                        // The last psi would contain a checksum.
                        psi = si;
                    }
                }
                else
                {
                    if (si >= MainAlphabetSize)
                        return false; // Cannot contain a symbol from the checksum alphabet when checksum is not used.

                    // Feed the symbol directly to the decoder.
                    b = (byte)si;
                }

                // Accumulate symbol bits.
                bits = (bits << BitsPerSymbol) | b;
            }

            if (checksum)
            {
                if (psi == -1)
                    return false; // No checksum symbol was read.

                // Verify the checksum.
                if (bits % ChecksumAlphabetSize != psi)
                    return false;
            }

            value = bits;
            return true;
        }

        void ValidateCodecOptions(DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Checksum) != 0)
            {
                throw new NotSupportedException(
                    string.Format(
                        "{0} encoding does not provide checksum operations over arbitrary data blocks.",
                        Name));
            }
        }

        /// <inheritdoc/>
        protected override IEncoderContext CreateEncoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
        {
            ValidateCodecOptions(options);

            return base.CreateEncoderContextCore(alphabet, options);
        }

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContextCore(TextDataEncodingAlphabet alphabet, DataEncodingOptions options)
        {
            ValidateCodecOptions(options);

            return new DecoderContext(this, alphabet, options)
            {
                Separator = Separator
            };
        }
    }
}
