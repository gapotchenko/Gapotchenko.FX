using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides Crockford Base 32 encoding implementation.
    /// </summary>
    public sealed class CrockfordBase32 : GenericCrockfordBase32
    {
        private CrockfordBase32() : base(
            new TextDataEncodingAlphabet(
                "0123456789ABCDEFGHJKMNPQRSTVWXYZ*~$=U",
                false,
                new Dictionary<char, string>
                {
                    ['0'] = "O",
                    ['1'] = "IL",
                }))
        {
        }

        /// <inheritdoc/>
        public override string Name => "Crockford Base 32";

        #region Data

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Crockford Base 32 symbols.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation, in Crockford Base 32, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation that is encoded with Crockford Base 32 symbols with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation, in Crockford Base 32, of the contents of <paramref name="data"/>.</returns>
        public new static string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Crockford Base 32 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Crockford Base 32 symbols, to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public static byte[] GetBytes(string s) => GetBytes(s.AsSpan());

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Crockford Base 32 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public new static byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

        /// <summary>
        /// Decodes the specified string, which represents encoded binary data as Crockford Base 32 symbols, to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes(s.AsSpan(), options);

        #endregion

        #region Int32

        /// <summary>
        /// Encodes specified <see cref="Int32"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        public new static string GetString(int value) => Instance.GetString(value);

        /// <summary>
        /// Encodes specified <see cref="Int32"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        public new static string GetString(int value, DataEncodingOptions options) => Instance.GetString(value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="Int32"/> value that is equivalent to <paramref name="s"/>.</returns>
        public new static int GetInt32(ReadOnlySpan<char> s) => Instance.GetInt32(s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="Int32"/> value that is equivalent to <paramref name="s"/>.</returns>
        public static int GetInt32(string s) => GetInt32(s.AsSpan());

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="Int32"/> value that is equivalent to <paramref name="s"/>.</returns>
        public new static int GetInt32(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetInt32(s, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="Int32"/> value that is equivalent to <paramref name="s"/>.</returns>
        public static int GetInt32(string s, DataEncodingOptions options) => GetInt32(s.AsSpan(), options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int32"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public new static bool TryGetInt32(ReadOnlySpan<char> s, out int value) => Instance.TryGetInt32(s, out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int32"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public static bool TryGetInt32(string s, out int value) => TryGetInt32(s.AsSpan(), out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int32"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public new static bool TryGetInt32(ReadOnlySpan<char> s, out int value, DataEncodingOptions options) => Instance.TryGetInt32(s, out value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int32"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public static bool TryGetInt32(string s, out int value, DataEncodingOptions options) => TryGetInt32(s.AsSpan(), out value, options);

        #endregion

        #region UInt32

        /// <summary>
        /// Encodes specified <see cref="UInt32"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="UInt32"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public new static string GetString(uint value) => Instance.GetString(value);

        /// <summary>
        /// Encodes specified <see cref="UInt32"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="UInt32"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public new static string GetString(uint value, DataEncodingOptions options) => Instance.GetString(value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="UInt32"/> value that is equivalent to <paramref name="s"/>.</returns>
        [CLSCompliant(false)]
        public new static uint GetUInt32(ReadOnlySpan<char> s) => Instance.GetUInt32(s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="UInt32"/> value that is equivalent to <paramref name="s"/>.</returns>
        [CLSCompliant(false)]
        public static uint GetUInt32(string s) => GetUInt32(s.AsSpan());

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="UInt32"/> value that is equivalent to <paramref name="s"/>.</returns>
        [CLSCompliant(false)]
        public new static uint GetUInt32(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetUInt32(s, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="UInt32"/> value that is equivalent to <paramref name="s"/>.</returns>
        [CLSCompliant(false)]
        public static uint GetUInt32(string s, DataEncodingOptions options) => GetUInt32(s.AsSpan(), options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt32"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        [CLSCompliant(false)]
        public new static bool TryGetUInt32(ReadOnlySpan<char> s, out uint value) => Instance.TryGetUInt32(s, out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt32"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        [CLSCompliant(false)]
        public static bool TryGetUInt32(string s, out uint value) => TryGetUInt32(s.AsSpan(), out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt32"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        [CLSCompliant(false)]
        public new static bool TryGetUInt32(ReadOnlySpan<char> s, out uint value, DataEncodingOptions options) => Instance.TryGetUInt32(s, out value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt32"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        [CLSCompliant(false)]
        public static bool TryGetUInt32(string s, out uint value, DataEncodingOptions options) => TryGetUInt32(s.AsSpan(), out value, options);

        #endregion

        #region Int64

        /// <summary>
        /// Encodes specified <see cref="Int64"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="Int64"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        public new static string GetString(long value) => Instance.GetString(value);

        /// <summary>
        /// Encodes specified <see cref="Int64"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="Int64"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        public new static string GetString(long value, DataEncodingOptions options) => Instance.GetString(value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="Int64"/> value that is equivalent to <paramref name="s"/>.</returns>
        public new static long GetInt64(ReadOnlySpan<char> s) => Instance.GetInt64(s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="Int64"/> value that is equivalent to <paramref name="s"/>.</returns>
        public static long GetInt64(string s) => GetInt64(s.AsSpan());

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="Int64"/> value that is equivalent to <paramref name="s"/>.</returns>
        public new static long GetInt64(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetInt64(s, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="Int64"/> value that is equivalent to <paramref name="s"/>.</returns>
        public static long GetInt64(string s, DataEncodingOptions options) => GetInt64(s.AsSpan(), options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int64"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public new static bool TryGetInt64(ReadOnlySpan<char> s, out long value) => Instance.TryGetInt64(s, out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int64"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public static bool TryGetInt64(string s, out long value) => TryGetInt64(s.AsSpan(), out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int64"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public new static bool TryGetInt64(ReadOnlySpan<char> s, out long value, DataEncodingOptions options) => Instance.TryGetInt64(s, out value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int64"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public static bool TryGetInt64(string s, out long value, DataEncodingOptions options) => TryGetInt64(s.AsSpan(), out value, options);

        #endregion

        #region UInt64

        /// <summary>
        /// Encodes specified <see cref="UInt64"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="UInt64"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public new static string GetString(ulong value) => Instance.GetString(value);

        /// <summary>
        /// Encodes specified <see cref="UInt64"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="UInt64"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public new static string GetString(ulong value, DataEncodingOptions options) => Instance.GetString(value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="UInt64"/> value that is equivalent to <paramref name="s"/>.</returns>
        [CLSCompliant(false)]
        public new static ulong GetUInt64(ReadOnlySpan<char> s) => Instance.GetUInt64(s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="UInt64"/> value that is equivalent to <paramref name="s"/>.</returns>
        [CLSCompliant(false)]
        public static ulong GetUInt64(string s) => GetUInt64(s.AsSpan());

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="UInt64"/> value that is equivalent to <paramref name="s"/>.</returns>
        [CLSCompliant(false)]
        public new static ulong GetUInt64(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetUInt64(s, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="UInt64"/> value that is equivalent to <paramref name="s"/>.</returns>
        [CLSCompliant(false)]
        public static ulong GetUInt64(string s, DataEncodingOptions options) => GetUInt64(s.AsSpan(), options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt64"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        [CLSCompliant(false)]
        public new static bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value) => Instance.TryGetUInt64(s, out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt64"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        [CLSCompliant(false)]
        public static bool TryGetUInt64(string s, out ulong value) => TryGetUInt64(s.AsSpan(), out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt64"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        [CLSCompliant(false)]
        public new static bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value, DataEncodingOptions options) => Instance.TryGetUInt64(s, out value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt64"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        [CLSCompliant(false)]
        public static bool TryGetUInt64(string s, out ulong value, DataEncodingOptions options) => TryGetUInt64(s.AsSpan(), out value, options);

        #endregion

        #region BigInteger

        /// <summary>
        /// Encodes specified <see cref="BigInteger"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="BigInteger"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        public new static string GetString(BigInteger value) => Instance.GetString(value);

        /// <summary>
        /// Encodes specified <see cref="BigInteger"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="BigInteger"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        public new static string GetString(BigInteger value, DataEncodingOptions options) => Instance.GetString(value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A <see cref="BigInteger"/> value that is equivalent to <paramref name="s"/>.</returns>
        public new static BigInteger GetBigInteger(ReadOnlySpan<char> s) => Instance.GetBigInteger(s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A <see cref="BigInteger"/> value that is equivalent to <paramref name="s"/>.</returns>
        public static BigInteger GetBigInteger(string s) => GetBigInteger(s.AsSpan());

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="BigInteger"/> value that is equivalent to <paramref name="s"/>.</returns>
        public new static BigInteger GetBigInteger(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBigInteger(s, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="BigInteger"/> value that is equivalent to <paramref name="s"/>.</returns>
        public static BigInteger GetBigInteger(string s, DataEncodingOptions options) => GetBigInteger(s.AsSpan(), options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="BigInteger"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public new static bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value) => Instance.TryGetBigInteger(s, out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="BigInteger"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public static bool TryGetBigInteger(string s, out BigInteger value) => TryGetBigInteger(s.AsSpan(), out value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="BigInteger"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public new static bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value, DataEncodingOptions options) => Instance.TryGetBigInteger(s, out value, options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="BigInteger"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        public static bool TryGetBigInteger(string s, out BigInteger value, DataEncodingOptions options) => TryGetBigInteger(s.AsSpan(), out value, options);

        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile ICrockfordBase32 m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="CrockfordBase32"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [CLSCompliant(false)]
        public static ICrockfordBase32 Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new CrockfordBase32();
                return m_Instance;
            }
        }
    }
}
