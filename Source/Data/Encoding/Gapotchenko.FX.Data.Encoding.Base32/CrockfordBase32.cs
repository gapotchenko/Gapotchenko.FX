using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides Crockford Base 32 encoding implementation.
/// </summary>
public sealed class CrockfordBase32 : GenericCrockfordBase32
{
    CrockfordBase32() : base(
        new TextDataEncodingAlphabet(
            "0123456789ABCDEFGHJKMNPQRSTVWXYZ*~$=U",
            false,
            new Dictionary<char, string>
            {
                ['0'] = "O",
                ['1'] = "IL",
            }),
        '#')
    {
    }

    #region Data

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of Crockford Base 32 characters.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <returns>The string with encoded data.</returns>
    public static new string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of Crockford Base 32 characters with specified options.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded data.</returns>
    public static new string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into a byte array.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static new byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into a byte array with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static new byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into a byte array.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan());

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into a byte array with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan(), options);

    #endregion

    #region Int32

    /// <summary>
    /// Encodes the specified <see cref="Int32"/> value into a string of Crockford Base 32 characters.
    /// </summary>
    /// <param name="value">The <see cref="Int32"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="Int32"/> value.</returns>
    public static new string GetString(int value) => Instance.GetString(value);

    /// <summary>
    /// Encodes the specified <see cref="Int32"/> value into a string of Crockford Base 32 characters with specified options.
    /// </summary>
    /// <param name="value">The <see cref="Int32"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="Int32"/> value.</returns>
    public static new string GetString(int value, DataEncodingOptions options) => Instance.GetString(value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="Int32"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="Int32"/> value.</returns>
    public static new int GetInt32(ReadOnlySpan<char> s) => Instance.GetInt32(s);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="Int32"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="Int32"/> value.</returns>
    public static new int GetInt32(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetInt32(s, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="Int32"/> value.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>The decoded <see cref="Int32"/> value.</returns>
    public static int GetInt32(string s) => GetInt32(s.AsSpan());

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="Int32"/> value with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="Int32"/> value.</returns>
    public static int GetInt32(string s, DataEncodingOptions options) => GetInt32(s.AsSpan(), options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="Int32"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static new bool TryGetInt32(ReadOnlySpan<char> s, out int value) => Instance.TryGetInt32(s, out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="Int32"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static new bool TryGetInt32(ReadOnlySpan<char> s, out int value, DataEncodingOptions options) => Instance.TryGetInt32(s, out value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="Int32"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetInt32([NotNullWhen(true)] string? s, out int value) => TryGetInt32(s.AsSpan(), out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="Int32"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetInt32([NotNullWhen(true)] string? s, out int value, DataEncodingOptions options) => TryGetInt32(s.AsSpan(), out value, options);

    #endregion

    #region UInt32

    /// <summary>
    /// Encodes the specified <see cref="UInt32"/> value into a string of Crockford Base 32 characters.
    /// </summary>
    /// <param name="value">The <see cref="UInt32"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="UInt32"/> value.</returns>
    [CLSCompliant(false)]
    public static new string GetString(uint value) => Instance.GetString(value);

    /// <summary>
    /// Encodes the specified <see cref="UInt32"/> value into a string of Crockford Base 32 characters with specified options.
    /// </summary>
    /// <param name="value">The <see cref="UInt32"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="UInt32"/> value.</returns>
    [CLSCompliant(false)]
    public static new string GetString(uint value, DataEncodingOptions options) => Instance.GetString(value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="UInt32"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="UInt32"/> value.</returns>
    [CLSCompliant(false)]
    public static new uint GetUInt32(ReadOnlySpan<char> s) => Instance.GetUInt32(s);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="UInt32"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="UInt32"/> value.</returns>
    [CLSCompliant(false)]
    public static new uint GetUInt32(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetUInt32(s, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="UInt32"/> value.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>The decoded <see cref="UInt32"/> value.</returns>
    [CLSCompliant(false)]
    public static uint GetUInt32(string s) => GetUInt32(s.AsSpan());

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="UInt32"/> value with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="UInt32"/> value.</returns>
    [CLSCompliant(false)]
    public static uint GetUInt32(string s, DataEncodingOptions options) => GetUInt32(s.AsSpan(), options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="UInt32"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static new bool TryGetUInt32(ReadOnlySpan<char> s, out uint value) => Instance.TryGetUInt32(s, out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="UInt32"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static new bool TryGetUInt32(ReadOnlySpan<char> s, out uint value, DataEncodingOptions options) => Instance.TryGetUInt32(s, out value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="UInt32"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static bool TryGetUInt32([NotNullWhen(true)] string? s, out uint value) => TryGetUInt32(s.AsSpan(), out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="UInt32"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt32"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static bool TryGetUInt32([NotNullWhen(true)] string? s, out uint value, DataEncodingOptions options) => TryGetUInt32(s.AsSpan(), out value, options);

    #endregion

    #region Int64

    /// <summary>
    /// Encodes the specified <see cref="Int64"/> value into a string of Crockford Base 32 characters.
    /// </summary>
    /// <param name="value">The <see cref="Int64"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="Int64"/> value.</returns>
    public static new string GetString(long value) => Instance.GetString(value);

    /// <summary>
    /// Encodes the specified <see cref="Int64"/> value into a string of Crockford Base 32 characters with specified options.
    /// </summary>
    /// <param name="value">The <see cref="Int64"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="Int64"/> value.</returns>
    public static new string GetString(long value, DataEncodingOptions options) => Instance.GetString(value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="Int64"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="Int64"/> value.</returns>
    public static new long GetInt64(ReadOnlySpan<char> s) => Instance.GetInt64(s);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="Int64"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="Int64"/> value.</returns>
    public static new long GetInt64(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetInt64(s, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="Int64"/> value.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>The decoded <see cref="Int64"/> value.</returns>
    public static long GetInt64(string s) => GetInt64(s.AsSpan());

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="Int64"/> value with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="Int64"/> value.</returns>
    public static long GetInt64(string s, DataEncodingOptions options) => GetInt64(s.AsSpan(), options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="Int64"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static new bool TryGetInt64(ReadOnlySpan<char> s, out long value) => Instance.TryGetInt64(s, out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="Int64"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static new bool TryGetInt64(ReadOnlySpan<char> s, out long value, DataEncodingOptions options) => Instance.TryGetInt64(s, out value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="Int64"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetInt64([NotNullWhen(true)] string? s, out long value) => TryGetInt64(s.AsSpan(), out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="Int64"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="Int64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetInt64([NotNullWhen(true)] string? s, out long value, DataEncodingOptions options) => TryGetInt64(s.AsSpan(), out value, options);

    #endregion

    #region UInt64

    /// <summary>
    /// Encodes the specified <see cref="UInt64"/> value into a string of Crockford Base 32 characters.
    /// </summary>
    /// <param name="value">The <see cref="UInt64"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="UInt64"/> value.</returns>
    [CLSCompliant(false)]
    public static new string GetString(ulong value) => Instance.GetString(value);

    /// <summary>
    /// Encodes the specified <see cref="UInt64"/> value into a string of Crockford Base 32 characters with specified options.
    /// </summary>
    /// <param name="value">The <see cref="UInt64"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="UInt64"/> value.</returns>
    [CLSCompliant(false)]
    public static new string GetString(ulong value, DataEncodingOptions options) => Instance.GetString(value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="UInt64"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="UInt64"/> value.</returns>
    [CLSCompliant(false)]
    public static new ulong GetUInt64(ReadOnlySpan<char> s) => Instance.GetUInt64(s);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="UInt64"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="UInt64"/> value.</returns>
    [CLSCompliant(false)]
    public static new ulong GetUInt64(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetUInt64(s, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="UInt64"/> value.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>The decoded <see cref="UInt64"/> value.</returns>
    [CLSCompliant(false)]
    public static ulong GetUInt64(string s) => GetUInt64(s.AsSpan());

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="UInt64"/> value with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="UInt64"/> value.</returns>
    [CLSCompliant(false)]
    public static ulong GetUInt64(string s, DataEncodingOptions options) => GetUInt64(s.AsSpan(), options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="UInt64"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static new bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value) => Instance.TryGetUInt64(s, out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="UInt64"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static new bool TryGetUInt64(ReadOnlySpan<char> s, out ulong value, DataEncodingOptions options) => Instance.TryGetUInt64(s, out value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="UInt64"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static bool TryGetUInt64([NotNullWhen(true)] string? s, out ulong value) => TryGetUInt64(s.AsSpan(), out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="UInt64"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="UInt64"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    [CLSCompliant(false)]
    public static bool TryGetUInt64([NotNullWhen(true)] string? s, out ulong value, DataEncodingOptions options) => TryGetUInt64(s.AsSpan(), out value, options);

    #endregion

    #region BigInteger

    /// <summary>
    /// Encodes the specified <see cref="BigInteger"/> value into a string of Crockford Base 32 characters.
    /// </summary>
    /// <param name="value">The <see cref="BigInteger"/> value to encode.</param>
    /// <returns>The string with encoded <see cref="BigInteger"/> value.</returns>
    public static new string GetString(BigInteger value) => Instance.GetString(value);

    /// <summary>
    /// Encodes the specified <see cref="BigInteger"/> value into a string of Crockford Base 32 characters with specified options.
    /// </summary>
    /// <param name="value">The <see cref="BigInteger"/> value to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded <see cref="BigInteger"/> value.</returns>
    public static new string GetString(BigInteger value, DataEncodingOptions options) => Instance.GetString(value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="BigInteger"/> value.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>The decoded <see cref="BigInteger"/> value.</returns>
    public static new BigInteger GetBigInteger(ReadOnlySpan<char> s) => Instance.GetBigInteger(s);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="BigInteger"/> value with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="BigInteger"/> value.</returns>
    public static new BigInteger GetBigInteger(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBigInteger(s, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="BigInteger"/> value.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>The decoded <see cref="BigInteger"/> value.</returns>
    public static BigInteger GetBigInteger(string s) => GetBigInteger(s.AsSpan());

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="BigInteger"/> value with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The decoded <see cref="BigInteger"/> value.</returns>
    public static BigInteger GetBigInteger(string s, DataEncodingOptions options) => GetBigInteger(s.AsSpan(), options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="BigInteger"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="BigInteger"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static new bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value) => Instance.TryGetBigInteger(s, out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified read-only span into an <see cref="BigInteger"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="BigInteger"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static new bool TryGetBigInteger(ReadOnlySpan<char> s, out BigInteger value, DataEncodingOptions options) => Instance.TryGetBigInteger(s, out value, options);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="BigInteger"/> value.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="BigInteger"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetBigInteger([NotNullWhen(true)] string? s, out BigInteger value) => TryGetBigInteger(s.AsSpan(), out value);

    /// <summary>
    /// Decodes all Crockford Base 32 characters in the specified string into an <see cref="BigInteger"/> value with specified options.
    /// A return value indicates whether the decoding succeeded.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="value">When this method returns, contains the decoded <see cref="BigInteger"/> value, if the decoding succeeded, or zero if the decoding failed.</param>
    /// <param name="options">The options.</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was decoded successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetBigInteger([NotNullWhen(true)] string? s, out BigInteger value, DataEncodingOptions options) => TryGetBigInteger(s.AsSpan(), out value, options);

    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static volatile ICrockfordBase32? m_Instance;

    /// <summary>
    /// Returns a default instance of <see cref="CrockfordBase32"/> encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [CLSCompliant(false)]
    public static ICrockfordBase32 Instance => m_Instance ??= new CrockfordBase32();
}
