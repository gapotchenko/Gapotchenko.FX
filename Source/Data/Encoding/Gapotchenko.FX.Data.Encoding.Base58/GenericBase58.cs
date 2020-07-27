using Gapotchenko.FX.Text;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Base58 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericBase58 : TextDataEncoding, IBase58
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase58"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase58(TextDataEncodingAlphabet alphabet)
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
                    string.Format("The alphabet size of {0} encoding should be {1}.", this, Base),
                    nameof(alphabet));
            }
        }

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly TextDataEncodingAlphabet Alphabet;

        #region Parameters

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
        public new const float Efficiency = 4.060443010546419336600504153820088f / 5.545177444479562475337856971665413f;  // log(Base) / log(256) = 0.7322476

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        /// <inheritdoc/>
        protected override float MaxEfficiencyCore => 1;

        /// <inheritdoc/>
        protected override string GetStringCore(ReadOnlySpan<byte> data, DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Checksum) != 0)
            {
                return GetStringCore(
                    AddChecksum(data),
                    options & ~DataEncodingOptions.Checksum);
            }

            int leadingZeroCount = GetLeadingZeroCount(data);

            data = data.Slice(leadingZeroCount);
            if (data.IsEmpty)
                return new string(Alphabet[0], leadingZeroCount);

            var capacity = GetMaxCharCountCore(data.Length, options) + leadingZeroCount;
            var sb = new StringBuilder(capacity);

            sb.Append(Alphabet[0], leadingZeroCount);

            var value = GetBigInteger(data, true, true);

            Write(sb, value);

            Debug.Assert(sb.Length <= capacity, "Invalid capacity.");

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override byte[] GetBytesCore(ReadOnlySpan<char> s, DataEncodingOptions options) => GetBytesCore(s, options, true)!;

        byte[]? GetBytesCore(ReadOnlySpan<char> s, DataEncodingOptions options, bool throwOnError)
        {
            if ((options & DataEncodingOptions.Checksum) != 0)
            {
                var data = VerifyAndRemoveChecksum(
                    GetBytesCore(s, options & ~DataEncodingOptions.Checksum, throwOnError),
                    throwOnError);

                return data == null ? null : data.ToArray();
            }

            int leadingZeroCount = GetLeadingZeroCount(s, Alphabet[0]);

            s = s.Slice(leadingZeroCount);
            if (s.IsEmpty)
                return new byte[leadingZeroCount];

            var capacity = GetMaxByteCountCore(s.Length, leadingZeroCount);
            var stream = new MemoryStream(capacity);

            for (int i = 0; i < leadingZeroCount; ++i)
                stream.WriteByte(0);

            BigInteger value = 0;

            var alphabet = Alphabet;

            foreach (var c in s)
            {
                int si = alphabet.IndexOf(c);
                if (si == -1)
                {
                    if (throwOnError)
                        throw new FormatException($"Encountered an invalid character for {this} encoding.");
                    return null;
                }

                value = value * Base + si;
            }

            var bytes = value.ToByteArray();

            bool lead = true;
            for (int i = bytes.Length - 1; i >= 0; --i)
            {
                var b = bytes[i];
                if (lead)
                {
                    if (b == 0)
                        continue;
                    lead = false;
                }
                stream.WriteByte(b);
            }

            Debug.Assert(stream.Length <= capacity, "Invalid capacity.");

            return stream.ToArray();
        }

        void Write(StringBuilder sb, BigInteger value)
        {
            int startIndex = sb.Length;
            var alphabet = Alphabet;

            while (value > 0)
            {
                var si = (int)(value % Base);
                value /= Base;

                sb.Append(alphabet[si]);
            }

            sb.Reverse(startIndex, sb.Length - startIndex);
        }

        static BigInteger GetBigInteger(ReadOnlySpan<byte> value, bool isUnsigned, bool isBigEndian)
        {
#if !TFF_MEMORY || TFF_MEMORY_OOB
            var bytes = new byte[value.Length + (isUnsigned ? 1 : 0)];

            int i = value.Length;
            foreach (var b in value)
                bytes[--i] = b;

            return new BigInteger(bytes);
#else
            return new BigInteger(value, isUnsigned, isBigEndian);
#endif
        }

        static int GetLeadingZeroCount(ReadOnlySpan<byte> data)
        {
            int count = 0;
            foreach (var b in data)
            {
                if (b != 0)
                    break;
                checked { ++count; }
            }
            return count;
        }

        static int GetLeadingZeroCount(ReadOnlySpan<char> s, char zeroChar)
        {
            int count = 0;
            foreach (var c in s)
            {
                if (c != zeroChar)
                    break;
                checked { ++count; }
            }
            return count;
        }

        const int ChecksumSize = 4;

        static ReadOnlySpan<byte> GetChecksum(ReadOnlySpan<byte> payload)
        {
            byte[] hash;

            using (var sha256 = SHA256.Create())
            {
#if !TFF_MEMORY || TFF_MEMORY_OOB
                hash = sha256.ComputeHash(sha256.ComputeHash(payload.ToArray()));
#else
                hash = new byte[32];
                if (!(sha256.TryComputeHash(payload, hash, out var hashSize) && sha256.TryComputeHash(hash, hash, out hashSize)))
                    throw new InvalidOperationException();
                Debug.Assert(hash.Length == hashSize);
#endif
            }

            return hash.AsSpan(0, ChecksumSize);
        }

        static byte[] AddChecksum(ReadOnlySpan<byte> payload)
        {
            int j = payload.Length;
            var data = new byte[j + ChecksumSize];

            payload.CopyTo(data);
            GetChecksum(payload).CopyTo(data.AsSpan(j));

            return data;
        }

        static ReadOnlySpan<byte> VerifyAndRemoveChecksum(ReadOnlySpan<byte> data, bool throwOnError)
        {
            int n = data.Length;
            if (n < ChecksumSize)
                throw new FormatException("Invalid checksum.");

            int j = n - ChecksumSize;

            var payload = data.Slice(0, j);
            var checksum = data.Slice(j);

            var actualChecksum = GetChecksum(payload);
            if (!checksum.SequenceEqual(actualChecksum))
            {
                if (throwOnError)
                    throw new FormatException("Invalid checksum.");
                return null;
            }

            return payload;
        }

        /// <inheritdoc/>
        public override bool CanStream => false;

        /// <inheritdoc/>
        protected override IEncoderContext CreateEncoderContext(DataEncodingOptions options) => throw new InvalidOperationException();

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContext(DataEncodingOptions options) => throw new InvalidOperationException();

        /// <inheritdoc/>
        public override bool IsCaseSensitive => Alphabet.IsCaseSensitive;

        /// <inheritdoc/>
        public override bool CanCanonicalize => Alphabet.IsCanonicalizable;

        /// <inheritdoc/>
        protected override void CanonicalizeCore(ReadOnlySpan<char> source, Span<char> destination) => Alphabet.Canonicalize(source, destination);

        /// <inheritdoc/>
        protected override int GetMaxCharCountCore(int byteCount, DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Checksum) != 0)
                byteCount += ChecksumSize;

            return (int)Math.Ceiling(byteCount / Efficiency);
        }

        /// <inheritdoc/>
        protected override int GetMaxByteCountCore(int charCount, DataEncodingOptions options)
        {
            int byteCount = charCount;

            if ((options & DataEncodingOptions.Checksum) != 0 && byteCount >= ChecksumSize)
                byteCount -= ChecksumSize;

            return byteCount;
        }

        int GetMaxByteCountCore(int charCount, int leadingZeroCount) =>
            leadingZeroCount +
            (int)Math.Ceiling(charCount * Efficiency);

        /// <inheritdoc/>
        public string GetString(int value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string GetString(int value, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetInt32(ReadOnlySpan<char> s)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int GetInt32(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryGetInt32(ReadOnlySpan<char> s, out int value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryGetInt32(ReadOnlySpan<char> s, out int value, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        #region UInt32

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public string GetString(uint value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public string GetString(uint value, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public uint GetUInt32(ReadOnlySpan<char> s)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public uint GetUInt32(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public bool TryGetUInt32(ReadOnlySpan<char> s, out uint value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public bool TryGetUInt32(ReadOnlySpan<char> s, out uint value, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Int64

        /// <inheritdoc/>
        public string GetString(long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string GetString(long value, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long GetInt64(ReadOnlySpan<char> s)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public long GetInt64(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryGetInt64(ReadOnlySpan<char> s, out long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryGetInt64(ReadOnlySpan<char> s, out long value, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region UInt64

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public string GetString(ulong value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public string GetString(ulong value, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ulong GetUInt64(ReadOnlySpan<char> s)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public ulong GetUInt64(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value, DataEncodingOptions options)
        {
            throw new NotImplementedException();
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

            if ((options & DataEncodingOptions.Checksum) != 0)
            {
                var bytes = value.ToByteArray();
                Array.Reverse(bytes);

                int leadingZeroCount = GetLeadingZeroCount(bytes.AsSpan(0, bytes.Length - 1));
                return GetString(bytes.AsSpan(leadingZeroCount), options);
            }

            if (value.IsZero)
                return new string(Alphabet[0], 1);

            var sb = new StringBuilder();
            Write(sb, value);
            return sb.ToString();
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
            var data = GetBytesCore(s, options, false);
            if (data == null)
            {
                value = default;
                return false;
            }
            else
            {
                value = GetBigInteger(data, true, true);
                return true;
            }
        }

        #endregion
    }
}
