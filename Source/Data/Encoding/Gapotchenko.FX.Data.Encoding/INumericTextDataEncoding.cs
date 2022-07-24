using System.Numerics;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// The interface of a numeric binary-to-text encoding.
/// </summary>
[CLSCompliant(false)]
public interface INumericTextDataEncoding : ITextDataEncoding
{
    #region Int32

    /// <summary>
    /// Encodes the specified <see cref="Int32"/> value into a string.
    /// </summary>
    /// <param name="value">The <see cref="Int32"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="Int32"/> value.</returns>
    string GetString(int value);

    /// <summary>
    /// Encodes the specified <see cref="Int32"/> value into a string with specified options.
    /// </summary>
    /// <param name="value">The <see cref="Int32"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="Int32"/> value.</returns>
    string GetString(int value, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="Int32"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="Int32"/> value.</returns>
    int GetInt32(ReadOnlySpan<char> s);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="Int32"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="Int32"/> value.</returns>
    int GetInt32(ReadOnlySpan<char> s, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="Int32"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetInt32(ReadOnlySpan<char> s, out int value);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="Int32"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetInt32(ReadOnlySpan<char> s, out int value, DataEncodingOptions options);

    #endregion

    #region UInt32

    /// <summary>
    /// Encodes the specified <see cref="UInt32"/> value into a string.
    /// </summary>
    /// <param name="value">The <see cref="UInt32"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="UInt32"/> value.</returns>
    string GetString(uint value);

    /// <summary>
    /// Encodes the specified <see cref="UInt32"/> value into a string with specified options.
    /// </summary>
    /// <param name="value">The <see cref="UInt32"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="UInt32"/> value.</returns>
    string GetString(uint value, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="UInt32"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="UInt32"/> value.</returns>
    uint GetUInt32(ReadOnlySpan<char> s);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="UInt32"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="UInt32"/> value.</returns>
    uint GetUInt32(ReadOnlySpan<char> s, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="UInt32"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetUInt32(ReadOnlySpan<char> s, out uint value);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="UInt32"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetUInt32(ReadOnlySpan<char> s, out uint value, DataEncodingOptions options);

    #endregion

    #region Int64

    /// <summary>
    /// Encodes the specified <see cref="Int64"/> value into a string.
    /// </summary>
    /// <param name="value">The <see cref="Int64"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="Int64"/> value.</returns>
    string GetString(long value);

    /// <summary>
    /// Encodes the specified <see cref="Int64"/> value into a string with specified options.
    /// </summary>
    /// <param name="value">The <see cref="Int64"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="Int64"/> value.</returns>
    string GetString(long value, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="Int64"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="Int64"/> value.</returns>
    long GetInt64(ReadOnlySpan<char> s);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="Int64"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="Int64"/> value.</returns>
    long GetInt64(ReadOnlySpan<char> s, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="Int64"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetInt64(ReadOnlySpan<char> s, out long value);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="Int64"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetInt64(ReadOnlySpan<char> s, out long value, DataEncodingOptions options);

    #endregion

    #region UInt64

    /// <summary>
    /// Encodes the specified <see cref="UInt64"/> value into a string.
    /// </summary>
    /// <param name="value">The <see cref="UInt64"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="UInt64"/> value.</returns>
    string GetString(ulong value);

    /// <summary>
    /// Encodes the specified <see cref="UInt64"/> value into a string with specified options.
    /// </summary>
    /// <param name="value">The <see cref="UInt64"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="UInt64"/> value.</returns>
    string GetString(ulong value, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="UInt64"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="UInt64"/> value.</returns>
    ulong GetUInt64(ReadOnlySpan<char> s);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="UInt64"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="UInt64"/> value.</returns>
    ulong GetUInt64(ReadOnlySpan<char> s, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="UInt64"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="UInt64"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value, DataEncodingOptions options);

    #endregion

    #region BigInteger

    /// <summary>
    /// Encodes the specified <see cref="BigInteger"/> value into a string.
    /// </summary>
    /// <param name="value">The <see cref="BigInteger"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="BigInteger"/> value.</returns>
    string GetString(BigInteger value);

    /// <summary>
    /// Encodes the specified <see cref="BigInteger"/> value into a string with specified options.
    /// </summary>
    /// <param name="value">The <see cref="BigInteger"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="BigInteger"/> value.</returns>
    string GetString(BigInteger value, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="BigInteger"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="BigInteger"/> value.</returns>
    BigInteger GetBigInteger(ReadOnlySpan<char> s);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="BigInteger"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="BigInteger"/> value.</returns>
    BigInteger GetBigInteger(ReadOnlySpan<char> s, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="BigInteger"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="BigInteger"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into an <see cref="BigInteger"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="BigInteger"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value, DataEncodingOptions options);

    #endregion
}
