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

#if !TFF_MEMORY || TFF_MEMORY_OOB
            var bytes = new byte[data.Length + 1];

            int i = data.Length;
            foreach (var b in data)
                bytes[--i] = b;

            var value = new BigInteger(bytes);
#else
            var value = new BigInteger(data, true, true);
#endif

            Write(sb, value);

            Debug.Assert(sb.Length <= capacity, "Invalid capacity.");

            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override byte[] GetBytesCore(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            if ((options & DataEncodingOptions.Checksum) != 0)
            {
                return VerifyAndRemoveChecksum(
                    GetBytesCore(s, options & ~DataEncodingOptions.Checksum))
                    .ToArray();
            }

            int leadingZeroCount = GetLeadingZeroCount(s);

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
                    throw new FormatException($"Encountered a non-{Name} character.");

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

            Reverse(sb, startIndex, sb.Length - startIndex);
        }

        static void Reverse(StringBuilder sb, int index, int length)
        {
            int i = index;
            int j = (index + length) - 1;

            while (i < j)
            {
                var t = sb[i];
                sb[i] = sb[j];
                sb[j] = t;

                i++;
                j--;
            }
        }

        static int GetLeadingZeroCount(ReadOnlySpan<byte> data)
        {
            int count = 0;
            foreach (var b in data)
            {
                if (b != 0)
                    break;
                ++count;
            }
            return count;
        }

        int GetLeadingZeroCount(ReadOnlySpan<char> s)
        {
            char zc = Alphabet[0];
            int count = 0;
            foreach (var c in s)
            {
                if (c != zc)
                    break;
                ++count;
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

        static ReadOnlySpan<byte> VerifyAndRemoveChecksum(ReadOnlySpan<byte> data)
        {
            int n = data.Length;
            if (n < ChecksumSize)
                throw new FormatException("Invalid checksum.");

            int j = n - ChecksumSize;

            var payload = data.Slice(0, j);
            var checksum = data.Slice(j);

            var actualChecksum = GetChecksum(payload);
            if (!checksum.SequenceEqual(actualChecksum))
                throw new FormatException("Invalid checksum.");

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

        /// <inheritdoc/>
        public string GetString(BigInteger value) => GetString(value, DataEncodingOptions.None);

        /// <inheritdoc/>
        public string GetString(BigInteger value, DataEncodingOptions options)
        {
            if (value.IsZero)
                return new string(Alphabet[0], 1);

            var sb = new StringBuilder();
            Write(sb, value);
            return sb.ToString();
        }

        /// <inheritdoc/>
        public BigInteger GetBigInteger(ReadOnlySpan<char> s)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public BigInteger GetBigInteger(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value, DataEncodingOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
