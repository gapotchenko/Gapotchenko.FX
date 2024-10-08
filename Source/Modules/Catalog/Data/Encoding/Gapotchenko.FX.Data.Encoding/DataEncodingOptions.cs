namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Specifies data encoding options.
/// </summary>
[Flags]
public enum DataEncodingOptions
{
    /// <summary>
    /// No options.
    /// </summary>
    None = 0,

    /// <summary>
    /// Inhibit generation of a padding during the encoding operation.
    /// </summary>
    NoPadding = 1 << 0,

#if BINARY_COMPATIBILITY
    /// <inheritdoc cref="NoPadding"/>
    [Obsolete($"Use {nameof(NoPadding)} instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    Unpad = NoPadding,
#endif

    /// <summary>
    /// Validate the padding in decoding operation or enforce its generation in encoding operation.
    /// </summary>
    /// <remarks>
    /// In some data encodings, the length of an encoded string must be a multiple of the predefined number of characters.
    /// If this is the case, an encoder may add padding characters to the output string as needed in order to meet this requirement.
    /// This added suffix is called padding.
    /// </remarks>
    Padding = 1 << 1,

    /// <summary>
    /// <para>
    /// Leave an underlying data object such as <see cref="Stream"/>, <see cref="TextReader"/>, or <see cref="TextWriter"/> open.
    /// </para>
    /// <para>
    /// This option only applies to
    /// <see cref="IDataEncoding.CreateEncoder(Stream, DataEncodingOptions)"/>,
    /// <see cref="IDataEncoding.CreateDecoder(Stream, DataEncodingOptions)"/>,
    /// and similar operations.
    /// </para>
    /// </summary>
    NoOwnership = 1 << 2,

    /// <summary>
    /// <para>
    /// Produce an indented/formatted output if the encoding supports it natively.
    /// </para>
    /// <para>
    /// For example, Base16 encoding produces the indented output by emitting a whitespace character for every 2 symbols of the output.
    /// </para>
    /// </summary>
    Indent = 1 << 3,

    /// <summary>
    /// <para>
    /// Produce a line-wrapped text output if the encoding supports it natively.
    /// </para>
    /// <para>
    /// For example, Base64 encoding handles wrapping by inserting a new line separator for every 76 symbols of the output.
    /// </para>
    /// </summary>
    Wrap = 1 << 4,

    /// <summary>
    /// <para>
    /// Relax the decoding rules.
    /// For example, if encoded Base64 string contains an invalid character it gets ignored instead of throwing an exception.
    /// </para>
    /// <para>
    /// Alternatively, the flag can be used to lift the restrictions imposed by the encoding operation.
    /// </para>
    /// </summary>
    Relax = 1 << 5,

    /// <summary>
    /// Produce or verify an embedded checksum if the encoding supports it natively.
    /// </summary>
    Checksum = 1 << 6,

    /// <summary>
    /// <para>
    /// Compress the data during encoding operation if the data encoding supports it natively.
    /// </para>
    /// <para>
    /// For example, ZBase32 encoding eliminates the insignificant bits from the output when this option is specified.
    /// </para>
    /// </summary>
    Compress = 1 << 7,

    /// <summary>
    /// <para>
    /// Use lowercase characters for a text output of the encoding instead of its default preference.
    /// </para>
    /// <para>
    /// This option can only be specified for case-insensitive text data encodings.
    /// </para>
    /// </summary>
    Lowercase = 1 << 8,

    /// <summary>
    /// <para>
    /// Use uppercase characters for a text output of the encoding instead of its default preference.
    /// </para>
    /// <para>
    /// This option can only be specified for case-insensitive text data encodings.
    /// </para>
    /// </summary>
    Uppercase = 1 << 9,

    /// <summary>
    /// <para>
    /// Disallows characters not present in the encoding alphabet.
    /// </para>
    /// <para>
    /// If this option is specified, the text data encoding will not allow non-significant characters such as white spaces during decoding operation treating them as invalid characters.
    /// </para>
    /// </summary>
    Pure = 1 << 10
}
