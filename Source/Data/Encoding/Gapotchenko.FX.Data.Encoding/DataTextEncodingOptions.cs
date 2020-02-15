using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines binary-to-text encoding options.
    /// </summary>
    [Flags]
    public enum DataTextEncodingOptions
    {
        /// <summary>
        /// The default options.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Do not add padding to the encoded string representation.
        /// </summary>
        NoPadding = 1 << 0
    }
}

