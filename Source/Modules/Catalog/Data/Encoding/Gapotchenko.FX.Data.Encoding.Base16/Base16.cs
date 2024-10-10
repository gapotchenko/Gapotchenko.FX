// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides implementation of Base16 encoding which conforms to RFC 4648.
/// </summary>
public sealed partial class Base16 : GenericBase16
{
    Base16() :
        base(new TextDataEncodingAlphabet("0123456789ABCDEF", false))
    {
    }

    /// <inheritdoc/>
    protected override byte[]? GetBytesCore(ReadOnlySpan<char> s, DataEncodingOptions options, bool throwOnError)
    {
        if (FastDecoder.SupportsOptions(options))
            return FastDecoder.GetBytes(s, throwOnError);
        else
            return base.GetBytesCore(s, options, throwOnError);
    }

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of Base16 characters.
    /// </summary>
    /// <inheritdoc cref="ITextDataEncoding.GetString(ReadOnlySpan{byte})"/>
    public static new string GetString(ReadOnlySpan<byte> data) => GetString(data, DataEncodingOptions.None);

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of Base16 characters with specified options.
    /// </summary>
    /// <inheritdoc cref="ITextDataEncoding.GetString(ReadOnlySpan{byte}, DataEncodingOptions)"/>
    public static new string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

    /// <summary>
    /// Decodes all Base16 characters in the specified read-only span into a byte array.
    /// </summary>
    /// <inheritdoc cref="ITextDataEncoding.GetBytes(ReadOnlySpan{char})"/>
    public static new byte[] GetBytes(ReadOnlySpan<char> s) => GetBytes(s, DataEncodingOptions.None);

    /// <summary>
    /// Decodes all Base16 characters in the specified read-only span into a byte array with specified options.
    /// </summary>
    /// <inheritdoc cref="ITextDataEncoding.GetBytes(ReadOnlySpan{char}, DataEncodingOptions)"/>
    public static new byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) =>
        FastDecoder.SupportsOptions(options) ?
            FastDecoder.GetBytes(s, true) ?? throw new FormatException("Cannot decode a Base16 string.") :
            Instance.GetBytes(s, options);

    /// <summary>
    /// Decodes all Base16 characters in the specified string into a byte array.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan());

    /// <summary>
    /// Decodes all Base16 characters in the specified string into a byte array with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan(), options);

    /// <summary>
    /// Tries to decode all Base16 characters in the specified read-only span into a byte array.
    /// </summary>
    /// <inheritdoc cref="ITextDataEncoding.TryGetBytes(ReadOnlySpan{char}, out byte[])"/>
    public static new bool TryGetBytes(ReadOnlySpan<char> s, [NotNullWhen(true)] out byte[]? data) =>
        TryGetBytes(s, DataEncodingOptions.None, out data);

    /// <summary>
    /// Tries to decode all Base16 characters in the specified read-only span into a byte array with specified options.
    /// </summary>
    /// <inheritdoc cref="ITextDataEncoding.TryGetBytes(ReadOnlySpan{char}, DataEncodingOptions, out byte[])"/>
    public static new bool TryGetBytes(ReadOnlySpan<char> s, DataEncodingOptions options, [NotNullWhen(true)] out byte[]? data) =>
        (data = TryGetBytes(s, options)) != null;

    /// <summary>
    /// Tries to decode all Base16 characters in the specified read-only span into a byte array.
    /// </summary>
    /// <inheritdoc cref="ITextDataEncoding.TryGetBytes(ReadOnlySpan{char})"/>
    public static new byte[]? TryGetBytes(ReadOnlySpan<char> s) => TryGetBytes(s, DataEncodingOptions.None);

    /// <summary>
    /// Tries to decode all Base16 characters in the specified read-only span into a byte array with specified options.
    /// </summary>
    /// <inheritdoc cref="ITextDataEncoding.TryGetBytes(ReadOnlySpan{char})"/>
    public static new byte[]? TryGetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) =>
        FastDecoder.SupportsOptions(options) ?
            FastDecoder.GetBytes(s, false) :
            Instance.TryGetBytes(s, options);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static volatile IBase16? m_Instance;

    /// <summary>
    /// Returns a default instance of <see cref="Base16"/> encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static IBase16 Instance => m_Instance ??= new Base16();
}
