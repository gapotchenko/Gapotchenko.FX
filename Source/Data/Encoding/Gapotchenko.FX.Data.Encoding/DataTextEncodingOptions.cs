using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Binary-to-text encoding options.
    /// </summary>
    [Flags]
    public enum DataTextEncodingOptions
    {
        /// <summary>
        /// The default options.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Instructs to inhibit padding production in encoding operation, or its consumption in decoding operation.
        /// </summary>
        NoPadding = 1 << 0,

        /// <summary>
        /// Instructs to produce padding in encoding operation, or to make its presence obligatory in decoding operation.
        /// </summary>
        RequirePadding = 1 << 1
    }
}

