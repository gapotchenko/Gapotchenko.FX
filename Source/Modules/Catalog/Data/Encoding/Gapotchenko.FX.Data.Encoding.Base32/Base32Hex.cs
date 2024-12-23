﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Implements base32-hex encoding described in RFC 4648.
/// </summary>
public sealed class Base32Hex : GenericBase32
{
    Base32Hex() :
        base(new TextDataEncodingAlphabet("0123456789ABCDEFGHIJKLMNOPQRSTUV", false), '=')
    {
    }

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of base32-hex characters.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <returns>The string with encoded data.</returns>
    public static new string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of base32-hex characters with specified options.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded data.</returns>
    public static new string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

    /// <summary>
    /// Decodes all base32-hex characters in the specified read-only span into a byte array.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static new byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

    /// <summary>
    /// Decodes all base32-hex characters in the specified read-only span into a byte array with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static new byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

    /// <summary>
    /// Decodes all base32-hex characters in the specified string into a byte array.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan());

    /// <summary>
    /// Decodes all base32-hex characters in the specified string into a byte array with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan(), options);

    /// <summary>
    /// The number of symbols for padding of the encoded data representation.
    /// </summary>
    public new const int Padding = SymbolsPerEncodedBlock;

    /// <summary>
    /// Pads the encoded read-only character span.
    /// </summary>
    /// <param name="s">The read-only character span to pad.</param>
    /// <returns>The padded encoded string.</returns>
    public static new string Pad(ReadOnlySpan<char> s) => Instance.Pad(s);

    /// <summary>
    /// Unpads the encoded read-only character span.
    /// </summary>
    /// <param name="s">The read-only character span to unpad.</param>
    /// <returns>The unpadded read-only character span.</returns>
    public static new ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s) => Instance.Unpad(s);

    /// <summary>
    /// Returns a default instance of <see cref="Base32Hex"/> encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static IBase32 Instance => m_Instance ??= new Base32Hex();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static volatile IBase32? m_Instance;
}
