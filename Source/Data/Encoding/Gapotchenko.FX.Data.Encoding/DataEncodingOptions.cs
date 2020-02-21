using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Instructs to inhibit padding generation in encoding operation.
        /// </summary>
        Unpad = 1 << 0,

        /// <summary>
        /// Validate padding in decoding operation or enforce padding generation in encoding operation.
        /// </summary>
        Padding = 1 << 1,

        /// <summary>
        /// Do not perform the lifetime management of a specified data object such as <see cref="Stream"/>, <see cref="TextReader"/>, <see cref="TextWriter"/> etc.
        /// </summary>
        NoOwnership = 1 << 2,

        /// <summary>
        /// Instructs to produce the indented or formatted output if the encoding supports it natively.
        /// </summary>
        Indent = 1 << 3,

        /// <summary>
        /// Relax decoding rules.
        /// For example, if encoded Base64 string contains an invalid character it gets ignored instead of throwing an exception.
        /// </summary>
        Relax = 1 << 4
    }
}

