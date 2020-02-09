using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Pointer.Dom
{
    /// <summary>
    /// Shorthand XML pointer.
    /// </summary>
    public sealed class XShorthandPointer : XPointer
    {
        /// <summary>
        /// Gets or sets NCName for a shorthand XML pointer.
        /// It identifies at most one element in the resource's information set; specifically, the first one in document order that has a matching value as an identifier.
        /// </summary>
        public string NCName { get; set; }
    }
}
