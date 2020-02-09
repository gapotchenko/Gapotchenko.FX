using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#nullable enable

namespace Gapotchenko.FX.Data.Xml.Pointer.Dom
{
    /// <summary>
    /// The part of a scheme-based XML pointer.
    /// </summary>
    public abstract class XPointerPart
    {
        /// <summary>
        /// Gets scheme name.
        /// </summary>
        public abstract XName SchemeName { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string? m_SchemeNamePrefix;

        /// <summary>
        /// Gets an existing or sets the desired scheme name prefix.
        /// </summary>
        public string? SchemeNamePrefix
        {
            get => m_SchemeNamePrefix;
            set
            {
                value = Empty.Nullify(value);
                if (value != null && SchemeName.Namespace == XNamespace.None)
                    throw new InvalidOperationException("Cannot set a scheme name prefix for XML pointer part without a namespace.");
                m_SchemeNamePrefix = value;
            }
        }

        /// <summary>
        /// Gets or sets the scheme data.
        /// </summary>
        public abstract string SchemeData { get; set; }
    }
}
