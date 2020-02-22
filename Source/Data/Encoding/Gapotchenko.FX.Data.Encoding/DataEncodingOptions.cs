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
        /// Do not perform the lifetime management of a specified data object such as <see cref="Stream"/>, <see cref="TextReader"/>, <see cref="TextWriter"/> etc.
        /// </summary>
        NoOwnership = 1 << 2,

        /// <summary>
        /// Instructs to produce the indented or formatted output if the encoding supports it natively.
        /// For example, Base16 encoding produces the indented output by emitting whitespace delimiters every 2 symbols of output.
        /// </summary>
        Indent = 1 << 3,

        /// <summary>
        /// Instructs to wrap the output in order to form human-readable blocks of data if the encoding supports it natively.
        /// For example, Base64 handles wrapping by inserting line feed characters every 76 symbols of output.
        /// </summary>
        Wrap = 1 << 4,

        /// <summary>
        /// Relax decoding rules.
        /// For example, if encoded Base64 string contains an invalid character it gets ignored instead of throwing an exception.
        /// </summary>
        Relax = 1 << 5
    }
}
