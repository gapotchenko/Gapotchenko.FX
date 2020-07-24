using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Xml.Pointer.Dom
{
    /// <summary>
    /// Scheme-based XML pointer.
    /// </summary>
    public class XSchemeBasedPointer : XPointer
    {
        /// <summary>
        /// Gets the list of parts.
        /// </summary>
        public IList<XPointerPart> Parts { get; } = new List<XPointerPart>();
    }
}
