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
        /// Instructs to inhibit padding generation.
        /// </summary>
        InhibitPadding = 1 << 0,

        /// <summary>
        /// Instructs to enforce padding generation and its presence validation.
        /// </summary>
        RequirePadding = 1 << 1,

        /// <summary>
        /// Instructs to not perform the lifetime management of an underlying data object such as <see cref="Stream"/>, <see cref="TextReader"/> or <see cref="TextWriter"/>.
        /// </summary>
        NoOwnership = 1 << 2,

        /// <summary>
        /// Instructs to produce the indented or formatted output if the encoding natively supports it.
        /// </summary>
        Indent = 1 << 3
    }
}

