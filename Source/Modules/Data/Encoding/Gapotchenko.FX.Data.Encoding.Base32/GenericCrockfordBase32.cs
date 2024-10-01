using Gapotchenko.FX.Text;
using System.Numerics;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides a generic implementation of Crockford Base 32 encoding.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GenericCrockfordBase32 : GenericBase32, ICrockfordBase32
{
    private protected GenericCrockfordBase32(TextDataEncodingAlphabet alphabet, char paddingChar) :
        base(alphabet, paddingChar)
    {
    }

    // The size of the main alphabet.
    const int MainAlphabetSize = 32;

    // The 5 last characters of the alphabet allow to encode a checksum value ∈ [0; 37).
    const int ChecksumAlphabetSize = MainAlphabetSize + 5;

    /// <inheritdoc/>
    protected override void ValidateAlphabet(TextDataEncodingAlphabet alphabet)
    {
        if (alphabet is null)
            throw new ArgumentNullException(nameof(alphabet));

        int size = alphabet.Size;
        if (size != MainAlphabetSize &&
            size != ChecksumAlphabetSize)
        {
            throw new ArgumentException(
                string.Format(
                    "The alphabet size of {0} encoding should be {1} or {2}.",
                    this,
                    ChecksumAlphabetSize,
                    MainAlphabetSize),
                nameof(alphabet));
        }
    }

    /// <inheritdoc/>
    protected override DataEncodingOptions GetEffectiveOptions(DataEncodingOptions options)
    {
        if ((options & DataEncodingOptions.Checksum) != 0 &&
            Alphabet.Size != ChecksumAlphabetSize)
        {
            throw new ArgumentException(
                string.Format(
                    "'{0}' option cannot be used for {1} encoding because it has a restricted alphabet of {2} symbols, not {3}.",
                    nameof(DataEncodingOptions.Checksum),
                    this,
                    Alphabet.Size,
                    ChecksumAlphabetSize),
                nameof(options));
        }

        if ((options & DataEncodingOptions.Padding) == 0)
        {
            // Produce unpadded strings unless padding is explicitly requested.
            options |= DataEncodingOptions.NoPadding;
        }
        else if ((options & DataEncodingOptions.Checksum) != 0)
        {
            throw new ArgumentException(
                string.Format(
                    "'{0}' and '{1}' options cannot be used simultaneously for {2} encoding.",
                    nameof(DataEncodingOptions.Padding),
                    nameof(DataEncodingOptions.Checksum),
                    this),
                nameof(options));
        }

        return base.GetEffectiveOptions(options);
    }

    string Epilogue(string s, DataEncodingOptions options)
    {
        if ((options & DataEncodingOptions.NoPadding) == 0)
            s = Pad(s.AsSpan());
        return s;
    }

    StringBuilder Epilogue(StringBuilder sb, DataEncodingOptions options)
    {
        if ((options & DataEncodingOptions.NoPadding) == 0)
        {
            int padding = Padding;
            while (sb.Length % padding != 0)
                sb.Append(PaddingChar);
        }

        return sb;
    }

    const char Separator = '-';

    static bool IsValidSeparator(char c) =>
        char.IsWhiteSpace(c) ||
        c == Separator;

    string GetZeroString(DataEncodingOptions options) =>
        Epilogue(
            (options & DataEncodingOptions.Checksum) != 0 ?
                "00" :
                "0",
            options);

    #region Int32

    /// <inheritdoc/>
    public string GetString(int value) => GetString(value, DataEncodingOptions.None);

    /// <inheritdoc/>
    public string GetString(int value, DataEncodingOptions options)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");

        return GetString((uint)value, options);
    }

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
        if (TryGetUInt32(s, out var x, options) && x <= int.MaxValue)
        {
            value = (int)x;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    #endregion

    #region UInt32

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public string GetString(uint value) => GetString(value, DataEncodingOptions.None);

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public string GetString(uint value, DataEncodingOptions options)
    {
        options = GetEffectiveOptions(options);

        if (value == 0)
            return GetZeroString(options);

        uint bits = value;

        const int BitsPerValue = sizeof(uint) * 8;
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
            sb.Append(alphabet[(int)(value % ChecksumAlphabetSize)]);

        return Epilogue(sb, options).ToString();
    }

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public uint GetUInt32(ReadOnlySpan<char> s) => GetUInt32(s, DataEncodingOptions.None);

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public uint GetUInt32(ReadOnlySpan<char> s, DataEncodingOptions options)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        if (!TryGetUInt32(s, out var value, options))
            throw new FormatException("Input string was not in a correct format.");

        return value;
    }

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public bool TryGetUInt32(ReadOnlySpan<char> s, out uint value) => TryGetUInt32(s, out value, DataEncodingOptions.None);

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public bool TryGetUInt32(ReadOnlySpan<char> s, out uint value, DataEncodingOptions options)
    {
        value = 0;

        options = GetEffectiveOptions(options);

        if (s.IsEmpty)
            return false;

        const int BitsPerValue = sizeof(uint) * 8;

        bool checksum = (options & DataEncodingOptions.Checksum) != 0;

        uint bits = 0; // accumulated bits
        int bitCount = 0; // count of accumulated bits
        int psi = -1; // previous symbol index

        var alphabet = Alphabet;
        bool isCaseSensitive = alphabet.IsCaseSensitive;
        var paddingChar = PaddingChar;
        bool relax = (options & DataEncodingOptions.Relax) != 0;

        foreach (var c in s)
        {
            if (!checksum && CharEqual(c, paddingChar, isCaseSensitive))
                continue;

            int si = alphabet.IndexOf(c); // symbol index lookup
            if (si == -1)
            {
                if (!relax)
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

    #endregion

    #region Int64

    /// <inheritdoc/>
    public string GetString(long value) => GetString(value, DataEncodingOptions.None);

    /// <inheritdoc/>
    public string GetString(long value, DataEncodingOptions options)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");

        return GetString((ulong)value, options);
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
        if (TryGetUInt64(s, out var x, options) && x <= long.MaxValue)
        {
            value = (long)x;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    #endregion

    #region UInt64

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public string GetString(ulong value) => GetString(value, DataEncodingOptions.None);

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public string GetString(ulong value, DataEncodingOptions options)
    {
        options = GetEffectiveOptions(options);

        if (value == 0)
            return GetZeroString(options);

        ulong bits = value;

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
            sb.Append(alphabet[(int)(value % ChecksumAlphabetSize)]);

        return Epilogue(sb, options).ToString();
    }

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public ulong GetUInt64(ReadOnlySpan<char> s) => GetUInt64(s, DataEncodingOptions.None);

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public ulong GetUInt64(ReadOnlySpan<char> s, DataEncodingOptions options)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        if (!TryGetUInt64(s, out var value, options))
            throw new FormatException("Input string was not in a correct format.");

        return value;
    }

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value) => TryGetUInt64(s, out value, DataEncodingOptions.None);

    /// <inheritdoc/>
    [CLSCompliant(false)]
    public bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value, DataEncodingOptions options)
    {
        value = 0;

        options = GetEffectiveOptions(options);

        if (s.IsEmpty)
            return false;

        const int BitsPerValue = sizeof(long) * 8;

        bool checksum = (options & DataEncodingOptions.Checksum) != 0;

        ulong bits = 0; // accumulated bits
        int bitCount = 0; // count of accumulated bits
        int psi = -1; // previous symbol index

        var alphabet = Alphabet;
        var paddingChar = PaddingChar;
        bool relax = (options & DataEncodingOptions.Relax) != 0;

        foreach (var c in s)
        {
            if (!checksum && CharEqual(c, paddingChar, alphabet.IsCaseSensitive))
                continue;

            int si = alphabet.IndexOf(c); // symbol index lookup
            if (si == -1)
            {
                if (!relax)
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

        value = bits;
        return true;
    }

    #endregion

    #region BigInteger

    /// <inheritdoc/>
    public string GetString(BigInteger value) => GetString(value, DataEncodingOptions.None);

    /// <inheritdoc/>
    public string GetString(BigInteger value, DataEncodingOptions options)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");

        options = GetEffectiveOptions(options);

        var sb = new StringBuilder();

        var alphabet = Alphabet;

        if ((options & DataEncodingOptions.Checksum) != 0)
            sb.Append(alphabet[(int)(value % ChecksumAlphabetSize)]);

        do
        {
            var si = (int)(value & SymbolMask);
            value >>= BitsPerSymbol;

            sb.Append(alphabet[si]);
        }
        while (value > 0);

        sb.Reverse();

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

        options = GetEffectiveOptions(options);

        if (s.IsEmpty)
            return false;

        bool checksum = (options & DataEncodingOptions.Checksum) != 0;

        BigInteger bits = 0; // accumulated bits
        int psi = -1; // previous symbol index

        var alphabet = Alphabet;
        bool isCaseSensitive = alphabet.IsCaseSensitive;
        var paddingChar = PaddingChar;
        bool relax = (options & DataEncodingOptions.Relax) != 0;

        foreach (var c in s)
        {
            if (!checksum && CharEqual(c, paddingChar, isCaseSensitive))
                continue;

            int si = alphabet.IndexOf(c); // symbol index lookup
            if (si == -1)
            {
                if (!relax)
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

    #endregion

    /// <inheritdoc/>
    protected override DataEncodingOptions GetEffectiveStreamingOptions(DataEncodingOptions options)
    {
        if ((options & DataEncodingOptions.Checksum) != 0)
        {
            throw new NotSupportedException(
                string.Format(
                    "{0} encoding does not support checksum for streaming operations.",
                    this));
        }

        return base.GetEffectiveStreamingOptions(options);
    }

    void ValidateCodecOptions(DataEncodingOptions options)
    {
        if ((options & DataEncodingOptions.Checksum) != 0)
        {
            throw new NotSupportedException(
                string.Format(
                    "{0} encoding does not provide checksum operations over arbitrary data blocks.",
                    this));
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
