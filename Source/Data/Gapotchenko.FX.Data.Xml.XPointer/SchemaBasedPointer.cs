using System;
using System.Collections;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.XPointer
{
    /// <summary>
    /// SchemaBased XPointer pointer.
    /// </summary>
    internal class SchemaBasedPointer : XPointer
    {
        private ArrayList _parts;

        /// <summary>
        /// Pointer parts collection.
        /// </summary>
        public ArrayList Parts
        {
            get { return _parts; }
        }

        public SchemaBasedPointer(ArrayList parts)
        {
            _parts = parts;
        }

        /// <summary>
        /// Evaluates this pointer.
        /// </summary>
        /// <param name="doc">Document to evaluate pointer on.</param>
        /// <returns>Pointed nodes.</returns>
        public override XmlNodeList Evaluate(XmlDocument doc)
        {
            XmlNodeList result;
            var nm = new XmlNamespaceManager(doc.NameTable);
            for (int i = 0; i < _parts.Count; i++)
            {
                PointerPart part = (PointerPart)_parts[i];
                result = part.Evaluate(doc, nm);
                if (result != null && result.Count > 0)
                    return result;
            }
            throw new XPointerMatchException("XPointer does not match any sub-resource.");
        }
    }
}
