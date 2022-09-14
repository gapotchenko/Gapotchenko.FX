using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides z-base-32 encoding implementation.
/// </summary>
public sealed class ZBase32 : GenericZBase32
{
    ZBase32() :
        base(new TextDataEncodingAlphabet("ybndrfg8ejkmcpqxot1uwisza345h769", false), '=')
    {
    }

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of z-base-32 characters.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <returns>The string with encoded data.</returns>
    public new static string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of z-base-32 characters with specified options.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded data.</returns>
    public new static string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

    /// <summary>
    /// Decodes all z-base-32 characters in the specified read-only span into a byte array.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public new static byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

    /// <summary>
    /// Decodes all z-base-32 characters in the specified read-only span into a byte array with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public new static byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

    /// <summary>
    /// Decodes all z-base-32 characters in the specified string into a byte array.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan());

    /// <summary>
    /// Decodes all z-base-32 characters in the specified string into a byte array with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan(), options);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static volatile IBase32? m_Instance;

    /// <summary>
    /// Returns a default instance of <see cref="ZBase32"/> encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static IBase32 Instance => m_Instance ??= new ZBase32();
}
