using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides Kuon Base24 encoding implementation.
/// </summary>
public sealed class KuonBase24 : GenericKuonBase24
{
    KuonBase24() :
        base(new TextDataEncodingAlphabet("ZAC2B3EF4GH5TK67P8RS9WXY", false), '=')
    {
    }

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of Kuon Base24 characters.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <returns>The string with encoded data.</returns>
    public static new string GetString(ReadOnlySpan<byte> data) => Instance.GetString(data);

    /// <summary>
    /// Encodes all the bytes in the specified span into a string of Kuon Base24 characters with specified options.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded data.</returns>
    public static new string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => Instance.GetString(data, options);

    /// <summary>
    /// Decodes all Kuon Base24 characters in the specified read-only span into a byte array.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static new byte[] GetBytes(ReadOnlySpan<char> s) => Instance.GetBytes(s);

    /// <summary>
    /// Decodes all Kuon Base24 characters in the specified read-only span into a byte array with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static new byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => Instance.GetBytes(s, options);

    /// <summary>
    /// Decodes all Kuon Base24 characters in the specified string into a byte array.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan());

    /// <summary>
    /// Decodes all Kuon Base24 characters in the specified string into a byte array with specified options.
    /// </summary>
    /// <param name="s">The string to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    public static byte[] GetBytes(string s, DataEncodingOptions options) => GetBytes((s ?? throw new ArgumentNullException(nameof(s))).AsSpan(), options);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static volatile IBase24? m_Instance;

    /// <summary>
    /// Returns a default instance of <see cref="KuonBase24"/> encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static IBase24 Instance => m_Instance ??= new KuonBase24();
}
