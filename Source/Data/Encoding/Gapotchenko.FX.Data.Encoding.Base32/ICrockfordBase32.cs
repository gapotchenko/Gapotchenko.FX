using System;
using System.Numerics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// A unifying interface for all possible Crockford Base 32 encoding implementations.
    /// </summary>
    public interface ICrockfordBase32 : IBase32
    {
        /// <summary>
        /// Encodes an <see cref="Int32"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(int value);

        /// <summary>
        /// Encodes an <see cref="Int32"/> value to its equivalent string representation with specified options.
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

        /// <summary>
        /// Encodes an <see cref="Int64"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="Int64"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(long value);

        /// <summary>
        /// Encodes an <see cref="Int64"/> value to its equivalent string representation with specified options.
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

        /// <summary>
        /// Encodes an <see cref="BigInteger"/> value to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="BigInteger"/> value.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(BigInteger value);

        /// <summary>
        /// Encodes an <see cref="BigInteger"/> value to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="value">The <see cref="BigInteger"/> value.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="value"/>.</returns>
        string GetString(BigInteger value, DataEncodingOptions options);
    }
}
