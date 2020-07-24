using System;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.Pointer._Legacy_
{
    /// <summary>
    /// Part of SchemaBased XPointer pointer.
    /// </summary>
    internal abstract class PointerPart
    {
        /// <summary>
        /// Evaluates XPointer pointer part and returns pointed nodes.
        /// </summary>
        /// <param name="doc">Document to evaluate pointer part on.</param>
        /// <param name="nm">Namespace manager.</param>
        /// <returns>Pointed nodes.</returns>		    
        public abstract XmlNodeList Evaluate(XmlDocument doc, XmlNamespaceManager nm);
    }
}
