using System;
using System.IO;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines data encoding options.
    /// </summary>
    [Flags]
    public enum DataEncodingOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Inhibit padding generation in the encoding operation.
        /// </summary>
        Unpad = 1 << 0,

        /// <summary>
        /// Instructs to validate the padding in decoding operation or enforce its generation in encoding operation.
        /// </summary>
        /// <remarks>
        /// In some encodings, the length of an output-encoded string must be a multiple of the predefined number of characters.
        /// The encoder adds padding characters at the end of the output as needed in order to meet this requirement.
        /// The added suffix is called padding.
        /// </remarks>
        Padding = 1 << 1,

        /// <summary>
        /// Do not perform lifetime management of an underlying data object such as <see cref="Stream"/>, <see cref="TextReader"/> or <see cref="TextWriter"/>.
        /// This option only applies to <see cref="IDataEncoding.CreateEncoder(Stream, DataEncodingOptions)"/>, <see cref="IDataEncoding.CreateDecoder(Stream, DataEncodingOptions)"/> and similar operations.
        /// </summary>
        NoOwnership = 1 << 2,

        /// <summary>
        /// Instructs to produce an indented/formatted output if the encoding supports it natively.
        /// For example, Base16 encoding produces the indented output by emitting a whitespace character for every 2 symbols of the output.
        /// </summary>
        Indent = 1 << 3,

        /// <summary>
        /// Instructs to produce a wrapped output if the encoding supports it natively.
        /// For example, Base64 encoding handles wrapping by inserting a new line separator for every 76 symbols of the output.
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
        /// Instructs to produce or verify an embedded checksum if the encoding supports it natively.
        /// </summary>
        Checksum = 1 << 6,

        /// <summary>
        /// Instructs to compress the data during encoding operation if the codec supports it natively.
        /// For example, ZBase32 encoding eliminates the insignificant bits from the output when this option is specified.
        /// </summary>
        Compress = 1 << 7
    }
}
