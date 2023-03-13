namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Defines the interface of a binary-to-text encoding.
/// </summary>
public interface ITextDataEncoding : IDataEncoding
{
    /// <summary>
    /// Gets a value indicating whether encoding is case-sensitive.
    /// </summary>
    bool IsCaseSensitive { get; }

    /// <summary>
    /// Encodes all the bytes in the specified span into a string.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <returns>The string with encoded data.</returns>
    string GetString(ReadOnlySpan<byte> data);

    /// <summary>
    /// Encodes all the bytes in the specified span into a string with specified options.
    /// </summary>
    /// <param name="data">The byte span to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The string with encoded data.</returns>
    string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into a byte array.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    byte[] GetBytes(ReadOnlySpan<char> s);

    /// <summary>
    /// Decodes all the characters in the specified read-only span into a byte array with specified options.
    /// </summary>
    /// <param name="s">The read-only character span to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>A byte array with decoded data.</returns>
    /// <exception cref="FormatException">Cannot decode the input string.</exception>
    byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options);

    /// <summary>
    /// Creates a streaming encoder.
    /// </summary>
    /// <param name="textWriter">The output text writer of an encoder.</param>
    /// <param name="options">The options.</param>
    /// <returns>The stream.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="textWriter"/> argument is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">The encoding does not support streaming operations.</exception>
    Stream CreateEncoder(TextWriter textWriter, DataEncodingOptions options = DataEncodingOptions.None);

    /// <summary>
    /// Creates a streaming decoder.
    /// </summary>
    /// <param name="textReader">The input text reader of a decoder.</param>
    /// <param name="options">The options.</param>
    /// <returns>The stream.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="textReader"/> argument is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">The encoding does not support streaming operations.</exception>
    Stream CreateDecoder(TextReader textReader, DataEncodingOptions options = DataEncodingOptions.None);

    /// <summary>
    /// Adds padding to the specified encoded read-only character span.
    /// </summary>
    /// <remarks>
    /// In some encodings, the length of an encoded string representation must be a multiple of the predefined number of characters.
    /// This method concatenates the specified character span with padding characters to meet that requirement and returns the resulting string.
    /// </remarks>
    /// <param name="s">The read-only character span to add padding to.</param>
    /// <returns>An encoded string with added padding.</returns>
    string Pad(ReadOnlySpan<char> s);

    /// <summary>
    /// Removes padding from the specified encoded read-only character span.
    /// </summary>
    /// <param name="s">The read-only character span to remove padding from.</param>
    /// <returns>A read-only character span with removed padding.</returns>
    ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s);

    /// <summary>
    /// <para>
    /// Gets a value indicating whether the current encoding supports canonicalization.
    /// </para>
    /// <para>
    /// Canonicalization is the process of normalization of the encoded symbols with their canonical forms.
    /// </para>
    /// </summary>
    bool CanCanonicalize { get; }

    /// <summary>
    /// Canonicalizes the encoded string.
    /// Canonicalization substitutes the encoded symbols with their canonical forms.
    /// Unrecognized and whitespace symbols are left intact.
    /// </summary>
    /// <param name="s">The encoded string.</param>
    /// <returns>The canonicalized encoded string.</returns>
    string Canonicalize(ReadOnlySpan<char> s);

    /// <summary>
    /// Calculates the maximum number of characters produced by encoding the specified number of bytes.
    /// </summary>
    /// <param name="byteCount">The number of bytes to encode.</param>
    /// <returns>The maximum number of characters produced by encoding the specified number of bytes.</returns>
    int GetMaxCharCount(int byteCount);

    /// <summary>
    /// Calculates the maximum number of characters produced by encoding the specified number of bytes with options.
    /// </summary>
    /// <param name="byteCount">The number of bytes to encode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The maximum number of characters produced by encoding the specified number of bytes.</returns>
    int GetMaxCharCount(int byteCount, DataEncodingOptions options);

    /// <summary>
    /// Calculates the maximum number of bytes produced by decoding the specified number of characters.
    /// </summary>
    /// <param name="charCount">The number of characters to decode.</param>
    /// <returns>The maximum number of bytes produced by decoding the specified number of characters.</returns>
    int GetMaxByteCount(int charCount);

    /// <summary>
    /// Calculates the maximum number of bytes produced by decoding the specified number of characters with options.
    /// </summary>
    /// <param name="charCount">The number of characters to decode.</param>
    /// <param name="options">The options.</param>
    /// <returns>The maximum number of bytes produced by decoding the specified number of characters.</returns>
    int GetMaxByteCount(int charCount, DataEncodingOptions options);
}
