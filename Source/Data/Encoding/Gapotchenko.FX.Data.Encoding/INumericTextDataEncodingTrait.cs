using System;
using System.Numerics;

#nullable enable

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines the numeric trait of a text data encoding.
    /// </summary>
    [CLSCompliant(false)]
    public interface INumericTextDataEncodingTrait
    {
        #region Int32

        /// <summary>
        /// Encodes specified <see cref="Int32"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(int value);

        /// <summary>
        /// Encodes specified <see cref="Int32"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(int value, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="Int32"/> value that is equivalent to <paramref name="s"/>.</returns>
        int GetInt32(ReadOnlySpan<char> s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="Int32"/> value that is equivalent to <paramref name="s"/>.</returns>
        int GetInt32(ReadOnlySpan<char> s, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int32"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetInt32(ReadOnlySpan<char> s, out int value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int32"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int32"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetInt32(ReadOnlySpan<char> s, out int value, DataEncodingOptions options);

        #endregion

        #region UInt32

        /// <summary>
        /// Encodes specified <see cref="UInt32"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="UInt32"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(uint value);

        /// <summary>
        /// Encodes specified <see cref="UInt32"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="UInt32"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(uint value, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="UInt32"/> value that is equivalent to <paramref name="s"/>.</returns>
        uint GetUInt32(ReadOnlySpan<char> s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="UInt32"/> value that is equivalent to <paramref name="s"/>.</returns>
        uint GetUInt32(ReadOnlySpan<char> s, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt32"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetUInt32(ReadOnlySpan<char> s, out uint value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt32"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt32"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetUInt32(ReadOnlySpan<char> s, out uint value, DataEncodingOptions options);

        #endregion

        #region Int64

        /// <summary>
        /// Encodes specified <see cref="Int64"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="Int64"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(long value);

        /// <summary>
        /// Encodes specified <see cref="Int64"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="Int64"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(long value, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="Int64"/> value that is equivalent to <paramref name="s"/>.</returns>
        long GetInt64(ReadOnlySpan<char> s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="Int64"/> value that is equivalent to <paramref name="s"/>.</returns>
        long GetInt64(ReadOnlySpan<char> s, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int64"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetInt64(ReadOnlySpan<char> s, out long value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="Int64"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="Int64"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetInt64(ReadOnlySpan<char> s, out long value, DataEncodingOptions options);

        #endregion

        #region UInt64

        /// <summary>
        /// Encodes specified <see cref="UInt64"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="UInt64"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(ulong value);

        /// <summary>
        /// Encodes specified <see cref="UInt64"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="UInt64"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(ulong value, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An <see cref="UInt64"/> value that is equivalent to <paramref name="s"/>.</returns>
        ulong GetUInt64(ReadOnlySpan<char> s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An <see cref="UInt64"/> value that is equivalent to <paramref name="s"/>.</returns>
        ulong GetUInt64(ReadOnlySpan<char> s, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt64"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="UInt64"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="UInt64"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value, DataEncodingOptions options);

        #endregion

        #region BigInteger

        /// <summary>
        /// Encodes specified <see cref="BigInteger"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="BigInteger"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(BigInteger value);

        /// <summary>
        /// Encodes specified <see cref="BigInteger"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="BigInteger"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(BigInteger value, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A <see cref="BigInteger"/> value that is equivalent to <paramref name="s"/>.</returns>
        BigInteger GetBigInteger(ReadOnlySpan<char> s);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value with the specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="BigInteger"/> value that is equivalent to <paramref name="s"/>.</returns>
        BigInteger GetBigInteger(ReadOnlySpan<char> s, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="BigInteger"/> value equivalent to <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value);

        /// <summary>
        /// Decodes the specified string to an equivalent <see cref="BigInteger"/> value with the specified options.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="value">When this method returns, contains the <see cref="BigInteger"/> value equivalent to <paramref name="s"/>.</param>
        /// <param name="options">The options.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was decoded successfully; otherwise, <c>false</c>.</returns>
        bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value, DataEncodingOptions options);

        #endregion
    }
}
