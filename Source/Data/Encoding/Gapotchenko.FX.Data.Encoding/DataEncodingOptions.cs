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
        /// Inhibit padding generation in encoding operation.
        /// </summary>
        Unpad = 1 << 0,

        /// <summary>
        /// Instructs to validate padding in decoding operation or to enforce its generation in encoding operation.
        /// </summary>
        Padding = 1 << 1,

        /// <summary>
        /// Do not perform lifetime management of an underlying data object such as <see cref="Stream"/>, <see cref="TextReader"/> or <see cref="TextWriter"/>.
        /// This option only applies to <see cref="IDataEncoding.CreateEncoder(Stream, DataEncodingOptions)"/>, <see cref="IDataEncoding.CreateDecoder(Stream, DataEncodingOptions)"/> and similar operations.
        /// </summary>
        NoOwnership = 1 << 2,

        /// <summary>
        /// Instructs to produce the indented or formatted output if the encoding supports it natively.
        /// For example, Base16 encoding produces the indented output by emitting whitespace delimiters every 2 symbols of the output.
        /// </summary>
        Indent = 1 << 3,

        /// <summary>
        /// Instructs to produce wrapped output in order to form human-readable blocks of data if the encoding supports it natively.
        /// For example, Base64 handles wrapping by inserting line feed characters every 76 symbols of output.
        /// </summary>
        Wrap = 1 << 4,

        /// <summary>
        /// Relax decoding rules.
        /// For example, if encoded Base64 string contains an invalid character it gets ignored instead of throwing an exception.
        /// </summary>
        Relax = 1 << 5,

        /// <summary>
        /// Instructs to emit checksum if the encoding supports it natively.
        /// </summary>
        Checksum = 1 << 6,

        /// <summary>
        /// Instructs to compress the data during encoding operation if the codec supports it natively.
        /// For example, ZBase32 encoding eliminates the insignificant bits from the output when this option is specified.
        /// </summary>
        Compress = 1 << 7,

        /// <summary>
        /// Instructs to interpret control characters such as EOT (ASCII 0x04, ^D) which indicates the end of transmission.
        /// Other supported control characters are SOH, STX, ETX, FS, GS, RS and US which are treated as transmission block separators.
        /// This option only applies to a streaming decoder.
        /// </summary>
        Control = 1 << 8
    }
}
